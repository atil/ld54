using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JamKit
{
    public partial class JamKit
    {
        public Coroutine Run(IEnumerator function)
        {
            return StartCoroutine(function);
        }

        public void RunDelayed(float delay, Action function)
        {
            StartCoroutine(DelayedActionCoroutine(delay, function));
        }

        private IEnumerator DelayedActionCoroutine(float delay, Action function)
        {
            yield return new WaitForSeconds(delay);
            function();
        }

        public void Stop(Coroutine function)
        {
            if (function != null)
            {
                StopCoroutine(function);
            }
        }
    }
}