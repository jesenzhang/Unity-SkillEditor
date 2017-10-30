using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CySkillEditor
{
    [Serializable]
    public class JTrajectoryTrack : ScriptableObject
    {
        [SerializeField]
        private List<JTrajectoryClipData> trackClipList = new List<JTrajectoryClipData>();
        [SerializeField]
        private JTimelineBase timeLine;
        public JTimelineBase TimeLine
        {
            get { return timeLine; }
            set { timeLine = value; }
        }

        [SerializeField]
        private bool enable = true;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField]
        private int layer;

        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        public List<JTrajectoryClipData> TrackClips
        {
            get { return trackClipList; }
            private set { trackClipList = value; }
        }


        public void AddClipWithName(string effectName, float startTime, float PlayBackduration,JSkillUnit unit, SkillEffectUnit effectUnit)
        {
            var clipData = ScriptableObject.CreateInstance<JTrajectoryClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.StateName = effectName;
            clipData.effectunit = effectUnit;
            clipData.skillunit = unit;
            clipData.StartTime = startTime;
            clipData.PlaybackDuration = PlayBackduration;
            clipData.Track = this;
            AddClip(clipData);
        }

        public void AddClip(JTrajectoryClipData clipData)
        {
            if (trackClipList.Contains(clipData))
                throw new Exception("Track already contains Clip");
            clipData.Track = this;
            trackClipList.Add(clipData);
        }

        public void RemoveClip(JTrajectoryClipData clipData)
        {
            if (!trackClipList.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            trackClipList.Remove(clipData);
        }

        private void SortClips()
        {
            trackClipList = trackClipList.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JTrajectoryClipData> JTrajectoryData)
        {
            trackClipList = JTrajectoryData;
        }
    }
}