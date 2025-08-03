using UnityEngine;
using UnityEngine.UI;

namespace AdoTrpDotTest.UI
{
    public abstract class UIPopupScreen : UIScreen
    {
        [SerializeField] protected Button _closeButton;
    
        public override void RegisterEventsListener<T>(T objectListener)
        {
            if(objectListener is not IUIPopupScreenEventsListener listener)
                return;
            _closeButton.onClick.AddListener(listener.HandleCloseButtonClicked);
        }

        public override void UnregisterEventsListener<T>(T objectListener)
        {
            if(objectListener is not IUIPopupScreenEventsListener listener)
                return;
            _closeButton.onClick.RemoveListener(listener.HandleCloseButtonClicked);
        }
    }
}
