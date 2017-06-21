using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
namespace CySkillEditor
{
    public partial class JContent : ScriptableObject
    {

        private void AddRenderDataForParticle(JTimelineBase timeline)
        {
            if (timeline is JTimelineParticle)
            {
                JTimelineParticle particleline = (JTimelineParticle)timeline;
                List<JClipRenderData> list = new List<JClipRenderData>();
                for (int i = 0; i < particleline.ParticleTracks.Count; i++)
                {
                    JParticleTrack track = particleline.ParticleTracks[i];
                    if (track)
                    {
                        for (int j = 0; j < track.TrackClips.Count; j++)
                        {
                            JParticleClipData ClipData = track.TrackClips[j];
                            var cachedData = ScriptableObject.CreateInstance<JClipRenderData>();
                            cachedData.ClipData = ClipData;
                            list.Add(cachedData);
                        }
                        if (!timelineClipRenderDataMap.ContainsKey(track))
                            timelineClipRenderDataMap.Add(track, list);
                    }
                }
            }
        }
        private void AddNewParticleTrack(JTimelineBase line)
        {
            if (line is JTimelineParticle)
            {
                JTimelineParticle tline = (JTimelineParticle)line;
                var track = ScriptableObject.CreateInstance<JParticleTrack>();
                tline.AddTrack(track);
                AddRenderDataForParticle(tline);
            }
        }
        private void AddNewParticleState(JTimelineParticle line, JParticleTrack track, float time, string stateName)
        {
           // var clipData = ScriptableObject.CreateInstance<JParticleClipData>();
            JParticleClipData clipData = new JParticleClipData();
            clipData.TargetObject = line.AffectedObject.gameObject;
            clipData.StartTime = time;
            clipData.ParticleName = stateName;
            clipData.EffectDuration = line.GetEffectDuration(stateName);  // ParticleSystemUtility.GetParticleDuration(stateName, line.AffectedObject.gameObject);
            clipData.PlaybackDuration = clipData.EffectDuration;
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
        private void RemoveParticleTimeline(JTimelineParticle line, JParticleTrack track)
        {
            if (timelineClipRenderDataMap.ContainsKey(track))
            {
                timelineClipRenderDataMap.Remove(track);
            }
            line.RemoveTrack(track);
            JTimelineContainer contain = line.TimelineContainer;
            if (line.ParticleTracks.Count == 0)
                DestroyImmediate(line.gameObject);
            //删除的是最后一个 删除掉container
            if (contain.Timelines.Length == 0)
            {
                DestroyImmediate(contain.gameObject);
            }
        }

        private void ParticleGUI(JTimelineParticle Particleline, JParticleTrack linetrack, JClipRenderData[] renderDataList)
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
                JParticleClipData particleClipData = (JParticleClipData)renderdata.ClipData;
                JParticleTrack track = particleClipData.Track;
                if (linetrack != track)
                {
                    continue;
                }
                var startX = ConvertTimeToXPos(particleClipData.StartTime);
                var endX = ConvertTimeToXPos(particleClipData.StartTime + particleClipData.PlaybackDuration);
                var transitionX = ConvertTimeToXPos(particleClipData.StartTime + particleClipData.TransitionDuration);
                var handleWidth = 2.0f;

                float posy = DisplayArea.y;
                float height = DisplayArea.height;

                Rect renderRect = new Rect(startX, posy, endX - startX, height);
                Rect transitionRect = new Rect(startX, posy, transitionX - startX, height);
                Rect leftHandle = new Rect(startX, posy, handleWidth * 2.0f, height);
                Rect rightHandle = new Rect(endX - (handleWidth * 2.0f), posy, handleWidth * 2.0f, height);
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


                GUI.color = new Color(163 / 255.0f, 237 / 255.0f, 116 / 255.0f, 1);
                if (SelectedObjects.Contains(renderdata))
                {
                    GUI.color = Color.yellow;
                }
                GUI.Box(renderRecttemp, "", USEditorUtility.NormalWhiteOutLineBG);

                if (particleClipData.CrossFade)
                    GUI.Box(transitionRecttemp, "");

                GUI.Box(leftHandletemp, "");
                GUI.Box(rightHandletemp, "");

                labelRect = renderRecttemp;
                labelRect.width = DisplayArea.width;

                renderdata.renderRect = renderRect;
                renderdata.labelRect = renderRect;
                renderdata.renderPosition = new Vector2(startX, height);
                renderdata.transitionRect = transitionRect;
                renderdata.leftHandle = leftHandle;
                renderdata.rightHandle = rightHandle;
                renderdata.ClipData = particleClipData;

                if (particleClipData.CrossFade)
                    labelRect.x = labelRect.x + (transitionX - startX);
                else
                    labelRect.x += 4.0f; // Nudge this along a bit so it isn't flush with the side.

                GUI.color = Color.black;
                GUI.Label(labelRect, particleClipData.FriendlyName);

                GUI.color = Color.white;

                if (isContext && renderRecttemp.Contains(UnityEngine.Event.current.mousePosition))
                {
                    hasBox = true;
                    contextMenu.AddItem(new GUIContent("DeleteClip"),
                           false, (obj) => RemoveParticleClip(((JClipRenderData)((object[])obj)[0])),
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
                Selection.activeGameObject = Particleline.gameObject;
                EditorGUIUtility.PingObject(Particleline.gameObject);
            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                contextMenu = MenuForParticleTimeLine(Particleline, linetrack);

            }
            if (!hasBox && isContext && DisplayAreaTemp.Contains(UnityEngine.Event.current.mousePosition))
            {
                UnityEngine.Event.current.Use();
                contextMenu.ShowAsContext();
            }
        }

        private GenericMenu MenuForParticleTimeLine(JTimelineParticle Particleline, JParticleTrack linetrack)
        {
            GenericMenu contextMenu = new GenericMenu();
            List<string> allStates = new List<string>();

            contextMenu.AddItem(new GUIContent("AddNewTrack"),
            false, (obj) => AddNewParticleTrack(((JTimelineBase)((object[])obj)[0])),
             new object[] { Particleline });

            foreach (var particlesys in Particleline.ParticleDict.Keys)
            {
                string statename = particlesys;
                allStates.Add(statename);
            }
            float newTime = (((UnityEngine.Event.current.mousePosition.x + XScroll) / DisplayArea.width) * Particleline.Sequence.Duration) / XScale;
            string Addstate = "Add Particle/{0}";
            foreach (var state in allStates)
                contextMenu.AddItem(
                    new GUIContent(string.Format(Addstate, state)),
                    false,
                    (obj) => AddNewParticleState(((JTimelineParticle)((object[])obj)[0]), ((JParticleTrack)((object[])obj)[1]), ((float)((object[])obj)[2]), ((string)((object[])obj)[3])),
                    new object[] { Particleline, linetrack, newTime, state }
                );
            //删除时间线
            contextMenu.AddItem(new GUIContent("DeleteParticleLine"),
                    false, (obj) => RemoveParticleTimeline(((JTimelineParticle)((object[])obj)[0]), ((JParticleTrack)((object[])obj)[1])),
                    new object[] { Particleline, linetrack });
            return contextMenu;
        }
        private void SideBarAndLineForParticle(JTimelineBase timeline)
        {
            if (timeline == null)
                return;
            if (timeline is JTimelineParticle)
            {
                GUILayout.BeginVertical();

                JTimelineParticle particleline = (JTimelineParticle)timeline;
                for (int j = 0; j < particleline.ParticleTracks.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(new GUIContent("" + particleline.AffectedObject.name, ""), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.MaxWidth(FloatingWidth));
                    Rect FloatingRect = GUILayoutUtility.GetLastRect();
                    GUILayout.Box(new GUIContent("", "ParticleTimeline for" + particleline.AffectedObject.name + "Track " + j), USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect ContentRect = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal();

                    // GUILayout.Box("" + particleline.AffectedObject.name, USEditorUtility.USeqSkin.GetStyle("TimelinePaneBackground"), GUILayout.Height(lineHeight), GUILayout.ExpandWidth(true));
                    Rect addRect = FloatingRect;// GUILayoutUtility.GetLastRect();
                    Rect labelRect = addRect;
                    labelRect.x += 40;
                    labelRect.width -= (lineHeight + 41);
                    GUI.Label(labelRect, "Track " + j);

                    //轨道名字
                    Rect nameRect = addRect;
                    nameRect.x += 40 + lineHeight + 40;
                    nameRect.width -= (lineHeight + 120);
                    particleline.ParticleTracks[j].name=GUI.TextField(nameRect, particleline.ParticleTracks[j].name);

                    Rect enableRect = addRect;
                    enableRect.x = addRect.x + addRect.width - 2 * lineHeight - 2.0f; ;
                    enableRect.width = lineHeight;
                    //enable开关
                    particleline.ParticleTracks[j].Enable = GUI.Toggle(enableRect, particleline.ParticleTracks[j].Enable, new GUIContent("", USEditorUtility.EditButton, "Enable The Timeline"));

                    addRect.x = addRect.x + addRect.width - lineHeight - 1.0f;
                    addRect.width = lineHeight;
                    GenericMenu contextMenu = new GenericMenu();

                    if (GUI.Button(addRect, new GUIContent("", USEditorUtility.EditButton, "Options for this Timeline"), USEditorUtility.ToolbarButtonSmall))
                    {
                        contextMenu = MenuForParticleTimeLine(particleline, particleline.ParticleTracks[j]);
                        contextMenu.ShowAsContext();
                    }
                    if (timelineClipRenderDataMap.ContainsKey(particleline.ParticleTracks[j]))
                    {
                        DisplayArea = ContentRect;// GUILayoutUtility.GetLastRect();
                        GUI.BeginGroup(DisplayArea);
                        List<JClipRenderData> renderDataList = timelineClipRenderDataMap[particleline.ParticleTracks[j]];
                        ParticleGUI(particleline, particleline.ParticleTracks[j], renderDataList.ToArray());
                        GUI.EndGroup();
                    }
                }
                GUILayout.EndVertical();
            }
        }
        private void RemoveParticleClip(JClipRenderData clip)
        {
            var clipdata = clip.ClipData;
            if (clipdata is JParticleClipData)
            {
                JParticleClipData anidata = (JParticleClipData)clipdata;
                if (timelineClipRenderDataMap.ContainsKey(anidata.Track))
                    if (timelineClipRenderDataMap[anidata.Track].Contains(clip))
                        timelineClipRenderDataMap[anidata.Track].Remove(clip);
                anidata.Track.RemoveClip(anidata);
            }

        }

        private void OnSingleParticleSelected(JClipRenderData selectobj)
        {
            if (selectobj.ClipData is JParticleClipData)
            {
                Selection.activeObject = (JParticleClipData)selectobj.ClipData;
            }
        }

        private void StartDraggingParticleClip(JClipRenderData clipData)
        {
            var trackdata = clipData.ClipData;
            if (trackdata is JParticleClipData)
            {
                SourcePositions[clipData] = ((JParticleClipData)trackdata).StartTime;
            }
        }

        private void ProcessDraggingParticleClip(JClipRenderData clip, Vector2 mouseDelta)
        {
            if (SourcePositions.ContainsKey(clip))
            {
                UnityEngine.Object selected = clip.ClipData;
                if (selected is JParticleClipData)
                {
                    JParticleClipData particleClipData = selected as JParticleClipData;
                    float mousePosOnTimeline = ContentXToTime(mouseDelta.x + FloatingWidth);
                    float newTime = SourcePositions[clip] + mousePosOnTimeline;
                    newTime = Mathf.Clamp(newTime, 0.0f, CurrentSequence.Duration - particleClipData.PlaybackDuration);
                    particleClipData.StartTime = newTime;
                    if (SelectedObjects.Count == 1)
                    {
                        Selection.activeObject = particleClipData;
                    }
                }
            }
        }
    }
}
