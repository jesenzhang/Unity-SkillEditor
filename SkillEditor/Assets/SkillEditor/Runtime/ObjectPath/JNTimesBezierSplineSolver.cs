using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CySkillEditor
{
    [System.Serializable]
    public class JNTimesBezierSplineSolver : JAbstractSplineSolver
    {
        public JNTimesBezierSplineSolver(List<JSplineKeyframe> nodes)
        {
            Nodes = nodes;
        }

        public override void Close()
        {

        }
        public override JSplineType SplineType()
        {
            return JSplineType.Bezier;
        }

        /// <summary>
        /// 德卡斯特里奥算法 N 是贝塞尔曲线的阶数 iter是迭代到第几轮 t是曲线上的归一化时间 （0-1）
        /// </summary>
        /// <param name="N"></param>
        /// <param name="iter"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 deCasteljauBezier(int N, int iter, float t)
        {
            if (N == 1)
            {
                return (1 - t) * Nodes[iter].Position + t * Nodes[iter + 1].Position;
            }
            return (1 - t) * deCasteljauBezier(N - 1, iter, t) + t * deCasteljauBezier(N - 1, iter + 1, t);
        }

        public override Vector3 GetPosition(float time)
        {
            if (AllPoints != null && AllPoints.ContainsKey(time))
                return AllPoints[time];

            int N = Nodes.Count - 1;
            Vector3 point = deCasteljauBezier(N, 0, time);
            AllPoints.Add(time, point);
            return point;

            // float d = 1f - time;
            //return d * d * d * Nodes[0].Position + 3f * d * d * time * Nodes[1].Position + 3f * d * time * time * Nodes[2].Position + time * time * time * Nodes[3].Position;
        }


        public override void Display(Color splineColor)
        {
            Color preColor = Gizmos.color;
            Gizmos.color = Color.red;
            for (int i = 0; i < Nodes.Count - 1; i++)
            {
                Gizmos.DrawLine(Nodes[i].Position, Nodes[i + 1].Position);
            }
            Gizmos.color = preColor;
        }
    }
}