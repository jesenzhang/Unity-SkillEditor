using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CySkillEditor
{
    [Serializable]
    public class JSoundTrack : ScriptableObject
    {

        [SerializeField]
        private List<JSoundClipData> trackClipList = new List<JSoundClipData>();
        public List<JSoundClipData> TrackClips
        {
            get { return trackClipList; }
            private set { trackClipList = value; }
        }
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

        public void AddClip(JSoundClipData clipData)
        {
            if (trackClipList.Contains(clipData))
                throw new Exception("Track already contains Clip");
            clipData.Track = this;
            trackClipList.Add(clipData);
        }

        public void RemoveClip(JSoundClipData clipData)
        {
            if (!trackClipList.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            trackClipList.Remove(clipData);
        }

        private void SortClips()
        {
            trackClipList = trackClipList.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JSoundClipData> soundData)
        {
            trackClipList = soundData;
        }

    }
}
