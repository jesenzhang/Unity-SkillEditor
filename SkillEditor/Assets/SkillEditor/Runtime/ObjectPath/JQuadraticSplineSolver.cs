using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CySkillEditor
{
    [System.Serializable]
    public class JQuadraticSplineSolver : JAbstractSplineSolver
    {
        public override JSplineType SplineType()
        {
            return JSplineType.Quadratic;
        }

        protected float quadBezierLength(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
        {
            var A = new Vector3[2];
            A[0] = controlPoint - startPoint;
            A[1] = startPoint - 2f * controlPoint + endPoint;

            float length;

            if (A[1] != Vector3.zero)
            {
                float c = 4.0f * Vector3.Dot(A[1], A[1]);
                float b = 8.0f * Vector3.Dot(A[0], A[1]);
                float a = 4.0f * Vector3.Dot(A[0], A[0]);
                float q = 4.0f * a * c - b * b;

                float twoCpB = 2.0f * c + b;
                float sumCBA = c + b + a;
                float mult0 = 0.25f / c;
                float mult1 = q / (8.0f * Mathf.Pow(c, 1.5f));
                length = mult0 * (twoCpB * Mathf.Sqrt(sumCBA) - b * Mathf.Sqrt(a)) +
                    mult1 * (Mathf.Log(2.0f * Mathf.Sqrt(c * sumCBA) + twoCpB) - Mathf.Log(2.0f * Mathf.Sqrt(c * a) + b));
            }
            else
                length = 2.0f * A[0].magnitude;

            return length;
        }

        public override void Close()
        {

        }

        public override Vector3 GetPosition(float time)
        {
            float d = 1.0f - time;
            return d * d * Nodes[0].Position + 2.0f * d * time * Nodes[1].Position + time * time * Nodes[2].Position;
        }


        public override void Display(Color splineColor)
        {
            Color preColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Nodes[0].Position, Nodes[1].Position);
            Gizmos.DrawLine(Nodes[Nodes.Count - 1].Position, Nodes[Nodes.Count - 2].Position);
            Gizmos.color = preColor;
        }
    }
}