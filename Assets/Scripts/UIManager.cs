using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IUIHomeScreenEventsListener, IUISettingsScreenEventsListener
{
    private enum UIScreenType
    {
        Home, EndGame, Popup, Settings
    }
    
    [SerializeField] private RectTransform _rectTransform;    
    [SerializeField] private List<UIScreen> _uiScreenPrefabs = new List<UIScreen>();
    [SerializeField] private InitGameDataSO _initGameDataSO;
    
    [SerializeField] private AudioManager _audioManager;

    [SerializeField] private bool _vibrationOn, _notifsOn;
    
    private readonly Dictionary<UIScreenType, UIScreen> _uiScreens = new();
    private readonly  Stack<UIScreen> _popupScreenStack = new();
    
    private UISettingsScreenInitData _currentSettingsData;

    private void Start()
    {
        _uiScreens.Clear();
        _popupScreenStack.Clear();
        
        var homeScreen = GetUIScreen(UIScreenType.Home);
        var homeScreenData = new UIHomeScreenInitData();
        if (_initGameDataSO != null)
        {
            homeScreenData.SetDataFromSO(_initGameDataSO);
            _vibrationOn = _initGameDataSO.vibrationOn;
            _notifsOn = _initGameDataSO.notifsOn;
        }
        else
        {
            homeScreenData.InitData();
            _vibrationOn = true;
            _notifsOn = true;
        }
        homeScreen.InitScreen(homeScreenData);
        homeScreen.ShowScreen();
    }
    
    private UIScreen GetUIScreen(UIScreenType uiScreenType)
    {
        if (_uiScreens.TryGetValue(uiScreenType, out var screen))
        {
            return screen;
        }
        else
        {
            screen = Instantiate(_uiScreenPrefabs[(int)uiScreenType], _rectTransform, false);
            _uiScreens.Add(uiScreenType, screen);
        }
        screen.RegisterEventsListener(this);
        return screen;
    }

#region  HomeScreenEvents
    public void HandleSettingsButtonClicked()
    {
        OpenPopupScreen();
        OpenSettingsScreen();
    }

    private void OpenPopupScreen()
    {
        var popupScreen = GetUIScreen(UIScreenType.Popup);
        popupScreen.ShowScreen();
    }

    private void ClosePopupScreen()
    {
        var popupScreen = GetUIScreen(UIScreenType.Popup);
        popupScreen.UnregisterEventsListener(this);
        popupScreen.HideScreen();
    }

    private void OpenSettingsScreen()
    {
        var settingsScreen = GetUIScreen(UIScreenType.Settings);
        _currentSettingsData ??= new UISettingsScreenInitData
        {
            soundOn = _audioManager.IsSoundOn(),
            musicOn = _audioManager.IsMusicOn(),
            vibrationOn = _vibrationOn,
            notifsOn = _notifsOn,
            languageIndex = 0
        };
        settingsScreen.InitScreen(_currentSettingsData);
        settingsScreen.ShowScreen();
    }

    private void CloseSettingsScreen()
    {
        var settingsScreen = GetUIScreen(UIScreenType.Settings);
        settingsScreen.UnregisterEventsListener(this);
        settingsScreen.HideScreen();
    }

    public void HandleAddMoneyButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleHomeButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleShopButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleMapButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleLeftButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleRightButtonClicked()
    {
        throw new System.NotImplementedException();
    }
#endregion
    
#region SettingsScreen Events
    public void HandleSoundToggleValueChanged(bool isOn)
    {
        _audioManager.ToggleSound(isOn);
    }

    public void HandleMusicToggleValueChanged(bool isOn)
    {
        _audioManager.ToggleMusic(isOn);
    }

    public void HandleVibrationToggleValueChanged(bool isOn)
    {
        _vibrationOn = isOn;
        Debug.LogWarning($"Vibration was changed to {isOn}");
    }

    public void HandleNotifToggleValueChanged(bool isOn)
    {
        _notifsOn = isOn;
        Debug.LogWarning($"Notif was changed to {isOn}");
    }

    public void HandleLanguageButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleTermsAndConditionsButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandlePrivacyButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleSupportButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleCloseButtonClicked()
    {
        CloseSettingsScreen();
        ClosePopupScreen();
    }
#endregion
}

