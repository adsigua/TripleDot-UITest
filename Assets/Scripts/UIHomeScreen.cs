using UnityEngine;
using UnityEngine.UI;

public class UIHomeScreen : UIScreen
{
    [SerializeField] private Button _settingsButton;

    public override void RegisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUIHomeScreenEventsListener listener)
            return;
        _settingsButton.onClick.AddListener(()=>listener.HandleSettingsButtonClicked());
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUIHomeScreenEventsListener listener)
            return;
        _settingsButton.onClick.RemoveAllListeners();
    }
}
