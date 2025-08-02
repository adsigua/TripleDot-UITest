using UnityEngine;
using UnityEngine.UI;

public class UISettingsScreen : UIPopupScreen
{
    [SerializeField] private UICustomToggleSlider _soundToggleSlider;
    [SerializeField] private UICustomToggleSlider _musicToggleSlider;
    [SerializeField] private UICustomToggleSlider _vibrationToggleSlider;
    [SerializeField] private UICustomToggleSlider _notifToggleSlider;
    [SerializeField] private Button _languageButton;
    [SerializeField] private Button _tncButton;
    [SerializeField] private Button _privacyButton;
    [SerializeField] private Button _supportButton;
    
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
        if (objectListener is not IUISettingsScreenEventsListener listener)
        {
            base.RegisterEventsListener(objectListener);
            return;
        }
        _soundToggleSlider.OnToggleValueChanged += listener.HandleSoundToggleValueChanged;
        _musicToggleSlider.OnToggleValueChanged += listener.HandleMusicToggleValueChanged;
        _vibrationToggleSlider.OnToggleValueChanged += listener.HandleVibrationToggleValueChanged;
        _notifToggleSlider.OnToggleValueChanged += listener.HandleNotifToggleValueChanged;
        
        _tncButton.onClick.AddListener(listener.HandleTermsAndConditionsButtonClicked);
        _privacyButton.onClick.AddListener(listener.HandlePrivacyButtonClicked);
        _supportButton.onClick.AddListener(listener.HandleSupportButtonClicked);
        _languageButton.onClick.AddListener(listener.HandleLanguageButtonClicked);

        base.RegisterEventsListener(objectListener);
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
        {
            base.UnregisterEventsListener(objectListener);
            return;
        }
        _soundToggleSlider.OnToggleValueChanged -= listener.HandleSoundToggleValueChanged;
        _musicToggleSlider.OnToggleValueChanged -= listener.HandleMusicToggleValueChanged;
        _vibrationToggleSlider.OnToggleValueChanged -= listener.HandleVibrationToggleValueChanged;
        _notifToggleSlider.OnToggleValueChanged -= listener.HandleNotifToggleValueChanged;
        
        _tncButton.onClick.RemoveAllListeners();
        _privacyButton.onClick.RemoveAllListeners();
        _supportButton.onClick.RemoveAllListeners();
        _languageButton.onClick.RemoveAllListeners();
        
        base.UnregisterEventsListener(objectListener);
    }
}
