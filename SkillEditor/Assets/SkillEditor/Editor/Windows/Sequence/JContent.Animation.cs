using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {
        //为时间线添加绘制片段数据
        private void AddRenderDataForAnimation(JTimelineBase timeline)
        {
            if (timeline is JTimelineAnimation)
            {
                JTimelineAnimation animationline = (JTimelineAnimation)timeline;
                List<JClipRenderData> list = new List<JClipRenderData>();
                for (int k = 0; k < animationline.AnimationTracks.Count; k++)
                {
                    JAnimationTrack track = animationline.AnimationTracks[k];
                    for (int l = 0; l < track.TrackClips.Count; l++)
                    {
                        JAnimationClipData animationClipData = track.TrackClips[l];
                        var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                        cachedData.ClipData = animationClipData;
                        list.Add(cachedData);
                    }
                    if (!timelineClipRenderDataMap.ContainsKey(track))
                        timelineClipRenderDataMap.Add(track, list);
                }
            }
        }
        //添加动作片段
        private void AddNewAnimationState(JTimelineAnimation line, JAnimationTrack track, float time, string stateName)
        {
            var clipData = ScriptableObject.CreateInstance<JAnimationClipData>();
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            clipData.StateName = stateName;
            clipData.StateDuration = MecanimAnimationUtility.GetStateDuration(stateName, line.AffectedObject.gameObject);
            clipData.PlaybackDuration = clipData.StateDuration;
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
        //移除时间线
        private void RemoveAnimationTimeline(JTimelineAnimation line, JAnimationTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.AnimationTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.transform.childCount == 0)
            {
                DestroyImmediate(contain.gameObject);
            }
        }
        private void AddNewAnimationTrack(JTimelineBase line)
        {
            if (line is JTimelineAnimation)
            {
                JTimelineAnimation tline = (JTimelineAnimation)line;
                var track = ScriptableObject.CreateInstance<JAnimationTrack>();
                tline.AddTrack(track);
                AddRenderDataForAnimation(tline);
            }
        }

        //绘制动作片段UI
        private void AnimationGUI(JTimelineAnimation Animationline, JAnimationTrack linetrack, JClipRenderData[] renderDataList)
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
                JAnimationClipData animationClipData = (JAnimationClipData)renderdata.ClipData;
                JAnimationTrack track = animationClipData.Track;
                if (linetrack != track)
                {
                    continue;
                }
                var startX = ConvertTimeToXPos(animationClipData.StartTime);
                var endX = ConvertTimeToXPos(animationClipData.StartTime + animationClipData.PlaybackDuration);
                var transitionX = ConvertTimeToXPos(animationClipData.StartTime + animationClipData.TransitionDuration);
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

                //GUI.color = new Color(70 / 255.0f, 147 / 255.0f, 236 / 255.0f, 1);
                GUI.color = ColorTools.GetGrandientColor((float)renderdata.index / (float)CountClip);

                if (SelectedObjects.Contains(renderdata))
                {
                    GUI.color = ColorTools.SelectColor;// Color.yellow;
                }

                GUI.Box(renderRecttemp, "", USEditorUtility.NormalWhiteOutLineBG);
                if (animationClipData.CrossFade)
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
                renderdata.ClipData = animationClipData;

                if (animationClipData.CrossFade)
                    labelRect.x = labelRect.x + (transitionX - startX);
                else
                    labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.

                GUI.color = Color.black;
                GUI.Label(labelRect, animationClipData.FriendlyName);

                GUI.color = Color.white;

                if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    hasBox = true;
                    contextMenu.AddItem(new GUIContent("DeleteClip"),
                           false, (obj) => RemoveAnimationClip(((JClipRenderData)((object[])obj)[0])),
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
                Selection.activeGameObject = Animationline.gameObject;
                EditorGUIUtility.PingObject(Animationline.gameObject);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                contextMenu = MenuForAnimationTimeLine(Animationline, linetrack);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                UnityEngine.Event.current.Use();
                contextMenu.ShowAsContext();
            }
        }

        //时间线右键点击的菜单
        private GenericMenu MenuForAnimationTimeLine(JTimelineAnimation Animationline, JAnimationTrack linetrack)
        {
            GenericMenu contextMenu = new GenericMenu();
            List<string> allStates = new List<string>();
            var animationLayers = MecanimAnimationUtility.GetAllLayerNames(Animationline.AffectedObject.gameObject);

            contextMenu.AddItem(new GUIContent("AddNewTrack"),
            false, (obj) => AddNewAnimationTrack(((JTimelineBase)((object[])obj)[0])),
            new object[] { Animationline });

            foreach (var animationLayer in animationLayers)
            {
                int layer = MecanimAnimationUtility.LayerNameToIndex(Animationline.AffectedObject.gameObject, animationLayer);
                List<string> layerallStates = MecanimAnimationUtility.GetAllStateNames(Animationline.AffectedObject.gameObject, layer);
                foreach (var statename in layerallStates)
                {
                    string Addstate = "Layer/{0}/{1}";
                    allStates.Add(string.Format(Addstate, animationLayer, statename));
                }
            }
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * Animationline.Sequence.Duration) / XScale;
            foreach (var state in allStates)
                contextMenu.AddItem(
                    new GUIContent(state),
                    false,
                    (obj) => AddNewAnimationState(((JTimelineAnimation)((object[])obj)[0]), ((JAnimationTrack)((object[])obj)[1]), ((float)((object[])obj)[2]), ((string)((object[])obj)[3])),
                    new object[] { Animationline, linetrack, newTime, state.Split('/')[2] }
                );
            //删除时间线
            contextMenu.AddItem(new GUIContent("DeleteAnimationLine"),
                    false, (obj) => RemoveAnimationTimeline(((JTimelineAnimation)((object[])obj)[0]), ((JAnimationTrack)((object[])obj)[1])),
                    new object[] { Animationline, linetrack });
            return contextMenu;
        }
        //时间线的侧边栏绘制
        private void SideBarAndLineForAnimation(JTimelineBase timeline)
        {
            if (timeline is JTimelineAnimation)
            {
                GUILayout.BeginVertical();
                JTimelineAnimation animationline = (JTimelineAnimation)timeline;
                for (int j = 0; j < animationline.AnimationTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + animationline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "AnimationTimeline for" + animationline.AffectedObject.name + " Track "+j+" " + animationline.AnimationTracks[j].name), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect ContentRect = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal();

                    //GUILayout.Box("" + animationline.AffectedObject.name, USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect addRect = FloatingRect;// GUILayoutUtility.GetLastRect();
                    Rect labelRect = addRect;
                    labelRect.x += 40;
                    labelRect.width = (lineHeight + 41);
                    //标签
                    GUI.Label(labelRect, "Track " + j);
                    //轨道名字
                    Rect nameRect = addRect;
                    nameRect.x += 40+ lineHeight + 40;
                    nameRect.width -= (lineHeight + 120);
                    //animationline.AnimationTracks[j].name=GUI.TextField(nameRect, animationline.AnimationTracks[j].name);
                    int select = SkillNames.ActionNames.Contains(animationline.AnimationTracks[j].name)? SkillNames.ActionNames.IndexOf(animationline.AnimationTracks[j].name) : 0;
                    select = EditorGUI.Popup(nameRect, select, SkillNames.ActionNames.ToArray());
                    animationline.AnimationTracks[j].name = SkillNames.ActionNames[select];

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    animationline.AnimationTracks[j].Enable = GUI.Toggle(enableRect, animationline.AnimationTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();
                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForAnimationTimeLine(animationline, animationline.AnimationTracks[j]);
                        contextMenu.ShowAsContext();
                    }

                    if (timelineClipRenderDataMap.ContainsKey(animationline.AnimationTracks[j]))
                    {
                        ///时间线背景 区域 只接受右键
                        // GUILayout.Box(new GUIContent("", "AnimationTimeline for" + animationline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[animationline.AnimationTracks[j]];
                        AnimationGUI(animationline, animationline.AnimationTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();

                    }
                }
                GUILayout.EndVertical();
            }
        }
        //移除一个动作片段
        private void RemoveAnimationClip(JClipRenderData clip)
        {
            if (clip.ClipData is JAnimationClipData)
            {
                JAnimationClipData anidata = (JAnimationClipData)clip.ClipData;
                if (timelineClipRenderDataMap.ContainsKey(anidata.Track))
                    if (timelineClipRenderDataMap[anidata.Track].Contains(clip))
                        timelineClipRenderDataMap[anidata.Track].Remove(clip);
                anidata.Track.RemoveClip(anidata);
            }
        }

        private void OnSingleAnimationSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JAnimationClipData)
            {
                Selection.activeObject = (JAnimationClipData)selectobj.ClipData;
            }
        }

        private void StartDraggingAnimationClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JAnimationClipData)
            {
                SourcePositions[clipData] = ((JAnimationClipData)trackdata).StartTime;
            }
        }

        private void ProcessDraggingAnimationClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = (ScriptableObject)clip.ClipData;
                if (selected is JAnimationClipData)
                {
                    JAnimationClipData animationClipData = selected as JAnimationClipData;

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
