using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
namespace CySkillEditor
{
    public class ContextData
    {
        public int ControlID;
        public Type Type;
        public Type PairedType;
        public JTimelineBase Line;
        public string Category;
        public string Label;
        public float Firetime;

        public ContextData(int controlId, Type type, Type pairedType, JTimelineBase line, string category, string label, float firetime)
        {
            ControlID = controlId;
            Type = type;
            PairedType = pairedType;
            Line = line;
            Category = category;
            Label = label;
            Firetime = firetime;
        }
    }

    public partial class JContent : ScriptableObject
    {

        private int controlID; // The control ID for this track control.
        private ContextData savedData = null; // Saved data from the object picker.


        private void showObjectPicker(ContextData data)
        {
            // Create an Object Picker with a dynamic type.
            MethodInfo method = typeof(EditorGUIUtility).GetMethod("ShowObjectPicker");
            MethodInfo generic = method.MakeGenericMethod(data.PairedType);
            generic.Invoke(this, new object[] { null, false, string.Empty, data.ControlID });

            savedData = data;
        }


        private void AddRenderDataForSound(JTimelineBase timeline)
        {
            if (timeline is JTimelineSound)
            {
                JTimelineSound Soundline = (JTimelineSound)timeline;
                List<JClipRenderData> list = new List<JClipRenderData>();
                for (int k = 0; k < Soundline.SoundTracks.Count; k++)
                {
                    JSoundTrack track = Soundline.SoundTracks[k];
                    for (int l = 0; l < track.TrackClips.Count; l++)
                    {
                        JSoundClipData SoundClipData = track.TrackClips[l];
                        var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                        cachedData.ClipData = SoundClipData;
                        list.Add(cachedData);
                    }
                    if (!timelineClipRenderDataMap.ContainsKey(track))
                        timelineClipRenderDataMap.Add(track, list);
                }
            }

        }
        private void AddNewSoundState(JTimelineSound line, JSoundTrack track, float time, string stateName, AudioClip clip)
        {
            controlID = GUIUtility.GetControlID(track.GetInstanceID(), FocusType.Passive);

            var clipData = ScriptableObject.CreateInstance<JSoundClipData>();
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            if (clip == null)
            {
                clipData.Clip = line.OrientationClip;
            }
            else
            {
                clipData.Clip = clip;
            }

            if (clipData.Clip == null)
            {
                SequenceWindow.ShowNotification(new GUIContent("AudioClip is NULL"));
            }
            clipData.SoundDuration = clipData.Clip.length;
            clipData.PlaybackDuration = clipData.SoundDuration;
            clipData.Track = track;
            track.AddClip(clipData);
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                cachedData.ClipData = clipData;
                timelineClipRenderDataMap[track].Add(cachedData);
            }
            else
            {
                var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                cachedData.ClipData = clipData;
                List<JClipRenderData> list = new List<JClipRenderData>();
                list.Add(cachedData);
                timelineClipRenderDataMap.Add(track, list);
            }
        }
        private void PickNewAudioClip(JTimelineSound line, JSoundTrack track, float time, string stateName)
        {
            ContextData data = new ContextData(controlID, typeof(JTimelineSound), typeof(AudioClip), line, "", "", time);
            showObjectPicker(data);
        }

        private void AddNewSoundTrack(JTimelineBase line)
        {
            if (line is JTimelineSound)
            {
                JTimelineSound tline = (JTimelineSound)line;
                var track = ScriptableObject.CreateInstance<JSoundTrack>();
                tline.AddTrack(track);
                AddRenderDataForSound(tline);
            }
        }

        private void RemoveSoundTimeline(JTimelineSound line, JSoundTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            JTimelineContainer contain = line.TimelineContainer;
            line.RemoveTrack(track);
            if (line.SoundTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.Timelines.Length == 0)
            {
                DestroyImmediate(contain.gameObject);
            }

        }

        private void SoundGUI(JTimelineSound Soundline, JSoundTrack linetrack, JClipRenderData[] renderDataList)
        {
            GenericMenu contextMenu = new GenericMenu();
            ///event 右键点击
            bool isContext = UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 1;
            bool isChoose = UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0 && UnityEngine.Event.current.clickCount == 1;
            bool hasBox = false;

            Rect DisplayAreaTemp = DisplayArea;
            DisplayAreaTemp.x = 0;
            DisplayAreaTemp.y = 0;

            for (int j = 0; j < renderDataList.Length; j++)
            {
                JClipRenderData renderdata = renderDataList[j];
                JSoundClipData SoundClipData = (JSoundClipData)renderdata.ClipData;
                JSoundTrack track = SoundClipData.Track;
                if (linetrack != track)
                {
                    continue;
                }
                var startX = ConvertTimeToXPos(SoundClipData.StartTime);
                var endX = ConvertTimeToXPos(SoundClipData.StartTime + SoundClipData.PlaybackDuration);
                var transitionX = ConvertTimeToXPos(SoundClipData.StartTime + SoundClipData.TransitionDuration);
                var handleWidth = 2.0f;

                Rect renderRect = new Rect(startX, DisplayArea.y, endX - startX, DisplayArea.height);
                Rect transitionRect = new Rect(startX, DisplayArea.y, transitionX - startX, DisplayArea.height);
                Rect leftHandle = new Rect(startX, DisplayArea.y, handleWidth * 2.0f, DisplayArea.height);
                Rect rightHandle = new Rect(endX - (handleWidth * 2.0f), DisplayArea.y, handleWidth * 2.0f, DisplayArea.height);
                Rect labelRect = new Rect();

                Rect renderRecttemp = renderRect;
                renderRecttemp.x -= DisplayArea.x;
                renderRecttemp.y = 0;
                Rect transitionRecttemp = transitionRect;
                transitionRecttemp.y = 0;
                transitionRecttemp.x -= DisplayArea.x;
                Rect leftHandletemp = leftHandle;
                leftHandletemp.y = 0;
                leftHandletemp.x -= DisplayArea.x;
                Rect rightHandletemp = rightHandle;
                rightHandletemp.x -= DisplayArea.x;
                rightHandletemp.y = 0;

                GUI.color = new Color(243 / 255.0f, 154 / 255.0f, 0 / 255.0f, 1f);
                if (SelectedObjects.Contains(renderdata))
                {
                    GUI.color = Color.yellow;
                }
                Texture2D texture = AssetPreview.GetAssetPreview(SoundClipData.Clip);
                if (texture != null)
                {
                    GUI.DrawTexture(renderRecttemp, texture, ScaleMode.StretchToFill);
                }
                else
                {
                    GUI.Box(renderRecttemp, "", USEditorUtility.NormalWhiteOutLineBG);
                }
                if (SoundClipData.CrossFade)
                    GUI.Box(transitionRecttemp, "");

                GUI.Box(leftHandletemp, "");
                GUI.Box(rightHandletemp, "");

                labelRect = renderRecttemp;
                labelRect.width = DisplayArea.width;

                renderdata.renderRect = renderRect;
                renderdata.labelRect = renderRect;
                renderdata.renderPosition = new Vector2(startX, DisplayArea.y);
                renderdata.transitionRect = transitionRect;
                renderdata.leftHandle = leftHandle;
                renderdata.rightHandle = rightHandle;
                renderdata.ClipData = SoundClipData;

                if (SoundClipData.CrossFade)
                    labelRect.x = labelRect.x + (transitionX - startX);
                else
                    labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.
                GUI.color = Color.black;
                GUI.Label(labelRect, SoundClipData.FriendlyName);

                GUI.color = Color.white;

                if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    hasBox = true;
                    contextMenu.AddItem(new GUIContent("DeleteClip"),
                           false, (obj) => RemoveSoundClip(((JClipRenderData)((object[])obj)[0])),
                           new object[] { renderdata });
                }
                if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    UnityEngine.Event.current.Use();
                    contextMenu.ShowAsContext();
                }
            }
            if (!hasBox && isChoose && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition) && (UnityEngine.Event.current.control || UnityEngine.Event.current.command))
            {
                //代码选中hierarchy中的对象 显示inspector 按住Ctrl or command
                //GameObject go = GameObject.Find(Animationline.gameObject.name);
                Selection.activeGameObject = Soundline.gameObject;
                EditorGUIUtility.PingObject(Soundline.gameObject);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                contextMenu = MenuForSoundTimeLine(Soundline, linetrack);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                UnityEngine.Event.current.Use();
                contextMenu.ShowAsContext();
            }

            // Handle the case where the object picker has a value selected.
            if (UnityEngine.Event.current.type == EventType.ExecuteCommand && UnityEngine.Event.current.commandName == "ObjectSelectorClosed")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == controlID)
                {
                    UnityEngine.Object pickedObject = EditorGUIUtility.GetObjectPickerObject();

                    if (pickedObject != null)
                    {
                        AudioClip clip = (AudioClip)pickedObject;
                        AddNewSoundState((JTimelineSound)savedData.Line, linetrack, savedData.Firetime, clip.name, clip);
                    }
                    UnityEngine.Event.current.Use();
                }
            }
        }

        private GenericMenu MenuForSoundTimeLine(JTimelineSound Soundline, JSoundTrack linetrack)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("AddNewTrack"),
             false, (obj) => AddNewSoundTrack(((JTimelineBase)((object[])obj)[0])),
                new object[] { Soundline });

            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * Soundline.Sequence.Duration) / XScale;

            contextMenu.AddItem(
                    new GUIContent("Add Original AudioClip"),
                    false,
                    (obj) => AddNewSoundState(((JTimelineSound)((object[])obj)[0]), ((JSoundTrack)((object[])obj)[1]), ((float)((object[])obj)[2]), ((string)((object[])obj)[3]), null),
                    new object[] { Soundline, linetrack, newTime, "" }
                );
            //删除时

            contextMenu.AddItem(
                    new GUIContent("Add New AudioClip"),
                    false,
                    (obj) => PickNewAudioClip(((JTimelineSound)((object[])obj)[0]), ((JSoundTrack)((object[])obj)[1]), ((float)((object[])obj)[2]), ((string)((object[])obj)[3])),
                    new object[] { Soundline, linetrack, newTime, "" }
                );
            //删除时间线
            contextMenu.AddItem(new GUIContent("DeleteSoundLine"),
                    false, (obj) => RemoveSoundTimeline(((JTimelineSound)((object[])obj)[0]), ((JSoundTrack)((object[])obj)[1])),
                    new object[] { Soundline, linetrack });
            return contextMenu;
        }
        private void SideBarAndLineForSound(JTimelineBase timeline)
        {
            if (timeline is JTimelineSound)
            {
                GUILayout.BeginVertical();
                JTimelineSound soundline = (JTimelineSound)timeline;
                for (int j = 0; j < soundline.SoundTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + soundline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "SoundTimeline for" + soundline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect ContentRect = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal();

                    //GUILayout.Box("" + soundline.AffectedObject.name, USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect addRect = FloatingRect;// GUILayoutUtility.GetLastRect();
                    Rect labelRect = addRect;
                    labelRect.x += 40;
                    labelRect.width -= (lineHeight + 41);
                    GUI.Label(labelRect, "Track " + j);

                    //轨道名字
                    Rect nameRect = addRect;
                    nameRect.x += 40 + lineHeight + 40;
                    nameRect.width -= (lineHeight + 120);
                    soundline.SoundTracks[j].name=GUI.TextField(nameRect, soundline.SoundTracks[j].name);


                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    soundline.SoundTracks[j].Enable = GUI.Toggle(enableRect, soundline.SoundTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();

                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForSoundTimeLine(soundline, soundline.SoundTracks[j]);
                        contextMenu.ShowAsContext();
                    }

                    if (timelineClipRenderDataMap.ContainsKey(soundline.SoundTracks[j]))
                    {
                        ///时间线背景 区域 只接受右键
                        DisplayArea = ContentRect;
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[soundline.SoundTracks[j]];
                        SoundGUI(soundline, soundline.SoundTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();
                    }
                }
                GUILayout.EndVertical();
            }
        }
        private void RemoveSoundClip(JClipRenderData clip)
        {
            var clipdata = clip.ClipData;
            if (clipdata is JSoundClipData)
            {
                JSoundClipData anidata = (JSoundClipData)clipdata;
                if (timelineClipRenderDataMap.ContainsKey(anidata.Track))
                    if (timelineClipRenderDataMap[anidata.Track].Contains(clip))
                        timelineClipRenderDataMap[anidata.Track].Remove(clip);
                anidata.Track.RemoveClip(anidata);
            }

        }

        private void StartDraggingSoundClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JSoundClipData)
            {
                SourcePositions[clipData] = ((JSoundClipData)trackdata).StartTime;
            }
        }
        private void OnSingleSoundSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JSoundClipData)
            {
                Selection.activeObject = (JSoundClipData)selectobj.ClipData;
            }
        }
        private void ProcessDraggingSoundClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = clip.ClipData;
                if (selected is JSoundClipData)
                {
                    JSoundClipData soundClipData = selected as JSoundClipData;
                    float mousePosOnTimeline = ContentXToTime(mouseDelta.x + FloatingWidth);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration - soundClipData.PlaybackDuration);
                    soundClipData.StartTime = newTime;
                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = soundClipData;
                    }
                }
            }
        }
    }
}
