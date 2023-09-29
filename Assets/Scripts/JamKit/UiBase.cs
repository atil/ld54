using System;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public abstract class UiBase : MonoBehaviour
    {
        private enum FadeType
        {
            FadeIn, FadeOut
        }

        [SerializeField] private JamKit _jamKit;

        protected Globals Globals => _jamKit.Globals;

        [SerializeField] private Camera _camera;
        protected Camera Camera => _camera;

        [SerializeField] private Image _coverImage = default;
        protected Image CoverImage => _coverImage;

        public virtual SceneTransitionParams SceneTransitionParams => Globals.SceneTransitionParams;

        protected void FadeIn(SceneTransitionParams sceneTransitionParams = null, Action postAction = null)
        {
            Fade(FadeType.FadeIn, sceneTransitionParams, postAction);
        }

        protected void FadeOut(SceneTransitionParams sceneTransitionParams = null, Action postAction = null)
        {
            Fade(FadeType.FadeOut, sceneTransitionParams, postAction);
        }

        private void Fade(FadeType type, SceneTransitionParams sceneTransitionParams, Action postAction)
        {
            if (sceneTransitionParams == null)
            {
                sceneTransitionParams = Globals.SceneTransitionParams;
            }

            Color srcColor = type == FadeType.FadeIn ? sceneTransitionParams.Color : Color.clear;
            Color targetColor = type == FadeType.FadeIn ? Color.clear : sceneTransitionParams.Color;

            if (sceneTransitionParams.IsDiscrete)
            {
                _jamKit.TweenDiscrete(sceneTransitionParams.Curve,
                    sceneTransitionParams.Duration,
                    Globals.DiscreteTickInterval,
                    t =>
                    {
                        _coverImage.color = Color.Lerp(srcColor, targetColor, t);
                    },
                    () =>
                    {
                        _coverImage.color = targetColor;
                        postAction?.Invoke();
                    });
            }
            else
            {
                _jamKit.Tween(sceneTransitionParams.Curve,
                    sceneTransitionParams.Duration,
                    t =>
                    {
                        _coverImage.color = Color.Lerp(srcColor, targetColor, t);
                    },
                    () =>
                    {
                        _coverImage.color = targetColor;
                        postAction?.Invoke();
                    });
            }
        }

    }
}