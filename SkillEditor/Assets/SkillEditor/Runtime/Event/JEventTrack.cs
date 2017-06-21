using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{
    [Serializable]
    public class JEventTrack : ScriptableObject
    {

        [SerializeField]
        private List<JEventBase> eventClips = new List<JEventBase>();


        public List<JEventBase> EventClips
        {
            get { return eventClips; }
            private set { eventClips = value; }
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
        public void AddClip(JEventBase clipData)
        {
            if (eventClips.Contains(clipData))
                throw new Exception("Track already contains Clip");

            eventClips.Add(clipData);
        }

        public void RemoveClip(JEventBase clipData)
        {
            if (!eventClips.Contains(clipData))
                throw new Exception("Track doesn't contains Clip");

            eventClips.Remove(clipData);
        }

        private void SortClips()
        {
            eventClips = eventClips.OrderBy(trackClip => trackClip.StartTime).ToList();
        }

        public void SetClipData(List<JEventBase> eventlist)
        {
            eventClips = eventlist;
        }

    }
}