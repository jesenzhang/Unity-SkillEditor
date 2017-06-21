using UnityEngine;
using System.Collections;
using System;

namespace CySkillEditor
{
    public static class Easing
    {
        public enum EasingType
        {
            Linear,

            QuadraticEaseOut,
            QuadraticEaseIn,
            QuadraticEaseInOut,
            QuadraticEaseOutIn,

            SineEaseOut,
            SineEaseIn,
            SineEaseInOut,
            SineEaseOutIn,

            ExponentialEaseOut,
            ExponentialEaseIn,
            ExponentialEaseInOut,
            ExponentialEaseOutIn,

            CirclicEaseOut,
            CirclicEaseIn,
            CirclicEaseInOut,
            CirclicEaseOutIn,

            CubicEaseOut,
            CubicEaseIn,
            CubicEaseInOut,
            CubicEaseOutIn,

            QuarticEaseOut,
            QuarticEaseIn,
            QuarticEaseInOut,
            QuarticEaseOutIn,

            QuinticEaseOut,
            QuinticEaseIn,
            QuinticEaseInOut,
            QuinticEaseOutIn,

            ElasticEaseOut,
            ElasticEaseIn,
            ElasticEaseInOut,
            ElasticEaseOutIn,

            BounceEaseOut,
            BounceEaseIn,
            BounceEaseInOut,
            BounceEaseOutIn,

            BackEaseOut,
            BackEaseIn,
            BackEaseInOut,
            BackEaseOutIn,
        }

        public delegate double EasingFunction(double currentTime, double startingValue, double finalValue, double duration);
        public static EasingFunction GetEasingFunctionFor(Easing.EasingType easingType)
        {
            switch (easingType)
            {
                case Easing.EasingType.Linear:
                    return Linear;

                case Easing.EasingType.ExponentialEaseOut:
                    return ExponentialEaseOut;
                case Easing.EasingType.ExponentialEaseIn:
                    return ExponentialEaseIn;
                case Easing.EasingType.ExponentialEaseInOut:
                    return ExponentialEaseInOut;
                case Easing.EasingType.ExponentialEaseOutIn:
                    return ExponentialEaseOutIn;

                case Easing.EasingType.CirclicEaseOut:
                    return CirclicEaseOut;
                case Easing.EasingType.CirclicEaseIn:
                    return CirclicEaseIn;
                case Easing.EasingType.CirclicEaseInOut:
                    return CirclicEaseInOut;
                case Easing.EasingType.CirclicEaseOutIn:
                    return CirclicEaseOutIn;

                case Easing.EasingType.QuadraticEaseOut:
                    return QuadraticEaseOut;
                case Easing.EasingType.QuadraticEaseIn:
                    return QuadraticEaseIn;
                case Easing.EasingType.QuadraticEaseInOut:
                    return QuadraticEaseInOut;
                case Easing.EasingType.QuadraticEaseOutIn:
                    return QuadraticEaseOutIn;

                case Easing.EasingType.SineEaseOut:
                    return SineEaseOut;
                case Easing.EasingType.SineEaseIn:
                    return SineEaseIn;
                case Easing.EasingType.SineEaseInOut:
                    return SineEaseInOut;
                case Easing.EasingType.SineEaseOutIn:
                    return SineEaseOutIn;

                case Easing.EasingType.CubicEaseOut:
                    return CubicEaseOut;
                case Easing.EasingType.CubicEaseIn:
                    return CubicEaseIn;
                case Easing.EasingType.CubicEaseInOut:
                    return CubicEaseInOut;
                case Easing.EasingType.CubicEaseOutIn:
                    return CubicEaseOutIn;

                case Easing.EasingType.QuarticEaseOut:
                    return QuarticEaseOut;
                case Easing.EasingType.QuarticEaseIn:
                    return QuarticEaseIn;
                case Easing.EasingType.QuarticEaseInOut:
                    return QuarticEaseInOut;
                case Easing.EasingType.QuarticEaseOutIn:
                    return QuarticEaseOutIn;

                case Easing.EasingType.QuinticEaseOut:
                    return QuinticEaseOut;
                case Easing.EasingType.QuinticEaseIn:
                    return QuinticEaseIn;
                case Easing.EasingType.QuinticEaseInOut:
                    return QuinticEaseInOut;
                case Easing.EasingType.QuinticEaseOutIn:
                    return QuinticEaseOutIn;

                case Easing.EasingType.ElasticEaseOut:
                    return ElasticEaseOut;
                case Easing.EasingType.ElasticEaseIn:
                    return ElasticEaseIn;
                case Easing.EasingType.ElasticEaseInOut:
                    return ElasticEaseInOut;
                case Easing.EasingType.ElasticEaseOutIn:
                    return ElasticEaseOutIn;

                case Easing.EasingType.BounceEaseOut:
                    return BounceEaseOut;
                case Easing.EasingType.BounceEaseIn:
                    return BounceEaseIn;
                case Easing.EasingType.BounceEaseInOut:
                    return BounceEaseInOut;
                case Easing.EasingType.BounceEaseOutIn:
                    return BounceEaseOutIn;

                case Easing.EasingType.BackEaseOut:
                    return BackEaseOut;
                case Easing.EasingType.BackEaseIn:
                    return BackEaseIn;
                case Easing.EasingType.BackEaseInOut:
                    return BackEaseInOut;
                case Easing.EasingType.BackEaseOutIn:
                    return BackEaseOutIn;

                default:
                    {
                        throw new Exception("Easing type not implemented");
                    }
            }
        }

        #region Linear
        /// <summary>
        /// A Linear Tween with no easing
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double Linear(double t, double b, double c, double d)
        {
            return c * t / d + b;
        }

        #endregion

        #region Exponential
        /// <summary>
        /// Easing with exponential (2^t) easing out:
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExponentialEaseOut(double t, double b, double c, double d)
        {
            return (t == d) ? b + c : c * (-System.Math.Pow(2, -10 * t / d) + 1) + b;
        }

        /// <summary>
        /// Easing with exponential (2^t) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExponentialEaseIn(double t, double b, double c, double d)
        {
            return (t == 0) ? b : c * System.Math.Pow(2, 10 * (t / d - 1)) + b;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExponentialEaseInOut(double t, double b, double c, double d)
        {
            if (t == 0)
            {
                return b;
            }

            if (t == d)
            {
                return b + c;
            }

            if ((t /= d / 2) < 1)
            {
                return c / 2 * System.Math.Pow(2, 10 * (t - 1)) + b;
            }

            return c / 2 * (-System.Math.Pow(2, -10 * --t) + 2) + b;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ExponentialEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return ExponentialEaseOut(t * 2, b, c / 2, d);
            }

            return ExponentialEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Circular
        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CirclicEaseOut(double t, double b, double c, double d)
        {
            return c * System.Math.Sqrt(1 - (t = t / d - 1) * t) + b;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CirclicEaseIn(double t, double b, double c, double d)
        {
            return -c * (System.Math.Sqrt(1 - (t /= d) * t) - 1) + b;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CirclicEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1)
            {
                return -c / 2 * (System.Math.Sqrt(1 - t * t) - 1) + b;
            }

            return c / 2 * (System.Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CirclicEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return CirclicEaseOut(t * 2, b, c / 2, d);
            }

            return CirclicEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Quad
        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadraticEaseOut(double t, double b, double c, double d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadraticEaseIn(double t, double b, double c, double d)
        {
            return c * (t /= d) * t + b;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadraticEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1)
            {
                return c / 2 * t * t + b;
            }

            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuadraticEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return QuadraticEaseOut(t * 2, b, c / 2, d);
            }

            return QuadraticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Sine
        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseOut(double t, double b, double c, double d)
        {
            return c * System.Math.Sin(t / d * (System.Math.PI / 2)) + b;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseIn(double t, double b, double c, double d)
        {
            return -c * System.Math.Cos(t / d * (System.Math.PI / 2)) + c + b;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1)
            {
                return c / 2 * (System.Math.Sin(System.Math.PI * t / 2)) + b;
            }

            return -c / 2 * (System.Math.Cos(System.Math.PI * --t / 2) - 2) + b;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double SineEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return SineEaseOut(t * 2, b, c / 2, d);
            }

            return SineEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Cubic
        /// <summary>
        /// Easing equation function for a cubic (t^3) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseOut(double t, double b, double c, double d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseIn(double t, double b, double c, double d)
        {
            return c * (t /= d) * t * t + b;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1)
            {
                return c / 2 * t * t * t + b;
            }

            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double CubicEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return CubicEaseOut(t * 2, b, c / 2, d);
            }

            return CubicEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Quartic
        /// <summary>
        /// Easing equation function for a quartic (t^4) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuarticEaseOut(double t, double b, double c, double d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuarticEaseIn(double t, double b, double c, double d)
        {
            return c * (t /= d) * t * t * t + b;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuarticEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1)
            {
                return c / 2 * t * t * t * t + b;
            }

            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuarticEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return QuarticEaseOut(t * 2, b, c / 2, d);
            }

            return QuarticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Quintic
        /// <summary>
        /// Easing equation function for a quintic (t^5) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuinticEaseOut(double t, double b, double c, double d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuinticEaseIn(double t, double b, double c, double d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuinticEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) < 1)
            {
                return c / 2 * t * t * t * t * t + b;
            }
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double QuinticEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return QuinticEaseOut(t * 2, b, c / 2, d);
            }
            return QuinticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Elastic
        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseOut(double t, double b, double c, double d)
        {
            if ((t /= d) == 1)
            {
                return b + c;
            }

            double p = d * .3;
            double s = p / 4;

            return (c * System.Math.Pow(2, -10 * t) * System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p) + c + b);
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseIn(double t, double b, double c, double d)
        {
            if ((t /= d) == 1)
            {
                return b + c;
            }

            double p = d * .3;
            double s = p / 4;

            return -(c * System.Math.Pow(2, 10 * (t -= 1)) * System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p)) + b;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseInOut(double t, double b, double c, double d)
        {
            if ((t /= d / 2) == 2)
            {
                return b + c;
            }

            double p = d * (.3 * 1.5);
            double s = p / 4;

            if (t < 1)
            {
                return -.5 * (c * System.Math.Pow(2, 10 * (t -= 1)) * System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p)) + b;
            }
            return c * System.Math.Pow(2, -10 * (t -= 1)) * System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p) * .5 + c + b;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double ElasticEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return ElasticEaseOut(t * 2, b, c / 2, d);
            }
            return ElasticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Bounce
        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseOut(double t, double b, double c, double d)
        {
            if ((t /= d) < (1 / 2.75))
            {
                return c * (7.5625 * t * t) + b;
            }
            else if (t < (2 / 2.75))
            {
                return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
            }
            else if (t < (2.5 / 2.75))
            {
                return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
            }
            else
            {
                return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
            }
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseIn(double t, double b, double c, double d)
        {
            return c - BounceEaseOut(d - t, 0, c, d) + b;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseInOut(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return BounceEaseIn(t * 2, 0, c, d) * .5 + b;
            }
            else
            {
                return BounceEaseOut(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
            }
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BounceEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return BounceEaseOut(t * 2, b, c / 2, d);
            }
            return BounceEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        #region Back
        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseOut(double t, double b, double c, double d)
        {
            return c * ((t = t / d - 1) * t * ((1.70158 + 1) * t + 1.70158) + 1) + b;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseIn(double t, double b, double c, double d)
        {
            return c * (t /= d) * t * ((1.70158 + 1) * t - 1.70158) + b;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseInOut(double t, double b, double c, double d)
        {
            double s = 1.70158;
            if ((t /= d / 2) < 1)
            {
                return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
            }
            return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static double BackEaseOutIn(double t, double b, double c, double d)
        {
            if (t < d / 2)
            {
                return BackEaseOut(t * 2, b, c / 2, d);
            }
            return BackEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion
    }
}