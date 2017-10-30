using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {
        private void AddRenderDataForTrajectory(JTimelineBase timeline)
        {
            if (timeline is JTimelineTrajectory)
            {
                JTimelineTrajectory tline = (JTimelineTrajectory)timeline;
                for (int k = 0; k < tline.TrajectoryTracks.Count; k++)
                {
                    List<JClipRenderData> list = new List<JClipRenderData>();
                    JTrajectoryTrack track = tline.TrajectoryTracks[k];
                    for (int l = 0; l < track.TrackClips.Count; l++)
                    {
                        JTrajectoryClipData key = track.TrackClips[l];
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
  
       
        private void AddNewTrajectoryTrack(JTimelineBase line)
        {
            if (line is JTimelineTrajectory)
            {
                JTimelineTrajectory tline = (JTimelineTrajectory)line;
                var track = ScriptableObject.CreateInstance<JTrajectoryTrack>();
                tline.AddTrack(track); 
                AddRenderDataForTrajectory(tline);
            }
        }

        private void AddNewTrajectoryClip(JTrajectoryTrack track,float time)
        {
            var clipData = ScriptableObject.CreateInstance<JTrajectoryClipData>();
            JTimelineTrajectory line = (JTimelineTrajectory)track.TimeLine;
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            clipData.StateName = "";
            clipData.PlaybackDuration =1; 
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
        private GenericMenu MenuForTrajectoryTimeLine(JTimelineBase line, JTrajectoryTrack track)
        {
            GenericMenu contextMenu = new GenericMenu();
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * line.Sequence.Duration) / XScale;

            contextMenu.AddItem(new GUIContent("AddNewTrack"),
                    false, (obj) => AddNewTrajectoryTrack(((JTimelineBase)((object[])obj)[0])),
                    new object[] { line });

            contextMenu.AddItem(new GUIContent("AddClip"),
                     false, (obj) => AddNewTrajectoryClip(((JTrajectoryTrack)((object[])obj)[0]), ((float)((object[])obj)[1])),
                     new object[] { track, newTime });


            contextMenu.AddItem(new GUIContent("DeleteLine"),
                     false, (obj) => RemoveTrajectoryLine(((JTrajectoryTrack)((object[])obj)[0])),
                     new object[] { track });

            return contextMenu;
        }
        private void RemoveTrajectoryLine(JTrajectoryTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);

            }
            JTimelineTrajectory line = (JTimelineTrajectory)track.TimeLine;
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.TrajectoryTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.Timelines.Length == 0)
            {
                DestroyImmediate(contain.gameObject);
            }
        }
        private void RemoveTrajectoryClip(JClipRenderData clip)
        {
            if (clip.ClipData is JTrajectoryClipData)
            {
                JTrajectoryClipData anidata = (JTrajectoryClipData)clip.ClipData;
                if (timelineClipRenderDataMap.ContainsKey(anidata.Track))
                    if (timelineClipRenderDataMap[anidata.Track].Contains(clip))
                        timelineClipRenderDataMap[anidata.Track].Remove(clip);
                anidata.Track.RemoveClip(anidata);
            }
        }

        private void TrajectoryGUI(JTimelineBase timeline, JTrajectoryTrack track, JClipRenderData[] renderDataList)
        {
            if (timeline is JTimelineTrajectory)
            {
                JTimelineTrajectory trajectoryline = (JTimelineTrajectory)timeline;

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
                    JTrajectoryClipData trajectoryClipData = (JTrajectoryClipData)renderdata.ClipData;
                    JTrajectoryTrack linetrack = trajectoryClipData.Track;
                    if (linetrack != track)
                    {
                        continue;
                    }
                    var startX = ConvertTimeToXPos(trajectoryClipData.StartTime);
                    var endX = ConvertTimeToXPos(trajectoryClipData.StartTime + trajectoryClipData.PlaybackDuration);
                    var transitionX = ConvertTimeToXPos(trajectoryClipData.StartTime + trajectoryClipData.PlaybackDuration);
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

                    GUI.color = new Color(156 / 255.0f, 11 / 255.0f, 11 / 255.0f, 1);
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
                    renderdata.ClipData = trajectoryClipData;

                  
                     labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.

                    GUI.color = Color.black;
                    GUI.Label(labelRect, trajectoryClipData.FriendlyName);

                    GUI.color = Color.white;

                    if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                    {
                        hasBox = true;
                        contextMenu.AddItem(new GUIContent("DeleteClip"),
                               false, (obj) => RemoveTrajectoryClip(((JClipRenderData)((object[])obj)[0])),
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
                    Selection.activeGameObject = trajectoryline.gameObject;
                    EditorGUIUtility.PingObject(trajectoryline.gameObject);

                }
                if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    contextMenu = MenuForTrajectoryTimeLine(trajectoryline, track);

                }
                if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    UnityEngine.Event.current.Use();
                    contextMenu.ShowAsContext();
                }
            }
        }
        //侧边栏
        private void SideBarAndLineForTrajectory(JTimelineBase timeline)
        {
            if (timeline is JTimelineTrajectory)
            {
                GUILayout.BeginVertical();
                JTimelineTrajectory transformline = (JTimelineTrajectory)timeline;
                for (int j = 0; j < transformline.TrajectoryTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + transformline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "TrajectoryTimeline for" + transformline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
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
                    transformline.TrajectoryTracks[j].name = GUI.TextField(nameRect, transformline.TrajectoryTracks[j].name);

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    transformline.TrajectoryTracks[j].Enable = GUI.Toggle(enableRect, transformline.TrajectoryTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();

                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForTrajectoryTimeLine(transformline, transformline.TrajectoryTracks[j]);
                        contextMenu.ShowAsContext();
                    }

                    if (timelineClipRenderDataMap.ContainsKey(transformline.TrajectoryTracks[j]))
                    {
                        ///时间线背景 区域 只接受右键
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[transformline.TrajectoryTracks[j]];
                        TrajectoryGUI(transformline, transformline.TrajectoryTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();
                    }
                }
                GUILayout.EndVertical();
            }

        }
        //单独选中一个关键帧
        private void OnSingleTrajectorySelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JTrajectoryClipData)
            {
                Selection.activeObject = (JTrajectoryClipData)selectobj.ClipData;
            }
        }

        //开始拖动记录原始位置
        private void StartDraggingTrajectoryClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JTrajectoryClipData)
            {
                SourcePositions[clipData] = ((JTrajectoryClipData)trackdata).StartTime;
            }
        }
        //拖动
        private void ProcessDraggingTrajectoryClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = (ScriptableObject)clip.ClipData;
                if (selected is JTrajectoryClipData)
                {
                    JTrajectoryClipData animationClipData = selected as JTrajectoryClipData;

                    //float newTime = ((newPosition.x / DisplayArea.width) * AnimationTimeline.Sequence.Duration) / XScale;
                    // newTime = Mathf.Clamp(newTime, 0.0f, AnimationTimeline.Sequence.Duration);

                    float mousePosOnTimeline = ContentXToTime(FloatingWidth + mouseDelta.x);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration - animationClipData.PlaybackDuration);
                    animationClipData.StartTime = newTime;
                    animationClipData.effectunit.artEffect.beginTime = (int)(newTime * 1000f);
                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = animationClipData;
                    }
                }
            }
        }
 

    }
}
