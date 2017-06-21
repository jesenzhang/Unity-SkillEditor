
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{
    /// <summary>
    /// Our Base event class, when creating custom events, you need to inherit from this.
    /// </summary>
    [ExecuteInEditMode]
    [Serializable]
    abstract public class JEventBase : ScriptableObject
    {
        [SerializeField]
        private bool fireOnSkip = false;

        [SerializeField]
        private string eventName = "NotSet";
        public string EventName
        {
            get { return eventName; }
            set
            {
                eventName = value;
            }
        }

        /// <summary>
        /// The time at which this event will be triggered
        /// </summary>
        /// [SerializeField]
        private float startTime;
        public float StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
            }
        }

        [SerializeField]
        private JEventTrack track;
        public JEventTrack Track
        {
            get { return track; }
            set
            {
                track = value;
            }
        }
        public GameObject AffectedObject
        {
            get { return Track.TimeLine.AffectedObject ? Track.TimeLine.AffectedObject.gameObject : null; }
        }
        /// <summary>
        /// The duration of this Event, <0 is a <see cref="WellFired.JEventBase.IsFireAndForget"/>
        /// </summary>
        /// <value>The duration.</value>
        /// [SerializeField]
        private float duration = -1.0f;
        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public float EndTime
        {
            get { return startTime + duration; }
            private set {; }
        }

        /// <summary>
        /// The sequence that this event currently resides
        /// </summary>
        /// <value>The sequence.</value>
        public JSequencer Sequence
        {
            get { return track.TimeLine.Sequence; }
        }

        /// <summary>
        /// The timeline that this event currently resides
        /// </summary>
        /// <value>The timeline.</value>
        public JTimelineBase Timeline
        {
            get
            {
                return Track.TimeLine;
            }
        }

        /// <summary>
        /// one shot event
        /// </summary>
        public bool IsFireOneShotEvent
        {
            get { return Duration < 0.0f; }
        }

        /// <summary>
        /// If this is set to true, when calling the method <see cref="WellFired.JSequencer.SkipTimelineTo"/> this event will be fired.
        /// </summary>
        /// <value><c>true</c> if fire on skip; otherwise, <c>false</c>.</value>
        public bool FireOnSkip
        {
            get { return fireOnSkip; }
            set { fireOnSkip = value; }
        }

        public static bool IsClipNotRunning(float sequencerTime, JEventBase clipData)
        {
            return sequencerTime < clipData.StartTime;
        }

        public static bool IsClipRunning(float sequencerTime, JEventBase clipData)
        {
            if (clipData.IsFireOneShotEvent)
            {
                return sequencerTime == clipData.StartTime;
            }
            return sequencerTime > clipData.StartTime && sequencerTime < clipData.EndTime;
        }

        public static bool IsClipFinished(float sequencerTime, JEventBase clipData)
        {
            return sequencerTime >= clipData.EndTime;
        }

        /// <summary>
        /// Implement this method to define the behaviour when the uSequencer scrub head passes the start point of this event.
        /// </summary>
        public abstract void FireEvent();

        /// <summary>
        /// This is called on any event that isn't <see cref="WellFired.JEventBase.IsFireAndForget"/>.
        /// </summary>
        /// <param name="runningTime">Running time.</param>
        public abstract void ProcessEvent(float runningTime);

        /// <summary>
        /// Optionally implement this if you want custom functionality when the user pauses a sequence.
        /// </summary>
        public virtual void PauseEvent() {; }

        /// <summary>
        /// Optionally implement this if you want custom functionality when the user resumes a sequence.
        /// </summary>
        public virtual void ResumeEvent() {; }

        /// <summary>
        /// Optionally implement this if you want custom functionality when the user stops a sequence.
        /// </summary>
        public virtual void StopEvent() {; }

        /// <summary>
        /// Optionally implement this if you want custom functionality after an event has finished. I.E. when the <see cref="WellFired.JSequencer.RunningTime"/> has passed the end of this event.
        /// </summary>
        public virtual void EndEvent() {; }

        /// <summary>
        /// Optionally implement this if you want custom functionality when the <see cref="WellFired.JSequencer.RunningTime"/> has passed the start of this event and subsequently gone back to before the start of the event.
        /// </summary>
        public virtual void UndoEvent() {; }

        /// <summary>
        /// Optionally implement this if you want custom functionality when the user has manually set the <see cref="WellFired.JSequencer.RunningTime"/>.
        /// </summary>
        /// <param name="deltaTime">Delta time.</param>
        public virtual void ManuallySetTime(float deltaTime) {; }

        /// <summary>
        /// This method should be implemented if you have custom serialization requirements.
        /// 
        /// This method should probably return any objects that live in the scene that uSequencer will not automatically serialize for you.
        /// Currently, only the Affected Object will be serialized.
        /// The objects returned from this method will be passed to <see cref="WellFired.JEventBase.SetAdditionalObjects"/> when this event is constructed The order is maintained.
        /// </summary>
        /// <returns>The additional objects.</returns>
        public virtual Transform[] GetAdditionalObjects() { return new Transform[] { }; }

        /// <summary>
        /// This method should be implemented if you have custom serialization requirements.
        /// 
        /// When a sequence is created from a serialized sequence, this method will be called, passing all objects that have been optionally serialized by <see cref="WellFired.JEventBase.GetAdditionalObjects"/> The order is maintained.
        /// </summary>
        /// <param name="additionalObjects">Additional objects.</param>
        public virtual void SetAdditionalObjects(Transform[] additionalObjects) {; }

        /// <summary>
        /// This method should be implemented if you have custom serialization requirements.
        /// 
        /// Return true if you have custom serialization requirements and want <see cref="WellFired.JEventBase.SetAdditionalObjects"/> and <see cref="WellFired.JEventBase.GetAdditionalObjects"/> to be called.
        /// </summary>
        /// <returns><c>true</c> if this instance has valid additional objects; otherwise, <c>false</c>.</returns>
        public virtual bool HasValidAdditionalObjects() { return false; }

        /// <summary>
        /// If your event does anything special with Scriptable Objects, or you want to implement custom behaviour on duplication,
        /// this will be your entry point.
        /// </summary>
        public virtual void MakeUnique() {; }
    }
}