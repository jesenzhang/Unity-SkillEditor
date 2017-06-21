using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
namespace CySkillEditor
{
    [System.Serializable]
    public class JSpline
    {
        public int CurrentSegment
        {
            get;
            private set;
        }
        [SerializeField]
        private bool isClosed = false;
        public bool IsClosed
        {
            get { return isClosed; }
            set { isClosed = value; }
        }

        private bool IsReversed
        {
            get;
            set;
        }
        [SerializeField]
        private JSplineType splineType = JSplineType.Liner;
        public JSplineType SplineType
        {
            get { return splineType; }
            set
            {
                splineType = value;

            }
        }


        [SerializeField]
        private Color splineColor = Color.white;
        public Color SplineColor
        {
            get { return splineColor; }
            set { splineColor = value; }
        }

        [SerializeField]
        private float displayResolution = 20.0f;
        public float DisplayResolution
        {
            get { return displayResolution; }
            set { displayResolution = value; }
        }

        public List<JSplineKeyframe> Nodes
        {
            get { return SplineSolver.Nodes; }
        }

        [SerializeField]
        private JAbstractSplineSolver splineSolver;
        public JAbstractSplineSolver SplineSolver
        {
            get { return splineSolver; }
            private set { splineSolver = value; }
        }

        public Vector3 LastNode
        {
            get { return SplineSolver.Nodes[SplineSolver.Nodes.Count].Position; }
        }

        public void BuildFromKeyframes(List<JSplineKeyframe> keyframes)
        {
            bool keyframeDifference = SplineSolver == null;

            if (SplineSolver != null)
            {

                keyframeDifference = keyframes.Count() != Nodes.Count();

                if (SplineType != SplineSolver.SplineType())
                {
                    keyframeDifference = true;
                }
            }

            if (keyframeDifference)
            {
                if (SplineSolver != null)
                    ScriptableObject.DestroyImmediate(SplineSolver);
                if (SplineType == JSplineType.Liner)
                {
                    if (keyframes.Count >= 2)
                        SplineSolver = ScriptableObject.CreateInstance<JLinearSplineSolver>();
                    else
                        throw new SystemException("Need At Least 2 Points");
                }
                else if (SplineType == JSplineType.Hermite)
                {
                    if (keyframes.Count >= 2)
                        SplineSolver = ScriptableObject.CreateInstance<JHermiteSplineSolver>();
                    else
                        throw new SystemException("Need At Least 2 Points");
                }
                else if (SplineType == JSplineType.Bezier)
                {
                    if (keyframes.Count >= 2)
                        SplineSolver = ScriptableObject.CreateInstance<JNTimesBezierSplineSolver>();
                    else
                        throw new SystemException("Need At Least 2 Points");
                }
                else if (SplineType == JSplineType.CatmullRom)
                {
                    if (keyframes.Count >= 4)
                        SplineSolver = ScriptableObject.CreateInstance<JCatmullRomSplineSolver>();
                    else
                        throw new SystemException("Need At Least 4 Points");
                }
                else if (SplineType == JSplineType.CubicSpline)
                {
                    if (keyframes.Count >= 2)
                        SplineSolver = ScriptableObject.CreateInstance<JNaturalCubicSplineSolver>();
                    else
                        throw new SystemException("Need At Least 2 Points");
                }
            }
            if (SplineSolver != null)
            {
                SplineSolver.Nodes = keyframes;
                SplineSolver.closedCurve = IsClosed;
                SplineSolver.Build();
            }
        }

        private Vector3 GetPosition(float time)
        {
            return SplineSolver.GetPosition(time);
        }

        public Quaternion GetRotation(float time)
        {
            return SplineSolver.GetRotation(time);
        }

        public Vector3 GetPositionOnPath(float time)
        {
            if (time < 0.0f || time > 1.0f)
            {
                if (IsClosed)
                {
                    if (time < 0.0f)
                        time += 1.0f;
                    else
                        time -= 1.0f;
                }
                else
                    time = Mathf.Clamp01(time);
            }

            return SplineSolver.GetPositionOnPath(time);
        }

        public Vector3 GetRotationOnPath(float time)
        {
            if (time < 0.0f || time > 1.0f)
            {
                if (IsClosed)
                {
                    if (time < 0.0f)
                        time += 1.0f;
                    else
                        time -= 1.0f;
                }
                else
                    time = Mathf.Clamp01(time);
            }

            return SplineSolver.GetRotationOnPath(time);
        }

        public void Close()
        {
            if (IsClosed)
                throw new System.Exception("Closing a Spline that is already closed");

            IsClosed = true;
            SplineSolver.Close();
        }

        public void Reverse()
        {
            if (!IsReversed)
            {
                SplineSolver.Reverse();
                IsReversed = true;
            }
            else
            {
                SplineSolver.Reverse();
                IsReversed = false;
            }
        }

        public void OnDrawGizmos()
        {
            if (SplineSolver == null)
                return;

            SplineSolver.OnInternalDrawGizmos(SplineColor, DisplayResolution);
        }
    }
}