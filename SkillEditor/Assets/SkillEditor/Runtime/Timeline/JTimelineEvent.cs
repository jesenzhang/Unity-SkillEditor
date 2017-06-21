using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CySkillEditor
{
    public class JEventHideDurationAttribute : Attribute
    {
        public JEventHideDurationAttribute()
        {

        }
    }

    public class JEventAttribute : Attribute
    {
        public JEventAttribute(string myEventPath)
        {
            eventPath = myEventPath;
        }

        private string eventPath;
        public string EventPath
        {
            get { return eventPath; }
        }
    }

    public class JFriendlyNameAttribute : Attribute
    {
        public JFriendlyNameAttribute(string myFriendlyName)
        {
            friendlyName = myFriendlyName;
        }

        private string friendlyName;
        public string FriendlyName
        {
            get { return friendlyName; }
        }
    }

    public class JTimelineEvent : JTimelineBase
    {
        private float elapsedTime = 0.0f;

        #region Properties

        [SerializeField]
        private List<JEventTrack> eventTracks = new List<JEventTrack>();
        public List<JEventTrack> EventTracks
        {
            get { return eventTracks; }
            private set { eventTracks = value; }
        }


        #endregion

        public override TimeLineType LineType()
        {
            return TimeLineType.Event;
        }


        public override void StopTimeline()
        {
            float prevElapsedTime = elapsedTime;
            elapsedTime = 0.0f;
            for (int i = 0; i < EventTracks.Count; i++)
            {
                JEventTrack track = EventTracks[i];
                JEventBase[] events = track.EventClips.ToArray();
                events = events.Where(e => e.StartTime <= prevElapsedTime).ToArray();
                for (int j = events.Length - 1; j >= 0; j--)
                    events[j].StopEvent();
            }

        }

        public override void PauseTimeline()
        {
            for (int i = 0; i < EventTracks.Count; i++)
            {
                JEventTrack track = EventTracks[i];
                JEventBase[] events = track.EventClips.ToArray();
                foreach (JEventBase eventBase in events)
                {
                    eventBase.PauseEvent();
                }
            }
        }

        public override void ResumeTimeline()
        {
            for (int i = 0; i < EventTracks.Count; i++)
            {
                JEventTrack track = EventTracks[i];
                JEventBase[] events = track.EventClips.ToArray();
                foreach (JEventBase eventBase in events)
                {
                    if (!eventBase.IsFireOneShotEvent && Sequence.RunningTime > eventBase.StartTime && Sequence.RunningTime < (eventBase.StartTime + eventBase.Duration))
                        eventBase.ResumeEvent();
                }
            }
        }


        public override void SkipTimelineTo(float time)
        {
            for (int i = 0; i < EventTracks.Count; i++)
            {
                JEventTrack track = EventTracks[i];
                JEventBase[] events = track.EventClips.ToArray();
                float prevElapsedTime = elapsedTime;
                elapsedTime = time;

                foreach (JEventBase baseEvent in events)
                {
                    if (!baseEvent)
                        continue;

                    bool shouldSkipEvent = !baseEvent.IsFireOneShotEvent || !baseEvent.FireOnSkip;
                    if (shouldSkipEvent)
                        continue;

                    if ((prevElapsedTime < baseEvent.StartTime || prevElapsedTime <= 0.0f) && time > baseEvent.StartTime)
                    {
                        if (Sequence.IsPlaying && baseEvent.AffectedObject)
                            baseEvent.FireEvent();
                    }
                }
            }
        }

        public override void Process(float sequencerTime, float playbackRate)
        {
            float prevElapsedTime = elapsedTime;
            elapsedTime = sequencerTime;
            for (int i = 0; i < EventTracks.Count; i++)
            {
                JEventTrack track = EventTracks[i];
                JEventBase[] events = track.EventClips.ToArray();

                if (prevElapsedTime < elapsedTime)
                    Array.Sort(events, delegate (JEventBase a, JEventBase b) { return a.StartTime.CompareTo(b.StartTime); });
                else
                    Array.Sort(events, delegate (JEventBase a, JEventBase b) { return b.StartTime.CompareTo(a.StartTime); });

                foreach (JEventBase baseEvent in events)
                {
                    if (playbackRate >= 0.0f)
                        FireEvent(baseEvent, prevElapsedTime, elapsedTime);
                    else
                        FireEventReverse(baseEvent, prevElapsedTime, elapsedTime);

                    FireEventCommon(baseEvent, sequencerTime, prevElapsedTime, elapsedTime);
                }
            }



        }

        private void FireEvent(JEventBase baseEvent, float prevElapsedTime, float elapsedTime)
        {
            if ((prevElapsedTime < baseEvent.StartTime || prevElapsedTime <= 0.0f) && elapsedTime >= baseEvent.StartTime)
            {
                Debug.Log("" + prevElapsedTime + " " + baseEvent.StartTime + " elapsedTime " + elapsedTime);
                //if (baseEvent.AffectedObject)
                baseEvent.FireEvent();
            }
        }

        private void FireEventReverse(JEventBase baseEvent, float prevElapsedTime, float elapsedTime)
        {

        }

        private void FireEventCommon(JEventBase baseEvent, float sequencerTime, float prevElapsedTime, float elapsedTime)
        {
            if (elapsedTime > baseEvent.StartTime && elapsedTime <= baseEvent.StartTime + baseEvent.Duration)
            {
                float deltaTime = sequencerTime - baseEvent.StartTime;
                if (baseEvent.AffectedObject)
                    baseEvent.ProcessEvent(deltaTime);
            }

            if (prevElapsedTime < baseEvent.StartTime + baseEvent.Duration && elapsedTime >= baseEvent.StartTime + baseEvent.Duration)
            {
                if (baseEvent.AffectedObject)
                {
                    float deltaTime = sequencerTime - baseEvent.StartTime;
                    baseEvent.ProcessEvent(deltaTime);
                    baseEvent.EndEvent();
                }
            }

            if (prevElapsedTime >= baseEvent.StartTime && elapsedTime < baseEvent.StartTime)
            {
                if (baseEvent.AffectedObject)
                    baseEvent.UndoEvent();
            }
        }

        public override void ManuallySetTime(float sequencerTime)
        {
            foreach (Transform child in transform)
            {
                JEventBase baseEvent = child.GetComponent<JEventBase>();
                if (!baseEvent)
                    continue;

                float deltaTime = sequencerTime - baseEvent.StartTime;
                if (baseEvent.AffectedObject)
                    baseEvent.ManuallySetTime(deltaTime);
            }
        }

        public void AddNewEvent(JEventBase sequencerEvent)
        {

            SortEvents();
        }

        public void RemoveAndDestroyEvent(Transform sequencerEvent)
        {
            if (!sequencerEvent.IsChildOf(transform))
            {
                Debug.LogError("We are trying to delete an Event that doesn't belong to this Timeline, from USTimelineEvent::RemoveAndDestroyEvent");
                return;
            }

            GameObject.DestroyImmediate(sequencerEvent.gameObject);
        }

        public void SortEvents()
        {
            Debug.LogWarning("Implement a sorting algorithm here!");
        }

        public override void ResetCachedData()
        {
            base.ResetCachedData();
        }

        public void AddTrack(JEventTrack ttrack)
        {
            ttrack.TimeLine = this;
            EventTracks.Add(ttrack);
        }

        public void RemoveTrack(JEventTrack tTrack)
        {
            EventTracks.Remove(tTrack);
        }

        public void SetTracks(List<JEventTrack> tTrack)
        {
            EventTracks = tTrack;
        }
    }

}