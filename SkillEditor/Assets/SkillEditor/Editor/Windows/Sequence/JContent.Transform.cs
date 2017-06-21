using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {
        private void AddRenderDataForTransform(JTimelineBase timeline)
        {
            if (timeline is JTimelineTransform)
            {
                JTimelineTransform tline = (JTimelineTransform)timeline;
                for (int k = 0; k < tline.Tracks.Count; k++)
                {
                    List<JClipRenderData> list = new List<JClipRenderData>();
                    JTransformTrack track = tline.Tracks[k];
                    for (int l = 0; l < track.Keyframes.Count; l++)
                    {
                        JSplineKeyframe key = track.Keyframes[l];
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
        private void AddNewKeyFrameWithOutIndex(JTransformTrack track, float time)
        {
            int index = 1;
            for (int i = 1; i < track.Keyframes.Count; i++)
            {
                if (time <= track.Keyframes[i].StartTime)
                {
                    index = i;
                    break;
                }
            }
            AddNewKeyFrame(track, time, index);
        }
        //添加关键帧
        private void AddNewKeyFrame(JTransformTrack track, float time, int index)
        {
            var clipData = ScriptableObject.CreateInstance<JSplineKeyframe>();
            clipData.StartTime = time;
            clipData.Track = track;
            track.InsertKeyframe(clipData, index);
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
        /// <summary>
        /// 重置这条轨道的渲染数据和关键帧数据的关联
        /// </summary>
        /// <param name="track"></param>
        public void ResetTimeLineTrack(JTransformTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            List<JClipRenderData> cliplist = new List<JClipRenderData>();
            for (int i = 0; i < track.ObjectSpline.Nodes.Count; i++)
            {
                JSplineKeyframe frame = track.ObjectSpline.Nodes[i];
                var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                cachedData.ClipData = frame;
                frame.StartTime = track.StartTime + i * (track.EndTime - track.StartTime) / (track.ObjectSpline.Nodes.Count - 1);
                cliplist.Add(cachedData);
            }
            timelineClipRenderDataMap.Add(track, cliplist);
        }
        private void CloseThePath(JTransformTrack track)
        {
            track.ObjectSpline.IsClosed = !track.ObjectSpline.IsClosed;
            track.Build();
            track.SetKeyframes(track.ObjectSpline.Nodes);
            ResetTimeLineTrack(track);
        }
        private void AddNewTransFormTrack(JTimelineBase line)
        {
            if (line is JTimelineTransform)
            {
                JTimelineTransform tline = (JTimelineTransform)line;
                var track = ScriptableObject.CreateInstance<JTransformTrack>();
                tline.AddTrack(track);
                tline.Build();
                AddRenderDataForTransform(tline);
            }
        }
        private GenericMenu MenuForTransformTimeLine(JTimelineBase line, JTransformTrack track)
        {
            GenericMenu contextMenu = new GenericMenu();
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * line.Sequence.Duration) / XScale;

            contextMenu.AddItem(new GUIContent("AddNewTrack"),
                    false, (obj) => AddNewTransFormTrack(((JTimelineBase)((object[])obj)[0])),
                    new object[] { line });

            contextMenu.AddItem(new GUIContent("AddKeyFrame"),
                     false, (obj) => AddNewKeyFrameWithOutIndex(((JTransformTrack)((object[])obj)[0]), ((float)((object[])obj)[1])),
                     new object[] { track, newTime });

            string closestate = track.ObjectSpline.IsClosed ? "Open The Path" : "Close The Path";
            contextMenu.AddItem(new GUIContent(closestate),
                    false, (obj) => CloseThePath(((JTransformTrack)((object[])obj)[0])),
                    new object[] { track });

            contextMenu.AddItem(new GUIContent("DeleteTransformLine"),
                     false, (obj) => RemoveTransformLine(((JTransformTrack)((object[])obj)[0])),
                     new object[] { track });

            return contextMenu;
        }
        private void RemoveTransformLine(JTransformTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            JTimelineTransform line = (JTimelineTransform)track.TimeLine;
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.Tracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.transform.childCount == 0)
            {
                DestroyImmediate(contain.gameObject);
            }

        }
        private void RemoveKeyFrameWithKeyframe(int index)
        {
            if ((Selection.objects.Length == 1) && (Selection.objects[0] is JSplineKeyframe))
            {
                JSplineKeyframe keyframe = (JSplineKeyframe)Selection.objects[0];
                JTransformTrack track = keyframe.Track;
                if (timelineClipRenderDataMap.ContainsKey(track))
                {
                    int remvindex = -1;
                    for (int i = 0; i < timelineClipRenderDataMap[track].Count; i++)
                    {
                        JSplineKeyframe key = (JSplineKeyframe)timelineClipRenderDataMap[track][i].ClipData;
                        if (key == keyframe)
                        {
                            remvindex = i;
                            break;
                        }
                    }
                    timelineClipRenderDataMap[track].RemoveAt(remvindex);
                }
                track.RemoveKeyframe(keyframe);
            }

        }

        private void RemoveKeyFrame(JClipRenderData renderdata)
        {
            var keyframe = renderdata.ClipData;
            if (keyframe is JSplineKeyframe)
            {
                JSplineKeyframe key = (JSplineKeyframe)keyframe;
                JTransformTrack track = key.Track;
                if (timelineClipRenderDataMap.ContainsKey(track))
                    if (timelineClipRenderDataMap[track].Contains(renderdata))
                        timelineClipRenderDataMap[track].Remove(renderdata);
                track.RemoveKeyframe(key);
            }
        }

        private void TransformGUI(JTimelineBase timeline, JTransformTrack track, JClipRenderData[] renderDataList)
        {
            GenericMenu contextMenu = new GenericMenu();
            ///event 右键点击
            bool isContext = UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 1 && UnityEngine.Event.current.clickCount == 1;
            bool isChoose = UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0 && UnityEngine.Event.current.clickCount == 1;
            bool isDouble = UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0 && UnityEngine.Event.current.clickCount == 2;
            bool hasBox = false;

            Rect DisplayAreaTemp = DisplayArea;
            DisplayAreaTemp.x = 0;
            DisplayAreaTemp.y = 0;

            var linestartX = ConvertTimeToXPos(track.StartTime);
            var lineendX = ConvertTimeToXPos(track.EndTime);

            Rect linerenderRect = new Rect(linestartX, DisplayArea.y, lineendX - linestartX, DisplayArea.height);
            Rect linerenderRectTemp = linerenderRect;
            linerenderRectTemp.x -= DisplayArea.x;
            linerenderRectTemp.y = 0;

            GUI.color = new Color(85 / 255.0f, 47 / 255.0f, 176 / 255.0f, 1f);
            GUI.Box(linerenderRectTemp, "", USEditorUtility.NormalWhiteOutLineBG);
            GUI.color = Color.white;

            for (int j = 0; j < renderDataList.Length; j++)
            {
                JClipRenderData renderdata = renderDataList[j];
                JSplineKeyframe key = (JSplineKeyframe)renderdata.ClipData;
                var startX = ConvertTimeToXPos(key.StartTime);
                var handleWidth = 5.0f;

                Rect renderRect = new Rect(startX - handleWidth, DisplayArea.y, handleWidth * 2, DisplayArea.height);
                Rect renderRectTemp = renderRect;
                renderRectTemp.x -= DisplayArea.x;
                renderRectTemp.y = 0;

                Rect labelRect = new Rect();

                GUI.color = new Color(255 / 255.0f, 122 / 255.0f, 105 / 255.0f, 1);
                if (SelectedObjects.Contains(renderdata))
                {
                    GUI.color = Color.yellow;
                }
                GUI.Box(renderRectTemp, "", USEditorUtility.NormalWhiteOutLineBG);
                labelRect = renderRectTemp;
                renderdata.renderRect = renderRect;
                renderdata.labelRect = renderRect;
                renderdata.renderPosition = new Vector2(startX - handleWidth, DisplayArea.y);
                renderdata.ClipData = key;
                GUI.color = Color.black;
                GUI.Label(labelRect, "" + track.Keyframes.IndexOf(key), USEditorUtility.USeqSkin.label);
                GUI.color = Color.white;

                if (isContext && renderRectTemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    hasBox = true;
                    contextMenu.AddItem(new GUIContent("DeleteKeyFrame"),
                            false, (obj) => RemoveKeyFrame(((JClipRenderData)((object[])obj)[0])),
                            new object[] { renderdata });
                }
                if (hasBox && isContext && renderRectTemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    UnityEngine.Event.current.Use();
                    contextMenu.ShowAsContext();
                }
            }

            if (!hasBox && isChoose && linerenderRectTemp.Contains(UnityEngine.Event.current.mousePosition) && (UnityEngine.Event.current.control || UnityEngine.Event.current.command))
            {
                //代码选中hierarchy中的对象 显示inspector 按住Ctrl or command
                //GameObject go = GameObject.Find(tline.gameObject.name);
                Selection.activeObject = track;
                EditorGUIUtility.PingObject(track);
            }

            if (!hasBox && isContext && linerenderRectTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                contextMenu = MenuForTransformTimeLine(timeline, track);
            }
            if (!hasBox && isContext && linerenderRectTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                UnityEngine.Event.current.Use();
                contextMenu.ShowAsContext();
            }
            if (!hasBox && isDouble && linerenderRectTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                int index = 1;
                float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * timeline.Sequence.Duration) / XScale;
                for (int i = 1; i < track.Keyframes.Count; i++)
                {
                    if (newTime <= track.Keyframes[i].StartTime)
                    {
                        index = i;
                        break;
                    }
                }
                AddNewKeyFrame(track, newTime, index);
            }
        }
        //侧边栏
        private void SideBarAndLineForTranform(JTimelineBase timeline)
        {
            if (timeline is JTimelineTransform)
            {
                GUILayout.BeginVertical();
                JTimelineTransform transformline = (JTimelineTransform)timeline;
                for (int j = 0; j < transformline.Tracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + transformline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "TransformTimeline for" + transformline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
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
                    transformline.Tracks[j].name=GUI.TextField(nameRect, transformline.Tracks[j].name);

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    transformline.Tracks[j].Enable = GUI.Toggle(enableRect, transformline.Tracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();

                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForTransformTimeLine(transformline, transformline.Tracks[j]);
                        contextMenu.ShowAsContext();
                    }

                    if (timelineClipRenderDataMap.ContainsKey(transformline.Tracks[j]))
                    {
                        ///时间线背景 区域 只接受右键
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[transformline.Tracks[j]];
                        TransformGUI(transformline, transformline.Tracks[j], renderDataList.ToArray());
                        GUI.EndGroup();
                    }
                }
                GUILayout.EndVertical();
            }

        }
        //单独选中一个关键帧
        private void OnSingleTransformSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JSplineKeyframe)
            {
                Selection.activeObject = (JSplineKeyframe)selectobj.ClipData;
            }
        }

        //开始拖动记录原始位置
        private void StartDraggingTransformClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JSplineKeyframe)
            {
                SourcePositions[clipData] = ((JSplineKeyframe)trackdata).StartTime;
            }
        }
        //拖动
        private void ProcessDraggingTransformClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = clip.ClipData;
                if (selected is JSplineKeyframe)
                {
                    JSplineKeyframe Keyframe = selected as JSplineKeyframe;
                    float mousePosOnTimeline = ContentXToTime(mouseDelta.x + FloatingWidth);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration);
                    int index = Keyframe.Track.Keyframes.IndexOf(Keyframe);
                    if (index == 0)
                    {
                        Keyframe.StartTime = newTime;
                        Keyframe.Track.StartTime = newTime;
                    }
                    else if (index == Keyframe.Track.Keyframes.Count - 1)
                    {
                        Keyframe.StartTime = newTime;
                        Keyframe.Track.EndTime = newTime;
                    }
                    else
                    {
                        newTime = Mathf.Clamp(newTime, Keyframe.Track.Keyframes[index - 1].StartTime, Keyframe.Track.Keyframes[index + 1].StartTime);
                        Keyframe.StartTime = newTime;
                    }
                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = Keyframe;
                    }
                }
            }
        }

        int SelectedNodeIndex = -1;
        private int GetNearestNodeForMousePosition(JTransformTrack ObjectPathTimeline, Vector3 mousePos)
        {
            var bestDistance = float.MaxValue;
            var index = -1;
            var distance = float.MaxValue;
            for (var i = ObjectPathTimeline.Keyframes.Count - 1; i >= 0; i--)
            {
                var nodeToGui = HandleUtility.WorldToGUIPoint(ObjectPathTimeline.Keyframes[i].Position);
                distance = Vector2.Distance(nodeToGui, mousePos);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    index = i;
                }
            }

            if (bestDistance < 26)
                return index;

            return -1;
        }
        public void OnTransformSceneGUI(JTimelineBase timeline)
        {
            if (timeline == null)
                return;
            if (timeline.LineType() == TimeLineType.Transform)
            {
                JTimelineTransform ObjectPathTimeline = (JTimelineTransform)timeline;
                if (ObjectPathTimeline.Tracks == null)
                    return;

                for (int i = 0; i < ObjectPathTimeline.Tracks.Count; i++)
                {
                    JTransformTrack tTrack = ObjectPathTimeline.Tracks[i];
                    if (SelectedNodeIndex >= 0)
                    {
                        if (UnityEngine.Event.current.isKey && (UnityEngine.Event.current.keyCode == KeyCode.Delete || UnityEngine.Event.current.keyCode == KeyCode.Backspace))
                        {
                            UnityEngine.Event.current.Use();
                            RemoveKeyFrameWithKeyframe(SelectedNodeIndex);
                            SelectedNodeIndex = -1;
                        }
                    }

                    if (UnityEngine.Event.current.type == EventType.mouseDown)
                    {
                        var nearestIndex = GetNearestNodeForMousePosition(tTrack, UnityEngine.Event.current.mousePosition);

                        if (nearestIndex != -1)
                        {
                            SelectedNodeIndex = nearestIndex;
                            if (UnityEngine.Event.current.clickCount == 1 && UnityEngine.Event.current.button == 0)
                            {
                                ResetSelection();
                                Selection.activeObject = tTrack.Keyframes[SelectedNodeIndex];
                                OnSelectedObjects(new List<Object> { tTrack.Keyframes[SelectedNodeIndex] });
                            }
                            if (UnityEngine.Event.current.clickCount > 1)
                            {
                                var cameraTransform = UnityEditor.SceneView.currentDrawingSceneView.camera.transform;
                                var keyframe = tTrack.Keyframes[SelectedNodeIndex];
                                int next = SelectedNodeIndex == (tTrack.Keyframes.Count - 1) ? (SelectedNodeIndex - 1) : (SelectedNodeIndex + 1);

                                var nextKeyframe = tTrack.Keyframes[next];
                                float newtime = (keyframe.StartTime + nextKeyframe.StartTime) * 0.5f;

                                AddNewKeyFrame(tTrack, newtime, SelectedNodeIndex);

                                GUI.changed = true;
                            }
                        }
                    }

                    if (tTrack.Keyframes.Count >= 2)
                    {
                        if (Vector3.Distance(tTrack.Keyframes[0].Position, tTrack.Keyframes[tTrack.Keyframes.Count - 1].Position) == 0)
                        {
                            Handles.Label(tTrack.Keyframes[0].Position, "Start and End");
                        }
                        else
                        {
                            Handles.Label(tTrack.Keyframes[0].Position, "Start");
                            Handles.Label(tTrack.Keyframes[tTrack.Keyframes.Count - 1].Position, "End");
                        }
                    }
                    for (int nodeIndex = 0; nodeIndex < tTrack.Keyframes.Count; nodeIndex++)
                    {
                        var node = tTrack.Keyframes[nodeIndex];

                        if (node && nodeIndex > 0 && nodeIndex < tTrack.Keyframes.Count - 1)
                        {
                            float handleSize = HandlesUtility.GetHandleSize(node.Position);
                            Handles.Label(node.Position + new Vector3(0.25f * handleSize, 0.0f * handleSize, 0.0f * handleSize), nodeIndex.ToString());
                        }
                        var existingKeyframe = tTrack.Keyframes[nodeIndex];
                        Quaternion oldrotation = Quaternion.Euler(existingKeyframe.Rotation);
                        Vector3 oldpos = existingKeyframe.Position;
                        Vector3 oldtang = existingKeyframe.Tangent;
                        Vector3 oldnormal = existingKeyframe.Normal;
                        Vector3 newPosition = HandlesUtility.PositionHandle(existingKeyframe.Position, oldrotation);
                        Vector3 newtangent = HandlesUtility.TangentHandle(existingKeyframe.Position + existingKeyframe.Tangent) - existingKeyframe.Position;
                        if (oldpos != newPosition || oldtang != newtangent)
                        {
                            tTrack.AlterKeyframe(newPosition, oldrotation.eulerAngles, newtangent, oldnormal, nodeIndex);
                            EditorUtility.SetDirty(tTrack);
                        }
                    }

                }

            }



        }

    }
}
