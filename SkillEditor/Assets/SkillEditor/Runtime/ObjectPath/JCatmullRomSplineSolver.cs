using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CySkillEditor
{
    [System.Serializable]
    public class JCatmullRomSplineSolver : JAbstractSplineSolver
    {
        public JCatmullRomSplineSolver(List<JSplineKeyframe> nodes)
        {
            Nodes = nodes;
        }
        public override JSplineType SplineType()
        {
            return JSplineType.CatmullRom;
        }
        public override void Close()
        {
            Nodes.RemoveAt(0);
            Nodes.RemoveAt(Nodes.Count - 1);

            if (Nodes[0] != Nodes[Nodes.Count - 1])
                Nodes.Add(Nodes[0]);

            var distanceToFirstNode = Vector3.Distance(Nodes[0].Position, Nodes[1].Position);
            var distanceToLastNode = Vector3.Distance(Nodes[0].Position, Nodes[Nodes.Count - 2].Position);

            var distanceToFirstTarget = distanceToLastNode / Vector3.Distance(Nodes[1].Position, Nodes[0].Position);
            var lastControlNode = (Nodes[0].Position + (Nodes[1].Position - Nodes[0].Position) * distanceToFirstTarget);

            var distanceToLastTarget = distanceToFirstNode / Vector3.Distance(Nodes[Nodes.Count - 2].Position, Nodes[0].Position);
            var firstControlNode = (Nodes[0].Position + (Nodes[Nodes.Count - 2].Position - Nodes[0].Position) * distanceToLastTarget);

            var firstControlKeyframe = new JSplineKeyframe();
            firstControlKeyframe.Position = firstControlNode;

            var lastControlKeyframe = new JSplineKeyframe();
            lastControlKeyframe.Position = lastControlNode;

            Nodes.Insert(0, firstControlKeyframe);
            Nodes.Add(lastControlKeyframe);
        }

        public override Vector3 GetPosition(float time)
        {
            int numSections = Nodes.Count - 3;
            int currentNode = Mathf.Min(Mathf.FloorToInt(time * (float)numSections), numSections - 1);
            float u = time * (float)numSections - (float)currentNode;

            Vector3 a = Nodes[currentNode].Position;
            Vector3 b = Nodes[currentNode + 1].Position;
            Vector3 c = Nodes[currentNode + 2].Position;
            Vector3 d = Nodes[currentNode + 3].Position;

            return .5f *
            (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
            );
        }

        public override void Display(Color splineColor)
        {
            if (Nodes.Count < 2)
                return;

            // draw the control points
            Color preColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Nodes[0].Position, Nodes[1].Position);
            Gizmos.DrawLine(Nodes[Nodes.Count - 1].Position, Nodes[Nodes.Count - 2].Position);
            Gizmos.color = preColor;
        }
    }
}