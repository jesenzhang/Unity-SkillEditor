
using UnityEngine;
using System;
using System.Collections;

namespace CySkillEditor
{
    public enum TimeLineType
    {
        Animation = 0,
        Effect = 1,
        Sound = 2,
        Transform = 3,
        Event = 4,
        Trajectory = 5,
        CameraAction = 6
    }
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class JTimelineBase : MonoBehaviour
    {
        /// <summary>
        /// 时间线作用的对象
        /// </summary>
        public Transform AffectedObject
        {
            get
            {
                return TimelineContainer.AffectedObject;
            }
        }

        /// <summary>
        /// 时间线的容器对象
        /// </summary>
        private JTimelineContainer timelineContainer;
        public JTimelineContainer TimelineContainer
        {
            get
            {
                if (timelineContainer)
                    return timelineContainer;

                timelineContainer = transform.parent.GetComponent<JTimelineContainer>();
                return timelineContainer;
            }
        }

        /// <summary>
        ///  时间序列对象
        /// </summary>
        public JSequencer Sequence
        {
            get
            {
                return TimelineContainer.Sequence;
            }
        }
        [SerializeField]
        private bool shouldRenderGizmos = true;
        /// <summary>
        /// Should this timeline render it's gizmos.
        /// </summary>
        public bool ShouldRenderGizmos
        {
            get { return shouldRenderGizmos; }
            set { shouldRenderGizmos = value; }
        }

        public virtual TimeLineType LineType()
        {
            throw new NotImplementedException();
        }
        public static int Comparer(JTimelineBase a, JTimelineBase b)
        {
            return (a.LineType().CompareTo(b.LineType()));
        }
        /// <summary>
        ///  Stops.
        /// </summary>
        public virtual void StopTimeline() {; }

        /// <summary>
        ///  Starts.
        /// </summary>
        public virtual void StartTimeline() {; }

        /// <summary>
        /// Ends.
        /// </summary>
        public virtual void EndTimeline() {; }

        /// <summary>
        /// Pauses.
        /// </summary>
        public virtual void PauseTimeline() {; }

        /// <summary>
        ///  Resumed.
        /// </summary>
        public virtual void ResumeTimeline() {; }

        /// <summary>
        /// Skips.
        /// </summary>
        public virtual void SkipTimelineTo(float time) {; }

        /// <summary>
        ///  processes. This should happen during regular playback and when scrubbing
        /// </summary>
        public virtual void Process(float sequencerTime, float playbackRate) {; }

        /// <summary>
        ///  has it's time manually set.
        /// </summary>
        public virtual void ManuallySetTime(float sequencerTime) {; }

        /// <summary>
        /// Implement custom logic here if you need to do something special when uSequencer finds a missing AffectedObject in the scene (prefab instantiaton, with late binding).
        /// </summary>

        public virtual void LateBindAffectedObjectInScene(Transform newAffectedObject) {; }

        public virtual string GetJson()
        {
            throw new NotImplementedException();
        }

        public virtual void ResetCachedData()
        {
            timelineContainer = null;
        }
    }
}