
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {
        private void AddRenderDataForEvent(JTimelineBase timeline)
        {
            if (timeline is JTimelineEvent)
            {
                JTimelineEvent tline = (JTimelineEvent)timeline;
                for (int k = 0; k < tline.EventTracks.Count; k++)
                {
                    List<JClipRenderData> list = new List<JClipRenderData>();
                    JEventTrack track = tline.EventTracks[k];
                    for (int l = 0; l < track.EventClips.Count; l++)
                    {
                        JEventBase key = track.EventClips[l];
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

        //添加关键帧
        private void AddNewTrackEvent(JEventTrack track, float time, Type eventType)
        {
            var clipData = ScriptableObject.CreateInstance(eventType) as JEventBase;
            clipData.StartTime = time;
            clipData.Track = track;
            clipData.EventName = eventType.Name;
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

        private void AddNewEventTrack(JTimelineBase line)
        {
            if (line is JTimelineEvent)
            {
                JTimelineEvent tline = (JTimelineEvent)line;
                var track = ScriptableObject.CreateInstance<JEventTrack>();
                tline.AddTrack(track);
                AddRenderDataForEvent(tline);
            }
        }

        private GenericMenu MenuForEventTimeLine(JTimelineBase line, JEventTrack track)
        {
            GenericMenu contextMenu = new GenericMenu();
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * line.Sequence.Duration) / XScale;

            contextMenu.AddItem(new GUIContent("AddNewEventTimeline"),
                false, (obj) => AddNewEventTrack(((JTimelineBase)((object[])obj)[0])),
                new object[] { line });

            string baseAdd = "Add Event/";

            var baseType = typeof(JEventBase);
            var types = USEditorUtility.GetAllSubTypes(baseType).Where(type => type.IsSubclassOf(baseType));
            foreach (Type type in types)
            {
                string fullAdd = baseAdd;
                var customAttributes = type.GetCustomAttributes(true).Where(attr => attr is JEventAttribute).Cast<JEventAttribute>();
                foreach (JEventAttribute customAttribute in customAttributes)
                    fullAdd += customAttribute.EventPath;
                contextMenu.AddItem(new GUIContent(fullAdd),
                    false, (obj) => AddNewTrackEvent(((JEventTrack)((object[])obj)[0]), ((float)((object[])obj)[1]), ((Type)((object[])obj)[2])),
                    new object[] { track, newTime, type });
            }


            contextMenu.AddItem(new GUIContent("DeleteEventLine"),
                false, (obj) => RemoveEventTrack(((JEventTrack)((object[])obj)[0])),
                new object[] { track });

            return contextMenu;
        }
        private void RemoveEventTrack(JEventTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            JTimelineEvent line = (JTimelineEvent)track.TimeLine;
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.EventTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.transform.childCount == 0)
            {
                DestroyImmediate(contain.gameObject);
            }

        }


        private void RemoveEvent(JClipRenderData renderdata)
        {
            var keyframe = renderdata.ClipData;
            if (keyframe is JEventBase)
            {
                JEventBase key = (JEventBase)keyframe;
                JEventTrack track = key.Track;
                if (timelineClipRenderDataMap.ContainsKey(track))
                    if (timelineClipRenderDataMap[track].Contains(renderdata))
                        timelineClipRenderDataMap[track].Remove(renderdata);
                track.RemoveClip(key);
            }
        }

        private void EventGUI(JTimelineEvent Eventine, JEventTrack linetrack, JClipRenderData[] renderDataList)
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
                JEventBase eventClipData = (JEventBase)renderdata.ClipData;
                JEventTrack track = eventClipData.Track;
                if (linetrack != track)
                {
                    continue;
                }
                var startX = ConvertTimeToXPos(eventClipData.StartTime);

                var endX = ConvertTimeToXPos(eventClipData.StartTime + eventClipData.Duration);
                if (eventClipData.IsFireOneShotEvent)
                {
                    endX = ConvertTimeToXPos(eventClipData.StartTime + 1f);
                }
                var handleWidth = 2.0f;

                float posy = DisplayArea.y;
                float height = DisplayArea.height;

                Rect renderRect = new Rect(startX, posy, endX - startX, height);
                Rect leftHandle = new Rect(startX, posy, handleWidth * 2.0f, height);
                Rect rightHandle = new Rect(endX - (handleWidth * 2.0f), posy, handleWidth * 2.0f, height);
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


                GUI.color = new Color(144 / 255.0f, 234 / 255.0f, 251 / 255.0f, 1);
                if (SelectedObjects.Contains(renderdata))
                {
                    GUI.color = Color.yellow;
                }
                GUI.Box(renderRecttemp, "", USEditorUtility.NormalWhiteOutLineBG);

                GUI.Box(leftHandletemp, "");
                GUI.Box(rightHandletemp, "");

                labelRect = renderRecttemp;
                renderdata.renderRect = renderRect;
                renderdata.labelRect = renderRect;
                renderdata.renderPosition = new Vector2(startX, height);
                renderdata.leftHandle = leftHandle;
                renderdata.rightHandle = rightHandle;
                renderdata.ClipData = eventClipData;

                labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.

                GUI.color = Color.black;
                GUI.Label(labelRect, eventClipData.EventName);

                GUI.color = Color.white;

                if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    hasBox = true;
                    contextMenu.AddItem(new GUIContent("DeleteEvent"),
                        false, (obj) => RemoveEvent(((JClipRenderData)((object[])obj)[0])),
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
                Selection.activeGameObject = Eventine.gameObject;
                EditorGUIUtility.PingObject(Eventine.gameObject);
            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                contextMenu = MenuForEventTimeLine(Eventine, linetrack);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                UnityEngine.Event.current.Use();
                contextMenu.ShowAsContext();
            }
        }
        //侧边栏
        private void SideBarAndLineForEvent(JTimelineBase timeline)
        {
            if (timeline is JTimelineEvent)
            {
                GUILayout.BeginVertical();

                JTimelineEvent eventline = (JTimelineEvent)timeline;
                for (int j = 0; j < eventline.EventTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + eventline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "EventTimeline for" + eventline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect ContentRect = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal();

                    // GUILayout.Box("" + eventline.AffectedObject.name, USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect addRect = FloatingRect;// GUILayoutUtility.GetLastRect();
                    Rect labelRect = addRect;
                    labelRect.x += 40;
                    labelRect.width -= (lineHeight + 41);
                    GUI.Label(labelRect, "Track " + j);
                    //轨道名字
                    Rect nameRect = addRect;
                    nameRect.x += 40 + lineHeight + 40;
                    nameRect.width -= (lineHeight + 120);
                    eventline.EventTracks[j].name=GUI.TextField(nameRect, eventline.EventTracks[j].name);

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    eventline.EventTracks[j].Enable = GUI.Toggle(enableRect, eventline.EventTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();

                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForEventTimeLine(eventline, eventline.EventTracks[j]);
                        contextMenu.ShowAsContext();
                    }
                    if (timelineClipRenderDataMap.ContainsKey(eventline.EventTracks[j]))
                    {
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[eventline.EventTracks[j]];
                        EventGUI(eventline, eventline.EventTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();
                    }
                }
                GUILayout.EndVertical();
            }
        }
        //单独选中一个关键帧
        private void OnSingleEventSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JEventBase)
            {
                Selection.activeObject = (JEventBase)selectobj.ClipData;
            }
        }

        //开始拖动记录原始位置
        private void StartDraggingEventClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JEventBase)
            {
                SourcePositions[clipData] = ((JEventBase)trackdata).StartTime;
            }
        }
        //拖动
        private void ProcessDraggingEventClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                ScriptableObject selected = clip.ClipData;
                if (selected is JEventBase)
                {
                    JEventBase aevent = selected as JEventBase;
                    float mousePosOnTimeline = ContentXToTime(mouseDelta.x + FloatingWidth);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration);
                    aevent.StartTime = newTime;

                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = aevent;
                    }
                }
            }
        }


    }
}


