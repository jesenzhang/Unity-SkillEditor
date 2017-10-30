using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CySkillEditor
{
    [Serializable]
    public class JAnimationTrack : ScriptableObject
    {
        [SerializeField]
        private List<JAnimationClipData> trackClipList = new List<JAnimationClipData>();
        [SerializeField]
        private JTimelineBase limeLine;
        public JTimelineBase TimeLine
        {
            get { return limeLine; }
            set { limeLine = value; }
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

        public List<JAnimationClipData> TrackClips
        {
            get { return trackClipList; }
            private set { trackClipList = value; }
        }

        public void AddClipWithState(string stateName, float startTime)
        {
            var clipData = ScriptableObject.CreateInstance<JAnimationClipData>();
            clipData.TargetObject = TimeLine.AffectedObject.gameObject;
            clipData.StartTime = startTime;
            clipData.StateName = stateName;
            clipData.StateDuration = MecanimAnimationUtility.GetStateDuration(stateName, TimeLine.AffectedObject.gameObject);
            clipData.PlaybackDuration = clipData.StateDuration;
            clipData.Track = this;
            AddClip(clipData);
        }

        public void AddClip(JAnimationClipData clipData)
        {
            if (trackClipList.Contains(clipData))
                throw new Exception("Track already contains Clip");
            clipData.Track = this;
            trackClipList.Add(clipData);
        }

        public void RemoveClip(JAnimationClipData clipData)
        {
            if (!trackClipList.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            trackClipList.Remove(clipData);
        }

        private void SortClips()
        {
            trackClipList = trackClipList.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JAnimationClipData> JAnimationClipData)
        {
            trackClipList = JAnimationClipData;
        }
    }
}