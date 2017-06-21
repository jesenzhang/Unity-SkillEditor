using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace CySkillEditor
{
    public enum CubicSplineMode
    {
        CUBIC_NATURAL = 0,    // Natural
        CUBIC_CLAMPED = 1,    // TODO: Clamped 
        CUBIC_NOT_A_KNOT = 2  // TODO: Not a knot 
    }

    public enum SplineFilterMode
    {
        CUBIC_WITHOUT_FILTER = 0, // without filter
        CUBIC_MEDIAN_FILTER = 1 // median filter
    }


    public class CubicSplineCoeffs
    {
        public CubicSplineCoeffs(int count)
        {
            a = new float[count];
            b = new float[count];
            c = new float[count];
            d = new float[count];
        }

        public float[] a, b, c, d;
    }



    public class JNaturalCubicSplineSolver : JAbstractSplineSolver
    {
        public JNaturalCubicSplineSolver(List<JSplineKeyframe> nodes)
        {
            Nodes = nodes;
        }

        CubicSplineCoeffs Coeff_x;
        CubicSplineCoeffs Coeff_y;
        CubicSplineCoeffs Coeff_z;

        List<float> input_t = new List<float>();
        List<float> input_x = new List<float>();
        List<float> input_y = new List<float>();
        List<float> input_z = new List<float>();


        public override void Build()
        {
            AllPoints.Clear();
            if (closedCurve)
            {
                Close();
            }
            CaculateSpline();

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
        }

        public void Fit(float[] x, float[] y, out float[] Coeff_a, out float[] Coeff_b, float startSlope = float.NaN, float endSlope = float.NaN)
        {
            if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
            {
                throw new Exception("startSlope and endSlope cannot be infinity.");
            }

            int n = x.Length;
            float[] r = new float[n]; // the right hand side numbers: wikipedia page overloads b

            TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
            float dx1, dx2, dy1, dy2;

            // First row is different (equation 16 from the article)
            if (float.IsNaN(startSlope))
            {
                dx1 = x[1] - x[0];
                m.C[0] = 1.0f / dx1;
                m.B[0] = 2.0f * m.C[0];
                r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
            }
            else
            {
                m.B[0] = 1;
                r[0] = startSlope;
            }

            // Body rows (equation 15 from the article)
            for (int i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];

                m.A[i] = 1.0f / dx1;
                m.C[i] = 1.0f / dx2;
                m.B[i] = 2.0f * (m.A[i] + m.C[i]);

                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            // Last row also different (equation 17 from the article)
            if (float.IsNaN(endSlope))
            {
                dx1 = x[n - 1] - x[n - 2];
                dy1 = y[n - 1] - y[n - 2];
                m.A[n - 1] = 1.0f / dx1;
                m.B[n - 1] = 2.0f * m.A[n - 1];
                r[n - 1] = 3 * (dy1 / (dx1 * dx1));
            }
            else
            {
                m.B[n - 1] = 1;
                r[n - 1] = endSlope;
            }

            // k is the solution to the matrix
            float[] k = m.Solve(r);
            // a and b are each spline's coefficients
            Coeff_a = new float[n - 1];
            Coeff_b = new float[n - 1];

            for (int i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                Coeff_a[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
                Coeff_b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
            }

        }
        private float EvalSpline(float x, int j, float[] xOrig, float[] yOrig, float[] Coeff_a, float[] Coeff_b)
        {
            float dx = xOrig[j + 1] - xOrig[j];
            float t = (x - xOrig[j]) / dx;
            float y = (1 - t) * yOrig[j] + t * yOrig[j + 1] + t * (1 - t) * (Coeff_a[j] * (1 - t) + Coeff_b[j] * t); // equation 9 
            return y;
        }

        public void CaculateSpline()
        {
            float begintime = Nodes[0].StartTime;
            float totaltime = Nodes[Nodes.Count - 1].StartTime - begintime;
            input_t = new List<float>();
            input_x = new List<float>();
            input_y = new List<float>();
            input_z = new List<float>();

            for (int i = 0; i < Nodes.Count; i++)
            {
                input_t.Add((Nodes[i].StartTime - begintime) / totaltime);
                input_x.Add(Nodes[i].Position.x);
                input_y.Add(Nodes[i].Position.y);
                input_z.Add(Nodes[i].Position.z);
            }
            Coeff_x = new CubicSplineCoeffs(Nodes.Count - 1);
            Coeff_y = new CubicSplineCoeffs(Nodes.Count - 1);
            Coeff_z = new CubicSplineCoeffs(Nodes.Count - 1);

            Fit(input_t.ToArray(), input_x.ToArray(), out Coeff_x.a, out Coeff_x.b);
            Fit(input_t.ToArray(), input_y.ToArray(), out Coeff_y.a, out Coeff_y.b);
            Fit(input_t.ToArray(), input_z.ToArray(), out Coeff_z.a, out Coeff_z.b);


        }
        public override void Close()
        {

        }

        public override JSplineType SplineType()
        {
            return JSplineType.CubicSpline;
        }

        private int GetNextXIndex(float x, float[] xOrig)
        {
            int _lastIndex = 0;

            if (x < xOrig[_lastIndex])
            {
                throw new ArgumentException("The X values to evaluate must be sorted.");
            }

            while ((_lastIndex < xOrig.Length - 2) && (x > xOrig[_lastIndex + 1]))
            {
                _lastIndex++;
            }

            return _lastIndex;
        }

        public override Vector3 GetPosition(float time)
        {
            if (time == 0)
                return Nodes[0].Position;
            if (AllPoints != null && AllPoints.ContainsKey(time))
                return AllPoints[time];
            Vector3 output = cubicSplineInterpolation(time);
            AllPoints.Add(time, output);
            return output;

        }
        public override void Display(Color splineColor)
        {

        }
        private Vector3 cubicSplineInterpolation(float time)
        {
            Vector3 output = Vector3.zero;
            int count = input_x.Count;

            int j = GetNextXIndex(time, input_t.ToArray());

            output.x = EvalSpline(time, j, input_t.ToArray(), input_x.ToArray(), Coeff_x.a, Coeff_x.b);
            output.y = EvalSpline(time, j, input_t.ToArray(), input_y.ToArray(), Coeff_y.a, Coeff_y.b);
            output.z = EvalSpline(time, j, input_t.ToArray(), input_z.ToArray(), Coeff_z.a, Coeff_z.b);

            return output;
        }

    }
}