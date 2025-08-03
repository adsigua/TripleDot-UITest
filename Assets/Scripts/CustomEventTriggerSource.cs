using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AdoTrpDotTest
{
    public class CustomEventTriggerSource : MonoBehaviour
    {
        [System.Serializable]
        public class CustomEventTrigger
        {
            public string eventName = "custom event";
            public float eventDelay = 0;
            public UnityEvent unityEvent;
            private Tween _delayTween;

            public void TriggerEvents()
            {
                if (eventDelay <= 0)
                {
                    InvokeEvents();
                    return;
                }
                    
                if (_delayTween is { active: true })
                {
                    _delayTween.Kill();
                }
                _delayTween = DOVirtual.Float(0, 1.0f, eventDelay, (x) => { })
                    .OnComplete(InvokeEvents);
            }

            private void InvokeEvents()
            {
                unityEvent.Invoke();
            }
        }
        
        [SerializeField] public List<CustomEventTrigger> triggerEvents;
    }
}

