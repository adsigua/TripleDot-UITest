using UnityEngine;
using UnityEngine.UI;

public class UISettingsScreen : UIScreen
{
    [SerializeField] private UICustomToggleSlider _soundToggleSlider;
    [SerializeField] private UICustomToggleSlider _musicToggleSlider;
    [SerializeField] private UICustomToggleSlider _vibrationToggleSlider;
    [SerializeField] private UICustomToggleSlider _notifToggleSlider;
    [SerializeField] private Button _languageButton;
    [SerializeField] private Button _closeButton;
    
    public override void InitScreen(UIScreenInitData uiScreenInitData)
    {
        base.InitScreen(uiScreenInitData);
        if(uiScreenInitData is not UISettingsScreenInitData settingsInitData)
            return;
        _soundToggleSlider.SetToggleState(settingsInitData.soundOn, true);
        _musicToggleSlider.SetToggleState(settingsInitData.musicOn, true);
        _vibrationToggleSlider.SetToggleState(settingsInitData.vibrationOn, true);
        _notifToggleSlider.SetToggleState(settingsInitData.notifsOn, true);
    }

    public override void RegisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
            return;
        _soundToggleSlider.OnToggleValueChanged += listener.HandleSoundToggleValueChanged;
        _musicToggleSlider.OnToggleValueChanged += listener.HandleMusicToggleValueChanged;
        _vibrationToggleSlider.OnToggleValueChanged += listener.HandleVibrationToggleValueChanged;
        _notifToggleSlider.OnToggleValueChanged += listener.HandleNotifToggleValueChanged;
        _closeButton.onClick.AddListener(listener.HandleCloseButtonClicked);
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
            return;
        _soundToggleSlider.OnToggleValueChanged -= listener.HandleSoundToggleValueChanged;
        _musicToggleSlider.OnToggleValueChanged -= listener.HandleMusicToggleValueChanged;
        _vibrationToggleSlider.OnToggleValueChanged -= listener.HandleVibrationToggleValueChanged;
        _notifToggleSlider.OnToggleValueChanged -= listener.HandleNotifToggleValueChanged;
        _closeButton.onClick.RemoveAllListeners();
    }
}
