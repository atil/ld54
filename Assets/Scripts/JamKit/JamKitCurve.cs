using System;
using System.Collections;
using UnityEngine;

namespace JamKit
{
    public partial class JamKit
    {
        public Coroutine TweenCoroutine(AnimationCurve curve, float duration, Action<float> perTickAction)
        {
            return Run(TweenCoroutine(curve, duration, perTickAction, () => { }));
        }

        public void Tween(AnimationCurve curve, float duration, Action<float> perTickAction, Action postAction)
        {
            Run(TweenCoroutine(curve, duration, perTickAction, postAction));
        }

        private IEnumerator TweenCoroutine(AnimationCurve curve, float duration, Action<float> perTickAction, Action postAction)
        {
            for (float f = 0f; f < duration; f += Time.deltaTime)
            {
                perTickAction(curve.Evaluate(f / duration));
                yield return null;
            }

            postAction();
        }

        public void TweenInfinite(AnimationCurve curve, float duration, Action<float> perTickAction)
        {
            Run(TweenInfiniteCoroutine(curve, duration, perTickAction));
        }

        private IEnumerator TweenInfiniteCoroutine(AnimationCurve curve, float duration, Action<float> perTickAction)
        {
            for (float f = 0f; ; f += Time.deltaTime)
            {
                perTickAction(curve.Evaluate(f / duration));
                if (f >= duration)
                {
                    f = 0;
                }

                yield return null;
            }
        }

        public Coroutine TweenDiscrete(AnimationCurve curve, float duration, float tickInterval, Action<float> perTickAction, Action postAction)
        {
            return Run(TweenDiscreteCoroutine(curve, duration, tickInterval, perTickAction, postAction));
        }

        private IEnumerator TweenDiscreteCoroutine(AnimationCurve curve, float duration, float tickInterval, Action<float> perTickAction, Action postAction)
        {
            int tickCount = (int)(duration / tickInterval);

            for (int i = 0; i < tickCount; i++)
            {
                float t = (float)i / tickCount;
                perTickAction(curve.Evaluate(t));
                yield return new WaitForSeconds(tickInterval);
            }

            postAction();
        }

    }
}