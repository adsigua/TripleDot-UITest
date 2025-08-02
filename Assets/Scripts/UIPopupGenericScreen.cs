using UnityEngine;
using UnityEngine.UI;

public class UIPopupGenericScreen : UIPopupScreen
{
    [SerializeField] private Button _backButton;
    
    public override void RegisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUIPopupScreenEventsListener listener)
            return;
        _closeButton.onClick.AddListener(listener.HandleCloseButtonClicked);
        _backButton.onClick.AddListener(listener.HandleCloseButtonClicked);
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUIPopupScreenEventsListener listener)
            return;
        _closeButton.onClick.RemoveListener(listener.HandleCloseButtonClicked);
        _backButton.onClick.RemoveListener(listener.HandleCloseButtonClicked);
    }
}
