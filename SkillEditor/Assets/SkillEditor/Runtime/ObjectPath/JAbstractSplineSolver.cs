using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace CySkillEditor
{
    public enum JSplineType
    {
        Liner = 0,
        Bezier = 1,
        Hermite = 2,
        CatmullRom = 3,
        Quadratic = 4,
        CubicSpline = 5
    }


    public abstract class JAbstractSplineSolver : ScriptableObject
    {
        public const int TOTAL_SUBDIVISIONS_PER_NODE = 5;
        [SerializeField]
        public bool closedCurve = false;
        [SerializeField]
        protected Dictionary<float, Vector3> allPoints;
        public Dictionary<float, Vector3> AllPoints
        {
            get
            {
                if (allPoints == null)
                    allPoints = new Dictionary<float, Vector3>();
                return allPoints;
            }
            set { allPoints = value; }
        }
        [SerializeField]
        protected List<JSplineKeyframe> nodes;
        public List<JSplineKeyframe> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        [SerializeField]
        private float pathLength;
        public float PathLength
        {
            get { return pathLength; }
            set { pathLength = value; }
        }

        protected Dictionary<float, float> segmentTimeForDistance;
        public Dictionary<float, float> SegmentTimeForDistance
        {
            get
            {
                if (segmentTimeForDistance == null)
                    segmentTimeForDistance = new Dictionary<float, float>();
                return segmentTimeForDistance;
            }
            set { segmentTimeForDistance = value; }
        }
        private void OnEnable()
        {
            if (Nodes == null)
                Nodes = new List<JSplineKeyframe>();
            Build();
        }
        [SerializeField]
        public bool Builed = false;

        public virtual JSplineType SplineType()
        {
            throw new NotImplementedException();
        }


        public virtual void Build()
        {
            AllPoints.Clear();
            if (closedCurve)
            {
                Close();
            }
            var totalSudivisions = Nodes.Count * TOTAL_SUBDIVISIONS_PER_NODE;
            PathLength = 0.0f;
            float timePerSlice = 1.0f / totalSudivisions;
            SegmentTimeForDistance.Clear();
            var lastPoint = GetPosition(0.0f);

            for (var i = 1; i < totalSudivisions + 1; i++)
            {
                float currentTime = timePerSlice * i;
                var currentPoint = GetPosition(currentTime);
                PathLength += Vector3.Distance(currentPoint, lastPoint);
                lastPoint = currentPoint;
                SegmentTimeForDistance.Add(currentTime, PathLength);
            }
            Builed = true;
        }

        public virtual Vector3 GetPositionOnPath(float time)
        {
            if (!Builed)
                Build();

            var targetDistance = PathLength * time;
            var previousNodeTime = 0.0f;
            var previousNodeLength = 0.0f;
            var nextNodeTime = 0.0f;
            var nextNodeLength = 0.0f;

            foreach (var item in SegmentTimeForDistance)
            {
                if (item.Value >= targetDistance)
                {
                    nextNodeTime = item.Key;
                    nextNodeLength = item.Value;

                    if (previousNodeTime > 0)
                        previousNodeLength = SegmentTimeForDistance[previousNodeTime];

                    break;
                }
                previousNodeTime = item.Key;
            }

            // translate the values from the lookup table estimating the arc length between our known nodes from the lookup table
            var segmentTime = nextNodeTime - previousNodeTime;
            var segmentLength = nextNodeLength - previousNodeLength;
            var distanceIntoSegment = targetDistance - previousNodeLength;

            time = previousNodeTime + (distanceIntoSegment / segmentLength) * segmentTime;
            return GetPosition(time);
        }

        public virtual Vector3 GetRotationOnPath(float time)
        {
            if (!Builed)
                Build();

            var targetDistance = PathLength * time;
            var previousNodeTime = 0.0f;
            var previousNodeLength = 0.0f;
            var nextNodeTime = 0.0f;
            var nextNodeLength = 0.0f;

            foreach (var item in SegmentTimeForDistance)
            {
                if (item.Value >= targetDistance)
                {
                    nextNodeTime = item.Key;
                    nextNodeLength = item.Value;

                    if (previousNodeTime > 0)
                        previousNodeLength = SegmentTimeForDistance[previousNodeTime];

                    break;
                }
                previousNodeTime = item.Key;
            }

            // translate the values from the lookup table estimating the arc length between our known nodes from the lookup table
            var segmentTime = nextNodeTime - previousNodeTime;
            var segmentLength = nextNodeLength - previousNodeLength;
            var distanceIntoSegment = targetDistance - previousNodeLength;

            time = previousNodeTime + (distanceIntoSegment / segmentLength) * segmentTime;
            return GetPosition(time);
        }

        public virtual Quaternion GetSlerpRotationNode(int index, float t)
        {
            Quaternion v;
            Quaternion P0 = Quaternion.Euler(Nodes[index - 1].Rotation);
            Quaternion P1 = Quaternion.Euler(Nodes[index].Rotation);
            v = Quaternion.Slerp(P0, P1, t);
            return v;
        }

        public virtual Quaternion GetRotation(float time)
        {
            int numSections = Nodes.Count - 1;
            int currentNode = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currentNode;
            Quaternion point = GetSlerpRotationNode(currentNode + 1, u);
            return point;
        }
        public void Reverse()
        {
            Nodes.Reverse();
        }
        /// <summary>
        /// 绘制运动路线 插值算法 
        /// </summary>
        /// <param name="splineColor"></param>
        /// <param name="displayResolution">displayResolution 每两个关键点之间插值数量</param>
        public void OnInternalDrawGizmos(Color splineColor, float displayResolution)
        {
            Display(splineColor);

            var currentDisplayResolution = displayResolution;
            Color PreviousColor = Gizmos.color;
            Gizmos.color = splineColor;
            var previousPosition = GetPosition(0.0f);
            currentDisplayResolution *= Nodes.Count;
            for (var i = 1.0f; i <= currentDisplayResolution; i += 1.0f)
            {
                var t = i / currentDisplayResolution;
                var currentPosition = GetPosition(t);
                Gizmos.DrawLine(currentPosition, previousPosition);
                previousPosition = currentPosition;
            }
            Gizmos.color = PreviousColor;
        }

        public abstract void Display(Color splineColor);
        public abstract void Close();
        public abstract Vector3 GetPosition(float time);


    }
}