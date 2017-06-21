using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace CySkillEditor
{
    public enum SplineOrientationMode
    {
        None = 0,
        ManualOrientation = 1,
        LookAtTransform = 2,
        LookAhead = 3,
    }

    [Serializable]
    public class JTransformTrack : ScriptableObject
    {
        [SerializeField]
        private JTimelineBase timeLine;
        public JTimelineBase TimeLine
        {
            get { return timeLine; }
            set { timeLine = value; }
        }

        /// <summary>
        ///时间曲线 
        /// </summary>
        [SerializeField]
        private AnimationCurve curve;

        public AnimationCurve Curve
        {
            get
            {
                if (curve == null || curve.length == 0)
                {
                    Keyframe[] ks = new Keyframe[2];
                    ks[0] = new Keyframe(0, 0);
                    ks[0].inTangent = 1;
                    ks[0].outTangent = 1;
                    ks[1] = new Keyframe(1, 1);
                    ks[1].inTangent = 1;
                    ks[1].outTangent = 1;
                    curve = new AnimationCurve(ks);
                }
                return curve;
            }
            set { curve = value; }
        }

        [SerializeField]
        private bool enable = true;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField]
        private SplineOrientationMode splineOrientationMode = SplineOrientationMode.LookAhead;
        public SplineOrientationMode SplineOrientationMode
        {
            get { return splineOrientationMode; }
            set { splineOrientationMode = value; }
        }

        [SerializeField]
        private Transform lookAtTarget;
        public Transform LookAtTarget
        {
            get { return lookAtTarget; }
            set { lookAtTarget = value; lookAtTargetPath = lookAtTarget.GetFullHierarchyPath(); }
        }

        [SerializeField]
        private string lookAtTargetPath = "";

        [SerializeField]
        private Vector3 sourcePosition;
        public Vector3 SourcePosition
        {
            get { return sourcePosition; }
            set { sourcePosition = value; }
        }

        [SerializeField]
        private Quaternion sourceRotation;
        public Quaternion SourceRotation
        {
            get { return sourceRotation; }
            set { sourceRotation = value; }
        }

        [SerializeField]
        private float startTime;
        public float StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        [SerializeField]
        private float endTime;
        public float EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        [SerializeField]
        private List<JSplineKeyframe> keyframes = new List<JSplineKeyframe>();
        public List<JSplineKeyframe> Keyframes
        {
            get { return keyframes; }
            private set
            {
                keyframes = value;
                BuildCurveFromKeyframes();
            }
        }

        [SerializeField]
        private JSpline objectSpline;
        public JSpline ObjectSpline
        {
            get { return objectSpline; }
            set { objectSpline = value; }
        }

        [SerializeField]
        private int seed = 0;
        public int Seed
        {
            private get { return seed; }
            set { seed = value; }
        }

        [SerializeField]
        public JSplineType SplineType
        {
            get { return ObjectSpline.SplineType; }
            set
            {
                ObjectSpline.SplineType = value;

            }
        }


        /// <summary>
        /// Gets the first node.
        /// </summary>
        /// <value>The first node.</value>
        public JSplineKeyframe FirstNode
        {
            get { return ObjectSpline.Nodes[0]; }
            private set {; }
        }

        /// <summary>
        /// Gets the last node.
        /// </summary>
        /// <value>The last node.</value>
        public JSplineKeyframe LastNode
        {
            get { return ObjectSpline.Nodes[ObjectSpline.Nodes.Count - 1]; }
            private set {; }
        }
        /// <summary>
        /// Gets or sets the color of the path in the scene view.
        /// </summary>
        /// <value>The color of the path.</value>
        public Color PathColor
        {
            get { return ObjectSpline.SplineColor; }
            set { ObjectSpline.SplineColor = value; }
        }
        /// <summary>
        /// Gets or sets the display resolution. Increasing this value will make the curve appear more smooth. This is entirely visual and has
        /// no affect on runtime performance.
        /// </summary>
        /// <value>The display resolution.</value>
        public float DisplayResolution
        {
            get { return ObjectSpline.DisplayResolution; }
            set { ObjectSpline.DisplayResolution = value; }
        }

        public void SetKeyframes(List<JSplineKeyframe> newkeyframes)
        {
            keyframes = new List<JSplineKeyframe>();
            keyframes = newkeyframes;
        }

        public void Build()
        {
            Debug.Log("Build");
            if (keyframes == null || (keyframes != null && keyframes.Count < 2))
                CreateEmpty();
            else
                BuildCurveFromKeyframes();
        }
        public void AddKeyframe(JSplineKeyframe keyframe)
        {
            keyframes.Add(keyframe);
            BuildCurveFromKeyframes();
        }

        public void InsertKeyframe(JSplineKeyframe keyframe, int index)
        {
            int before = Mathf.Max(index - 1, 0);
            int after = index == 0 ? 1 : Mathf.Min(index, keyframes.Count - 1);
            keyframe.Position = Vector3.Lerp(keyframes[before].Position, keyframes[after].Position, 0.5f);
            Quaternion a = Quaternion.Euler(keyframes[before].Rotation);
            Quaternion b = Quaternion.Euler(keyframes[after].Rotation);
            keyframe.Rotation = Quaternion.Lerp(a, b, 0.5f).eulerAngles;
            keyframe.Scale = Vector3.Lerp(keyframes[before].Scale, keyframes[after].Scale, 0.5f);
            keyframes.Add(keyframe);
            SortKeyframes();
        }
        public void AlterKeyframe(Vector3 position, Vector3 rotation, Vector3 tangent, Vector3 normal, int keyframeIndex)
        {
            keyframes[keyframeIndex].Position = position;
            keyframes[keyframeIndex].Rotation = rotation;
            keyframes[keyframeIndex].Normal = normal;
            keyframes[keyframeIndex].Tangent = tangent;

            BuildCurveFromKeyframes();
        }
        public void RemoveKeyframe(JSplineKeyframe keyframe)
        {
            keyframes.Remove(keyframe);
            BuildCurveFromKeyframes();
        }
        public void BuildCurveFromKeyframes()
        {
            ObjectSpline.BuildFromKeyframes(Keyframes);
        }


        public void SortKeyframes()
        {
            Keyframes.Sort(JSplineKeyframe.Comparer);
            BuildCurveFromKeyframes();
        }

        private void CreateEmpty()
        {
            ObjectSpline = new JSpline();
            Keyframes = new List<JSplineKeyframe>() { ScriptableObject.CreateInstance<JSplineKeyframe>(), ScriptableObject.CreateInstance<JSplineKeyframe>() };

            StartTime = 0.0f;
            EndTime = TimeLine.Sequence.Duration;

            Keyframes[0].Position = TimeLine.AffectedObject.transform.position;
            Keyframes[0].StartTime = StartTime;
            Keyframes[1].Position = TimeLine.transform.position;
            Keyframes[1].StartTime = EndTime;
            Keyframes[0].Track = this;
            Keyframes[1].Track = this;

        }
        public void StartTimeline()
        {
            SourcePosition = TimeLine.AffectedObject.transform.position;
            SourceRotation = TimeLine.AffectedObject.transform.rotation;
        }
        public void StopTimeline()
        {
            TimeLine.AffectedObject.transform.position = SourcePosition;
            TimeLine.AffectedObject.transform.rotation = SourceRotation;
        }
        public void SkipTimelineTo(float time)
        {
            Process(time, 1.0f);
        }
        public void Process(float sequencerTime, float playbackRate)
        {
            if (!TimeLine.AffectedObject)
                return;

            if (sequencerTime < StartTime || sequencerTime > EndTime)
                return;


            var sampleTime = (sequencerTime - StartTime) / ((EndTime - StartTime));
            //var easingFunction = Easing.GetEasingFunctionFor(easingType);
            sampleTime = Curve.Evaluate(sampleTime);
            //sampleTime = (float)easingFunction(sampleTime, 0.0, 1.0, 1.0);

            var modifiedRotation = sourceRotation;
            switch (SplineOrientationMode)
            {
                case SplineOrientationMode.LookAtTransform:
                    TimeLine.AffectedObject.position = ObjectSpline.GetPositionOnPath(sampleTime);
                    TimeLine.AffectedObject.LookAt(LookAtTarget, Vector3.up);
                    modifiedRotation = TimeLine.AffectedObject.rotation;
                    break;
                case SplineOrientationMode.LookAhead:
                    {
                        Vector3 nextNodePosition = ObjectSpline.GetPositionOnPath(sampleTime);
                        Vector3 prevNodePosition = TimeLine.AffectedObject.position;// ObjectSpline.GetPositionOnPath(sampleTime + JSequencer.SequenceUpdateRate);


                        Quaternion target = TimeLine.AffectedObject.rotation;
                        if ((nextNodePosition - prevNodePosition) != Vector3.zero)
                            target = Quaternion.LookRotation((nextNodePosition - prevNodePosition).normalized, Vector3.up);


                        TimeLine.AffectedObject.rotation = target;
                        TimeLine.AffectedObject.position = nextNodePosition;//ObjectSpline.GetPositionOnPath(sampleTime);

                        modifiedRotation = TimeLine.AffectedObject.rotation;
                        break;
                    }
                case SplineOrientationMode.ManualOrientation:
                    TimeLine.AffectedObject.position = ObjectSpline.GetPositionOnPath(sampleTime);
                    TimeLine.AffectedObject.rotation = ObjectSpline.GetRotation(sampleTime);
                    break;
                case SplineOrientationMode.None:
                    TimeLine.AffectedObject.position = ObjectSpline.GetPositionOnPath(sampleTime);
                    break;
            }


            // TimeLine.AffectedObject.position = ObjectSpline.GetPositionOnPath(sampleTime);

        }


        public void SetStartingOrientation()
        {
            switch (SplineOrientationMode)
            {
                case SplineOrientationMode.LookAtTransform:
                    TimeLine.AffectedObject.position = FirstNode.Position;
                    TimeLine.AffectedObject.LookAt(LookAtTarget, Vector3.up);
                    break;
                case SplineOrientationMode.LookAhead:
                    var nextNodePosition = ObjectSpline.GetPositionOnPath(TimeLine.Sequence.PlaybackRate > 0.0f ? JSequencer.SequenceUpdateRate : -JSequencer.SequenceUpdateRate);
                    TimeLine.AffectedObject.position = FirstNode.Position;
                    TimeLine.AffectedObject.LookAt(nextNodePosition, Vector3.up);
                    break;
                case SplineOrientationMode.ManualOrientation:
                    TimeLine.AffectedObject.position = FirstNode.Position;
                    break;
            }
        }

        public void OnDrawGizmos()
        {
            if (ObjectSpline == null)
                return;
            ObjectSpline.OnDrawGizmos();
        }

        public void FixupAdditionalObjects()
        {
            if (!String.IsNullOrEmpty(lookAtTargetPath))
            {
                var lookAtGameObject = (GameObject.Find(lookAtTargetPath) as GameObject);
                if (lookAtGameObject != null)
                    LookAtTarget = lookAtGameObject.transform;
            }

            if (LookAtTarget == null && !String.IsNullOrEmpty(lookAtTargetPath))
                Debug.LogWarning(string.Format("Tried to fixup a lookat target for this object path timeline, but it doesn't exist in this scene. (Target = {0}, ObjectPathTimeline = {1})", lookAtTargetPath, this), this);
        }

        public void RecordAdditionalObjects()
        {
            if (LookAtTarget != null)
                lookAtTargetPath = LookAtTarget.GetFullHierarchyPath();
        }
    }
}