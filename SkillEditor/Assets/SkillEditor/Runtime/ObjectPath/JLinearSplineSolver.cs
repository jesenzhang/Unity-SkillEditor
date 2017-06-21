using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CySkillEditor
{
    [System.Serializable]
    public class JLinearSplineSolver : JAbstractSplineSolver
    {
        private Dictionary<int, float> segmentStartLocations;

        public Dictionary<int, float> SegmentStartLocations
        {
            get
            {
                if (segmentStartLocations == null)
                {
                    segmentStartLocations = new Dictionary<int, float>();
                }
                return segmentStartLocations;
            }
            set
            {
                segmentStartLocations = value;
            }
        }
        private Dictionary<int, float> segmentDistances;

        public Dictionary<int, float> SegmentDistances
        {
            get
            {
                if (segmentDistances == null)
                {
                    segmentDistances = new Dictionary<int, float>();
                }
                return segmentDistances;
            }
            set
            {
                segmentDistances = value;
            }
        }
        private int currentSegment;

        public override JSplineType SplineType()
        {
            return JSplineType.Liner;
        }

        public override void Build()
        {
            PathLength = 0;
            SegmentStartLocations.Clear();
            SegmentDistances.Clear();
            if (closedCurve)
                Close();

            for (var i = 0; i < Nodes.Count - 1; i++)
            {
                // calculate the distance to the next node
                var distance = Vector3.Distance(Nodes[i].Position, Nodes[i + 1].Position);
                SegmentDistances.Add(i, distance);
                PathLength += distance;
            }

            // now that we have the total length we can loop back through and calculate the segmentStartLocations
            var accruedRouteLength = 0f;
            for (var i = 0; i < SegmentDistances.Keys.Count - 1; i++)
            {
                accruedRouteLength += SegmentDistances[i];
                SegmentStartLocations.Add(i + 1, accruedRouteLength / PathLength);
            }
            Builed = true;
        }


        public override void Close()
        {
            // add a node to close the route if necessary
            if (Nodes[0].Position != Nodes[Nodes.Count - 1].Position)
                Nodes.Add(Nodes[0]);
        }


        public override Vector3 GetPosition(float time)
        {
            return GetPositionOnPath(time);
        }


        public override Vector3 GetPositionOnPath(float time)
        {
            if (!Builed)
                Build();
            if (Nodes.Count < 3)
                return Vector3.Lerp(Nodes[0].Position, Nodes[1].Position, time);

            // which segment are we on?
            currentSegment = 0;
            foreach (var info in SegmentStartLocations)
            {
                if (info.Value < time)
                {
                    currentSegment = info.Key;
                    continue;
                }

                break;
            }

            var totalDistanceTravelled = time * PathLength;
            var i = currentSegment - 1;
            while (i >= 0)
            {
                totalDistanceTravelled -= SegmentDistances[i];
                --i;
            }

            return Vector3.Lerp(Nodes[currentSegment].Position, Nodes[currentSegment + 1].Position, totalDistanceTravelled / SegmentDistances[currentSegment]);
        }



        public override void Display(Color splineColor)
        {
            Color preColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Nodes[0].Position, Nodes[1].Position);
            Gizmos.color = preColor;
        }
    }
}