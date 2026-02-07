using DG.Tweening;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.UI
{
    public class ScreenCover : MonoBehaviourSingleton<ScreenCover>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image screenCover;
        [SerializeField] private Animator screenAnimator;

        private Action onAnimationCompleteCallback;

        public void ToggleScreenCover(bool on)
        {
            if(on)
            {
                canvas.enabled = true;
                screenCover.color = Color.black;
            }
            else
            {
                canvas.enabled = false;
            }
        }

        public IEnumerator FadeCoverIn(float time = 1f)
        {
            if (!canvas.enabled)
            {
                canvas.enabled = false;
                screenCover.color = Color.clear;
                canvas.enabled = true;
                yield return screenCover.DOColor(Color.black, time).SetEase(Ease.Linear).WaitForCompletion();
            }
        }

        public IEnumerator FadeCoverOut(float time = 1f)
        {
            if (canvas.enabled)
            {
                screenCover.color = Color.black;
                canvas.enabled = true;
                yield return screenCover.DOColor(Color.clear, time).SetEase(Ease.Linear).WaitForCompletion();
                canvas.enabled = false;
            }
        }

        public void AnimateScreenCoverIn(Action callback = null)
        {
            onAnimationCompleteCallback = callback;
            screenCover.color = Color.white;
            canvas.enabled = true;
            screenAnimator.SetTrigger("FadeIn");
        }

        public void AnimateScreenCoverOut(Action callback = null)
        {
            onAnimationCompleteCallback = callback;
            screenCover.color = Color.white;
            canvas.enabled = true;
            screenAnimator.SetTrigger("FadeOut");
        }

        public void OnAnimationComplete()
        {
            onAnimationCompleteCallback?.Invoke();
            
            if (screenAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut"))
            {
                screenCover.color = Color.clear;
                canvas.enabled = false;
            }
            
            onAnimationCompleteCallback = null;
        }
    }
}