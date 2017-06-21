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
        private void AddRenderDataForCamera(JTimelineBase timeline)
        {
            if (timeline is JTimelineCamera)
            {
                JTimelineCamera Cameraline = (JTimelineCamera)timeline;
                List<JClipRenderData> list = new List<JClipRenderData>();
                for (int k = 0; k < Cameraline.CameraTracks.Count; k++)
                {
                    JCameraTrack track = Cameraline.CameraTracks[k];
                    for (int l = 0; l < track.TrackClips.Count; l++)
                    {
                        JCameraClipData CameraClipData = track.TrackClips[l];
                        var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                        cachedData.ClipData = CameraClipData;
                        list.Add(cachedData);
                    }
                    if (!timelineClipRenderDataMap.ContainsKey(track))
                        timelineClipRenderDataMap.Add(track, list);
                }
            }
        }
        //添加动作片段
        private void AddNewCameraState(JTimelineCamera line, JCameraTrack track, float time, string stateName)
        {
            var clipData = ScriptableObject.CreateInstance<JCameraClipData>();
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            clipData.ActionName = stateName;
            clipData.Action = new SkillCameraAction();
            clipData.Action.action = (SkillCameraAction.CameraAction)Enum.Parse(typeof(SkillCameraAction.CameraAction), stateName);
            clipData.PlaybackDuration =0.5f;
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
        private void RemoveCameraTimeline(JTimelineCamera line, JCameraTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.CameraTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.transform.childCount == 0)
            {
                DestroyImmediate(contain.gameObject);
            }
        }
        private void AddNewCameraTrack(JTimelineBase line)
        {
            if (line is JTimelineCamera)
            {
                JTimelineCamera tline = (JTimelineCamera)line;
                var track = ScriptableObject.CreateInstance<JCameraTrack>();
                tline.AddTrack(track);
                AddRenderDataForCamera(tline);
            }
        }

        //绘制动作片段UI
        private void CameraGUI(JTimelineCamera Cameraline, JCameraTrack linetrack, JClipRenderData[] renderDataList)
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
                JCameraClipData CameraClipData = (JCameraClipData)renderdata.ClipData;
                JCameraTrack track = CameraClipData.Track;
                if (linetrack != track)
                {
                    continue;
                }
                var startX = ConvertTimeToXPos(CameraClipData.StartTime);
                var endX = ConvertTimeToXPos(CameraClipData.StartTime + CameraClipData.PlaybackDuration);
                var handleWidth = 2.0f;

                Rect renderRect = new Rect(startX, DisplayArea.y, endX - startX, DisplayArea.height);
                Rect leftHandle = new Rect(startX, DisplayArea.y, handleWidth * 2.0f, DisplayArea.height);
                Rect rightHandle = new Rect(endX - (handleWidth * 2.0f), DisplayArea.y, handleWidth * 2.0f, DisplayArea.height);
                Rect labelRect = new Rect();

                Rect renderRecttemp = renderRect;
                renderRecttemp.x -= DisplayArea.x;
                renderRecttemp.y = 0;
                Rect leftHandletemp = leftHandle;
                leftHandletemp.y = 0;
                leftHandletemp.x -= DisplayArea.x;
                Rect rightHandletemp = rightHandle;
                rightHandletemp.x -= DisplayArea.x;
                rightHandletemp.y = 0;

                GUI.color = new Color(70 / 255.0f, 147 / 255.0f, 236 / 255.0f, 1);
                if (SelectedObjects.Contains(renderdata))
                {
                    GUI.color = Color.yellow;
                }

                GUI.Box(renderRecttemp, "", USEditorUtility.NormalWhiteOutLineBG);
               
                GUI.Box(leftHandletemp, "");
                GUI.Box(rightHandletemp, "");

                labelRect = renderRecttemp;
                labelRect.width = DisplayArea.width;

                renderdata.renderRect = renderRect;
                renderdata.labelRect = renderRect;
                renderdata.renderPosition = new Vector2(startX, DisplayArea.y);
    
                renderdata.leftHandle = leftHandle;
                renderdata.rightHandle = rightHandle;
                renderdata.ClipData = CameraClipData;
                labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.

                GUI.color = Color.black;
                GUI.Label(labelRect, CameraClipData.FriendlyName);

                GUI.color = Color.white;

                if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    hasBox = true;
                    contextMenu.AddItem(new GUIContent("DeleteClip"),
                           false, (obj) => RemoveCameraClip(((JClipRenderData)((object[])obj)[0])),
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
                //GameObject go = GameObject.Find(Cameraline.gameObject.name);
                Selection.activeGameObject = Cameraline.gameObject;
                EditorGUIUtility.PingObject(Cameraline.gameObject);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                contextMenu = MenuForCameraTimeLine(Cameraline, linetrack);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                UnityEngine.Event.current.Use();
                contextMenu.ShowAsContext();
            }
        }

        //时间线右键点击的菜单
        private GenericMenu MenuForCameraTimeLine(JTimelineCamera Cameraline, JCameraTrack linetrack)
        {
            GenericMenu contextMenu = new GenericMenu();
            string[] allStates = Enum.GetNames(typeof(SkillCameraAction.CameraAction)); 

            contextMenu.AddItem(new GUIContent("AddNewTrack"),
            false, (obj) => AddNewCameraTrack(((JTimelineBase)((object[])obj)[0])),
            new object[] { Cameraline });

  
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * Cameraline.Sequence.Duration) / XScale;
            foreach (var state in allStates)
                contextMenu.AddItem(
                    new GUIContent("AddNewAction/"+state),
                    false,
                    (obj) => AddNewCameraState(((JTimelineCamera)((object[])obj)[0]), ((JCameraTrack)((object[])obj)[1]), ((float)((object[])obj)[2]), ((string)((object[])obj)[3])),
                    new object[] { Cameraline, linetrack, newTime, state }
                );
            //删除时间线
            contextMenu.AddItem(new GUIContent("DeleteCameraLine"),
                    false, (obj) => RemoveCameraTimeline(((JTimelineCamera)((object[])obj)[0]), ((JCameraTrack)((object[])obj)[1])),
                    new object[] { Cameraline, linetrack });
            return contextMenu;
        }
        //时间线的侧边栏绘制
        private void SideBarAndLineForCamera(JTimelineBase timeline)
        {
            if (timeline is JTimelineCamera)
            {
                GUILayout.BeginVertical();
                JTimelineCamera Cameraline = (JTimelineCamera)timeline;
                for (int j = 0; j < Cameraline.CameraTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + Cameraline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "CameraTimeline for" + Cameraline.AffectedObject.name + " Track " + j + " " + Cameraline.CameraTracks[j].name), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect ContentRect = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal();

                    //GUILayout.Box("" + Cameraline.AffectedObject.name, USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect addRect = FloatingRect;// GUILayoutUtility.GetLastRect();
                    Rect labelRect = addRect;
                    labelRect.x += 40;
                    labelRect.width = (lineHeight + 41);
                    //标签
                    GUI.Label(labelRect, "Track " + j);
                    //轨道名字
                    Rect nameRect = addRect;
                    nameRect.x += 40 + lineHeight + 40;
                    nameRect.width -= (lineHeight + 120);
                    Cameraline.CameraTracks[j].name = GUI.TextField(nameRect, Cameraline.CameraTracks[j].name);

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    Cameraline.CameraTracks[j].Enable = GUI.Toggle(enableRect, Cameraline.CameraTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();
                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForCameraTimeLine(Cameraline, Cameraline.CameraTracks[j]);
                        contextMenu.ShowAsContext();
                    }

                    if (timelineClipRenderDataMap.ContainsKey(Cameraline.CameraTracks[j]))
                    {
                        ///时间线背景 区域 只接受右键
                        // GUILayout.Box(new GUIContent("", "CameraTimeline for" + Cameraline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[Cameraline.CameraTracks[j]];
                        CameraGUI(Cameraline, Cameraline.CameraTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();

                    }
                }
                GUILayout.EndVertical();
            }
        }
        //移除一个动作片段
        private void RemoveCameraClip(JClipRenderData clip)
        {
            if (clip.ClipData is JCameraClipData)
            {
                JCameraClipData anidata = (JCameraClipData)clip.ClipData;
                if (timelineClipRenderDataMap.ContainsKey(anidata.Track))
                    if (timelineClipRenderDataMap[anidata.Track].Contains(clip))
                        timelineClipRenderDataMap[anidata.Track].Remove(clip);
                anidata.Track.RemoveClip(anidata);
            }
        }

        private void OnSingleCameraSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JCameraClipData)
            {
                Selection.activeObject = (JCameraClipData)selectobj.ClipData;
            }
        }

        private void StartDraggingCameraClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JCameraClipData)
            {
                SourcePositions[clipData] = ((JCameraClipData)trackdata).StartTime;
            }
        }

        private void ProcessDraggingCameraClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = (ScriptableObject)clip.ClipData;
                if (selected is JCameraClipData)
                {
                    JCameraClipData CameraClipData = selected as JCameraClipData;

                    //float newTime = ((newPosition.x / DisplayArea.width) * CameraTimeline.Sequence.Duration) / XScale;
                    // newTime = Mathf.Clamp(newTime, 0.0f, CameraTimeline.Sequence.Duration);

                    float mousePosOnTimeline = ContentXToTime(FloatingWidth + mouseDelta.x);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration - CameraClipData.PlaybackDuration);
                    CameraClipData.StartTime = newTime;
                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = CameraClipData;
                    }
                }
            }
        }
    }
}
