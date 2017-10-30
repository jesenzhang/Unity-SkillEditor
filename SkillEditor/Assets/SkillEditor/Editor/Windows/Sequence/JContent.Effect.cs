using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {
        private void AddRenderDataForEffect(JTimelineBase timeline)
        {
            if (timeline is JTimelineEffect)
            {
                JTimelineEffect tline = (JTimelineEffect)timeline;
                for (int k = 0; k < tline.EffectTracks.Count; k++)
                {
                    List<JClipRenderData> list = new List<JClipRenderData>();
                    JEffectTrack track = tline.EffectTracks[k];
                    for (int l = 0; l < track.TrackClips.Count; l++)
                    {
                        JEffectClipData key = track.TrackClips[l];
                        var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                        cachedData.ClipData = key;
                        list.Add(cachedData);
                    }

                    if (!timelineClipRenderDataMap.ContainsKey(track))
                        timelineClipRenderDataMap.Add(track, list);
                    else
                    {
                        timelineClipRenderDataMap[track] = list;
                    }
                }
            }
        }


        private void AddNewEffectTrack(JTimelineBase line)
        {
            if (line is JTimelineEffect)
            {
                JTimelineEffect tline = (JTimelineEffect)line;
                var track = ScriptableObject.CreateInstance<JEffectTrack>();
                tline.AddTrack(track);
                AddRenderDataForEffect(tline);
            }
        }

        private void AddNewEffectClip(JEffectTrack track, float time)
        {
            var clipData = ScriptableObject.CreateInstance<JEffectClipData>();
            JTimelineEffect line = (JTimelineEffect)track.TimeLine;
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            clipData.StateName = "";
            clipData.PlaybackDuration = 1;
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

        private void AddNewEffectClip(JEffectTrack track, EffectType type, float time)
        {
            var clipData = ScriptableObject.CreateInstance<JEffectClipData>();
            JTimelineEffect line = (JTimelineEffect)track.TimeLine;
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            clipData.StateName = "";
            clipData.PlaybackDuration = 1;
            clipData.Track = track;
            clipData.effectType = type;
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

        private GenericMenu MenuForEffectTimeLine(JTimelineBase line, JEffectTrack track)
        {
            GenericMenu contextMenu = new GenericMenu();
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * line.Sequence.Duration) / XScale;

            contextMenu.AddItem(new GUIContent("AddNewTrack"),
                    false, (obj) => AddNewEffectTrack(((JTimelineBase)((object[])obj)[0])),
                    new object[] { line });

      
            foreach (var effectType in Enum.GetValues(typeof(EffectType)))
            {
                string name = Enum.GetName(typeof(EffectType),(EffectType)effectType); 
                contextMenu.AddItem(new GUIContent("AddClip/"+ name),

                    false, (obj) => AddNewEffectClip(((JEffectTrack)((object[])obj)[0]), ((EffectType)((object[])obj)[1]), ((float)((object[])obj)[2])),
                    new object[] { track, (EffectType)effectType, newTime });
            }
            
            contextMenu.AddItem(new GUIContent("DeleteLine"),
                     false, (obj) => RemoveEffectLine(((JEffectTrack)((object[])obj)[0])),
                     new object[] { track });

            return contextMenu;
        }
        private void RemoveEffectLine(JEffectTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);

            }
            JTimelineEffect line = (JTimelineEffect)track.TimeLine;
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.EffectTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.Timelines.Length == 0)
            {
                DestroyImmediate(contain.gameObject);
            }
        }
        private void RemoveEffectClip(JClipRenderData clip)
        {
            if (clip.ClipData is JEffectClipData)
            {
                JEffectClipData anidata = (JEffectClipData)clip.ClipData;
                if (timelineClipRenderDataMap.ContainsKey(anidata.Track))
                    if (timelineClipRenderDataMap[anidata.Track].Contains(clip))
                        timelineClipRenderDataMap[anidata.Track].Remove(clip);
                anidata.Track.RemoveClip(anidata);
            }
        }

        private void EffectGUI(JTimelineBase timeline, JEffectTrack track, JClipRenderData[] renderDataList)
        {
            if (timeline is JTimelineEffect)
            {
                JTimelineEffect Effectline = (JTimelineEffect)timeline;

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
                    JEffectClipData EffectClipData = (JEffectClipData)renderdata.ClipData;
                    JEffectTrack linetrack = EffectClipData.Track;
                    if (linetrack != track)
                    {
                        continue;
                    }
                    var startX = ConvertTimeToXPos(EffectClipData.StartTime);
                    var endX = ConvertTimeToXPos(EffectClipData.StartTime + EffectClipData.PlaybackDuration);
                    var transitionX = ConvertTimeToXPos(EffectClipData.StartTime + EffectClipData.PlaybackDuration);
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

                    // GUI.color = new Color(156 / 255.0f, 11 / 255.0f, 11 / 255.0f, 1);
                    
                   // GUI.color = ColorTools.GetGrandientColor((float)Effectline.EffectTracks.IndexOf(track)/ (float)Effectline.EffectTracks.Count);
                    GUI.color = ColorTools.GetGrandientColor((float)renderdata.index / (float)CountClip);

                    if (SelectedObjects.Contains(renderdata))
                    {
                        GUI.color = ColorTools.SelectColor;
                    }

                    GUI.Box(renderRecttemp, "", USEditorUtility.NormalWhiteOutLineBG);
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
                    renderdata.ClipData = EffectClipData;


                    labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.

                    GUI.color = Color.black;
                    GUI.Label(labelRect, EffectClipData.FriendlyName);

                    GUI.color = Color.white;

                    if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                    {
                        hasBox = true;
                        contextMenu.AddItem(new GUIContent("DeleteClip"),
                               false, (obj) => RemoveEffectClip(((JClipRenderData)((object[])obj)[0])),
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
                    Selection.activeGameObject = Effectline.gameObject;
                    EditorGUIUtility.PingObject(Effectline.gameObject);

                }
                if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    contextMenu = MenuForEffectTimeLine(Effectline, track);

                }
                if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    UnityEngine.Event.current.Use();
                    contextMenu.ShowAsContext();
                }
            }
        }
        //侧边栏
        private void SideBarAndLineForEffect(JTimelineBase timeline)
        {
            if (timeline is JTimelineEffect)
            {
                GUILayout.BeginVertical();
                JTimelineEffect transformline = (JTimelineEffect)timeline;
                for (int j = 0; j < transformline.EffectTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + transformline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "EffectTimeline for" + transformline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect ContentRect = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal();

                    Rect addRect = FloatingRect;
                    Rect labelRect = addRect;
                    labelRect.x += 40;
                    labelRect.width -= (lineHeight + 41);
                    GUI.Label(labelRect, "Track " + j);

                    //轨道名字
                    Rect nameRect = addRect;
                    nameRect.x += 40 + lineHeight + 40;
                    nameRect.width -= (lineHeight + 120);
                    //transformline.EffectTracks[j].name = GUI.TextField(nameRect, transformline.EffectTracks[j].name);

                    int select = SkillNames.EffectNames.Contains(transformline.EffectTracks[j].name) ? SkillNames.EffectNames.IndexOf(transformline.EffectTracks[j].name) : 0;
                    select = EditorGUI.Popup(nameRect, select, SkillNames.EffectNames.ToArray());
                    transformline.EffectTracks[j].name = SkillNames.EffectNames[select];

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    transformline.EffectTracks[j].Enable = GUI.Toggle(enableRect, transformline.EffectTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();

                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForEffectTimeLine(transformline, transformline.EffectTracks[j]);
                        contextMenu.ShowAsContext();
                    }

                    if (timelineClipRenderDataMap.ContainsKey(transformline.EffectTracks[j]))
                    {
                        ///时间线背景 区域 只接受右键
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[transformline.EffectTracks[j]];
                        EffectGUI(transformline, transformline.EffectTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();
                    }
                }
                GUILayout.EndVertical();
            }

        }
        //单独选中一个关键帧
        private void OnSingleEffectSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JEffectClipData)
            {
                Selection.activeObject = (JEffectClipData)selectobj.ClipData;
            }
        }

        //开始拖动记录原始位置
        private void StartDraggingEffectClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JEffectClipData)
            {
                SourcePositions[clipData] = ((JEffectClipData)trackdata).StartTime;
            }
        }
        //拖动
        private void ProcessDraggingEffectClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = (ScriptableObject)clip.ClipData;
                if (selected is JEffectClipData)
                {
                    JEffectClipData animationClipData = selected as JEffectClipData;

                    //float newTime = ((newPosition.x / DisplayArea.width) * AnimationTimeline.Sequence.Duration) / XScale;
                    // newTime = Mathf.Clamp(newTime, 0.0f, AnimationTimeline.Sequence.Duration);

                    float mousePosOnTimeline = ContentXToTime(FloatingWidth + mouseDelta.x);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration - animationClipData.PlaybackDuration);
                    animationClipData.StartTime = newTime;
                    
                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = animationClipData;
                    }
                }
            }
        }


    }
}
