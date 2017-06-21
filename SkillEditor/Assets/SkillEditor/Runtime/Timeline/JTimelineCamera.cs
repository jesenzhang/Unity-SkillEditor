using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CySkillEditor
{
    public class JTimelineCamera : JTimelineBase
    {
        [SerializeField]
        private List<JCameraTrack> cameraTracks = new List<JCameraTrack>();
        public List<JCameraTrack> CameraTracks
        {
            get { return cameraTracks; }
            private set { cameraTracks = value; }
        }
        [SerializeField]
        private List<JCameraClipData> allClips = new List<JCameraClipData>();

        private List<JCameraClipData> cachedRunningClips = new List<JCameraClipData>();

        float previousTime = 0.0f;

        public override TimeLineType LineType()
        {
            return TimeLineType.CameraAction;
        }
        public override void StartTimeline()
        {
            previousTime = 0.0f;
        }

        public override void StopTimeline()
        {
            previousTime = 0.0f;
            ResetCamera();
        }

        public override void EndTimeline()
        {
            base.EndTimeline();
            ResetCamera();
        }


        public override void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < CameraTracks.Count; index++)
            {
                var track = CameraTracks[index];
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
                ResetCamera();
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
                        if (!JCameraClipData.IsClipRunning(runningTotalTime, clip) && !clip.Looping)
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
                        clip.OnUpdate(runningTotalTime);
                      
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
        public void ResetCamera()
        {
            for (int index = 0; index < CameraTracks.Count; index++)
            {
                var track = CameraTracks[index];
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
        public JCameraTrack AddNewTrack()
        {
            var track = ScriptableObject.CreateInstance<JCameraTrack>();
            AddTrack(track);
            return track;
        }
        public void AddTrack(JCameraTrack particleTrack)
        {
            particleTrack.TimeLine = this;
            CameraTracks.Add(particleTrack);
        }

        public void RemoveTrack(JCameraTrack particleTrack)
        {
            CameraTracks.Remove(particleTrack);
        }

        public void SetTracks(List<JCameraTrack> particleTrack)
        {
            CameraTracks = particleTrack;
        }
    }
}