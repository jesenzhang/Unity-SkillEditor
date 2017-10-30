
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{

    [Serializable]
    public class JEffectTrack : ScriptableObject
    {

        [SerializeField]
        private List<JEffectClipData> trackClipList = new List<JEffectClipData>();


        public List<JEffectClipData> TrackClips
        {
            get { return trackClipList; }
            private set { trackClipList = value; }
        }
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

        public void AddClip(JEffectClipData clipData)
        {
            if (trackClipList.Contains(clipData))
                throw new Exception("Track already contains Clip");
            clipData.Track = this;
            trackClipList.Add(clipData);
        }

        public void RemoveClip(JEffectClipData clipData)
        {
            if (!trackClipList.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            trackClipList.Remove(clipData);
        }

        private void SortClips()
        {
            trackClipList = trackClipList.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JEffectClipData> particleData)
        {
            trackClipList = particleData;
        }

    }
}
