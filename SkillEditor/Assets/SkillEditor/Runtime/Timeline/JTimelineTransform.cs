using UnityEngine;
using System;
using System.Collections.Generic;

namespace CySkillEditor
{
    public class JTimelineTransform : JTimelineBase
    {
        [SerializeField]
        private List<JTransformTrack> tracks = new List<JTransformTrack>();
        public List<JTransformTrack> Tracks
        {
            get { return tracks; }
            private set { tracks = value; }
        }

        public override TimeLineType LineType()
        {
            return TimeLineType.Transform;
        }

        private void OnEnable()
        {
            Debug.Log("Onenable");
            Build();
        }
        public void Build()
        {
            foreach (var track in Tracks)
            {
                track.Build();
            }
        }

        public override void StartTimeline()
        {
            foreach (var track in Tracks)
            {
                track.StartTimeline();
            }
        }
        public override void StopTimeline()
        {
            foreach (var track in Tracks)
            {
                track.StopTimeline();
            }
        }
        public override void SkipTimelineTo(float time)
        {
            foreach (var track in Tracks)
            {
                track.SkipTimelineTo(time);
            }
        }
        public override void Process(float sequencerTime, float playbackRate)
        {
            foreach (var track in Tracks)
            {
                if (track.Enable)
                {
                    track.Process(sequencerTime, playbackRate);
                }
            }

        }
        private void OnDrawGizmos()
        {
            if (!ShouldRenderGizmos)
                return;

            foreach (var track in Tracks)
            {
                track.OnDrawGizmos();
            }

        }

        public override void PauseTimeline()
        {
        }

        public override void EndTimeline()
        {
            base.EndTimeline();
        }

        public void AddTrack(JTransformTrack ttrack)
        {
            ttrack.TimeLine = this;
            Tracks.Add(ttrack);
        }

        public void RemoveTrack(JTransformTrack tTrack)
        {
            Tracks.Remove(tTrack);
        }

        public void SetTracks(List<JTransformTrack> tTrack)
        {
            Tracks = tTrack;
        }
    }
}