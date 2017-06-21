using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CySkillEditor
{
    [System.Serializable]
    public class JHermiteSplineSolver : JAbstractSplineSolver
    {

        [SerializeField]
        public float smoothFactor = 2.0f;

        public override void Close()
        {
            // add a node to close the route if necessary
            if (Nodes[0].Position != Nodes[Nodes.Count - 1].Position)
                Nodes.Add(Nodes[0]);
        }
        public override JSplineType SplineType()
        {
            return JSplineType.Hermite;
        }
        /// <summary>
        /// Hermite 曲线
        /// </summary>
        /// <param name="N"></param>
        /// <param name="iter"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 HermiteFunc(Vector3 p1, Vector3 p2, Vector3 t1, Vector3 t2, float t)
        {
            //调和函数
            float param1 = 2 * t * t * t - 3 * t * t + 1;
            float param2 = -2 * t * t * t + 3 * t * t;
            float param3 = t * t * t - 2 * t * t + t;
            float param4 = t * t * t - t * t;

            Vector3 val = p1 * param1 + p2 * param2 + t1 * param3 / smoothFactor + t2 * param4 / smoothFactor;
            return val;
        }
        public Vector3 GetHermitNode(int index, float t)
        {
            Vector3 v;
            Vector3 P0 = Nodes[index - 1].Position;
            Vector3 P1 = Nodes[index].Position;
            Vector3 P2 = Nodes[index - 1].Tangent;
            Vector3 P3 = Nodes[index].Tangent;
            v = HermiteFunc(P0, P1, P2, P3, t);
            return v;
        }


        public override Vector3 GetPosition(float time)
        {
            if (AllPoints != null && AllPoints.ContainsKey(time))
                return AllPoints[time];

            int numSections = Nodes.Count - 1;
            int currentNode = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currentNode;
            Vector3 point = GetHermitNode(currentNode + 1, u);
            AllPoints.Add(time, point);
            return point;
        }

        public override Quaternion GetRotation(float time)
        {
            int numSections = Nodes.Count - 1;
            int currentNode = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currentNode;
            Quaternion point = GetSlerpRotationNode(currentNode + 1, u);
            return point;
        }

        public override void Display(Color splineColor)
        {
            Color preColor = Gizmos.color;
            Gizmos.color = Color.red;
            for (int i = 0; i < Nodes.Count; i++)
            {
                Gizmos.DrawLine(Nodes[i].Position, Nodes[i].Position + Nodes[i].Tangent);
                //Gizmos.DrawLine(Nodes[i].Position, Nodes[i + 1].Position);
            }
            Gizmos.color = preColor;
        }
    }
}