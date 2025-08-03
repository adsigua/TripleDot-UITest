using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AdoTrpDotTest.UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        public bool isShown { get; private set; } = false;
    
        [SerializeField] protected List<UIAnimatedElement> _animatedElements;
        [SerializeField] protected List<CustomEventTriggerSource> _customEventTriggers;
        [SerializeField] protected float _hideScreenDelay = 0.0f;
        protected Tween _hideScreenTween;

        public virtual void InitScreen(UIScreenInitData uiScreenInitData)
        {
        
        }

        public virtual void ShowScreen(bool doAnimation = true, System.Action onComplete = null)
        {
            if(_hideScreenTween is {active: true})
                _hideScreenTween.Kill();
            gameObject.SetActive(true);
            if (doAnimation && _animatedElements.Count > 0)
            {
                PlayEntryAnimation();
            }
            isShown = true;
        }

        public virtual void HideScreen(bool doAnimation = true, System.Action onComplete = null)
        {
            if(_hideScreenTween is {active: true})
                _hideScreenTween.Kill();
            if (_animatedElements.Count == 0 || !doAnimation)
            {
                gameObject.SetActive(false);
                isShown = false;
                onComplete?.Invoke();
                return;
            }
            PlayExitAnimation();
            _hideScreenTween = DOVirtual.Float(0, 1, _hideScreenDelay, (x)=>{})
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    gameObject.SetActive(false);
                    isShown = false;
                });
        }
    
        protected void PlayEntryAnimation()
        {
            foreach (UIAnimatedElement uiElement in _animatedElements)
            {
                uiElement.InitElementState();
                uiElement.PlayShowAnimation();
            }
            TriggerCustomEventByIndex(0);
        }
    
        protected void PlayExitAnimation()
        {
            foreach (UIAnimatedElement uiElement in _animatedElements)
            {
                uiElement.PlayExitAnimation();
            }

            TriggerCustomEventByIndex(1);
        }

        protected void TriggerCustomEventByIndex(int index)
        {
            foreach (var eventTrigger in _customEventTriggers)
            {
                if (eventTrigger.triggerEvents.Count <= index)
                    continue;
                eventTrigger.triggerEvents[index].TriggerEvents();
            }
        }

        public abstract void RegisterEventsListener<T>(T objectListener) where T : class;
        public abstract void UnregisterEventsListener<T>(T objectListener) where T : class;
    }
}
