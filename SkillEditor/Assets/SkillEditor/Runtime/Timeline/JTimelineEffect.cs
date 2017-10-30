using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    public class JTimelineEffect : JTimelineBase
    {
        [SerializeField]
        private List<JEffectTrack> effectTracks = new List<JEffectTrack>();
        public List<JEffectTrack> EffectTracks
        {
            get { return effectTracks; }
            private set { effectTracks = value; }
        }
        [SerializeField]
        private List<JEffectClipData> allClips = new List<JEffectClipData>();

        private List<JEffectClipData> cachedRunningClips = new List<JEffectClipData>();

        float previousTime = 0.0f;

        public override TimeLineType LineType()
        {
            return TimeLineType.Effect;
        }

        public override void StartTimeline()
        {
            previousTime = 0.0f;
        }

        public override void StopTimeline()
        {
            previousTime = 0.0f;
            ResetEffect();
        }

        public override void EndTimeline()
        {
            base.EndTimeline();
            ResetEffect();
        }


        public override void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < effectTracks.Count; index++)
            {
                var track = effectTracks[index];
                if (track != null && track.Enable)
                {
                    for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                    {
                        var trackClip = track.TrackClips[trackClipIndex];
                        allClips.Add(trackClip);
                    }
                }
            }
            var totalDeltaTime = sequenceTime - previousTime;
            var absDeltaTime = Mathf.Abs(totalDeltaTime);
            var timelinePlayingInReverse = totalDeltaTime < 0.0f;
            var runningTime = JSequencer.SequenceUpdateRate;
            var runningTotalTime = previousTime + runningTime;
            if (timelinePlayingInReverse)
            {
                ResetEffect();
                previousTime = 0.0f;
                Process(sequenceTime, playbackRate);
            }
            else
            {
                while (absDeltaTime > 0.0f)
                {
                    cachedRunningClips.Clear();
                    for (int allClipIndex = 0; allClipIndex < allClips.Count; allClipIndex++)
                    {
                        var clip = allClips[allClipIndex];
                        if (!JEffectClipData.IsClipRunning(runningTotalTime, clip) && !clip.Looping)
                        {
                            if (clip.active)
                                clip.Reset();
                            continue;
                        }
                        cachedRunningClips.Add(clip);
                    }
                    cachedRunningClips.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

                    for (int runningClipIndex = 0; runningClipIndex < cachedRunningClips.Count; runningClipIndex++)
                    {
                        var clip = cachedRunningClips[runningClipIndex];
                        clip.Init();
                        clip.OnUpdate(runningTime);

                    }
                    absDeltaTime -= JSequencer.SequenceUpdateRate;
                    if (!Mathf.Approximately(absDeltaTime, Mathf.Epsilon) && absDeltaTime < JSequencer.SequenceUpdateRate)
                        runningTime = absDeltaTime;
                    runningTotalTime += runningTime;
                }
            }
            previousTime = sequenceTime;
        }

        public override void PauseTimeline()
        {

        }
        public void ResetEffect()
        {
            for (int index = 0; index < effectTracks.Count; index++)
            {
                var track = effectTracks[index];
                if (track != null && track.Enable)
                {
                    for (int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
                    {
                        var trackClip = track.TrackClips[trackClipIndex];
                        trackClip.Reset();
                    }
                }
            }
        }
        public JEffectTrack AddNewTrack()
        {
            var track = ScriptableObject.CreateInstance<JEffectTrack>();
            AddTrack(track);
            return track;
        }
        public void AddTrack(JEffectTrack particleTrack)
        {
            particleTrack.TimeLine = this;
            effectTracks.Add(particleTrack);
        }

        public void RemoveTrack(JEffectTrack particleTrack)
        {
            effectTracks.Remove(particleTrack);
        }

        public void SetTracks(List<JEffectTrack> particleTrack)
        {
            effectTracks = particleTrack;
        }
    }
}