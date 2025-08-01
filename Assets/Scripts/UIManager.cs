using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class UIManager : MonoBehaviour, IUIHomeScreenEventsListener, IUISettingsScreenEventsListener, IUIPopupScreenEventsListener
{
    private enum UIScreenType
    {
        Home, EndGame, Popup, Settings, TermsAndConditions
    }
    
    [SerializeField] private RectTransform _rectTransform;    
    [SerializeField] private List<UIScreen> _uiScreenPrefabs = new List<UIScreen>();
    [SerializeField] private InitGameDataSO _initGameDataSO;
    
    [SerializeField] private AudioManager _audioManager;

    [SerializeField] private bool _vibrationOn, _notifsOn;
    
    private readonly Dictionary<UIScreenType, UIScreen> _uiScreens = new();
    private readonly  Stack<UIPopupScreen> _popupScreenStack = new();
    
    private UISettingsScreenInitData _currentSettingsData;

    [SerializeField, ReadOnly] private int _currentCoins;

    private void Start()
    {
        _uiScreens.Clear();
        _popupScreenStack.Clear();
        
        var homeScreen = GetUIScreen<UIHomeScreen>(UIScreenType.Home);
        var homeScreenData = new UIHomeScreenInitData();
        if (_initGameDataSO != null)
        {
            homeScreenData.SetDataFromSO(_initGameDataSO);
            _vibrationOn = _initGameDataSO.vibrationOn;
            _notifsOn = _initGameDataSO.notifsOn;
            _currentCoins = _initGameDataSO.coinsCount;
        }
        else
        {
            homeScreenData.InitData();
            _vibrationOn = true;
            _notifsOn = true;
            _currentCoins = 300;
        }
        homeScreen.InitScreen(homeScreenData);
        homeScreen.RegisterEventsListener(this);
        homeScreen.ShowScreen();
    }
    
    private T GetUIScreen<T>(UIScreenType uiScreenType) where T : UIScreen
    {
        if (!_uiScreens.TryGetValue(uiScreenType, out var screen))
        {
            screen = Instantiate(_uiScreenPrefabs[(int)uiScreenType], _rectTransform, false);
            _uiScreens.Add(uiScreenType, screen);
        }
        return screen as T;
    }

#region  HomeScreenEvents
    public void HandleSettingsButtonClicked()
    {
        _audioManager.PlaySFXOneShot(SFXType.CLICK);
        OpenPopupBaseScreen();
        OpenSettingsScreen();
    }

    private void OpenPopupBaseScreen()
    {
        var popupScreen = GetUIScreen<UIPopupBaseScreen>(UIScreenType.Popup);
        popupScreen.ShowScreen();
    }

    private void ClosePopupBaseScreen()
    {
        var popupScreen = GetUIScreen<UIPopupBaseScreen>(UIScreenType.Popup);
        popupScreen.UnregisterEventsListener(this);
        popupScreen.RegisterEventsListener(this);
        popupScreen.HideScreen();
    }

    private void OpenSettingsScreen()
    {
        var settingsScreen = GetUIScreen<UISettingsScreen>(UIScreenType.Settings);
        if(settingsScreen == null)
            return;
        _currentSettingsData ??= new UISettingsScreenInitData();
        _currentSettingsData.soundOn = _audioManager.IsSoundOn();
        _currentSettingsData.musicOn = _audioManager.IsMusicOn();
        _currentSettingsData.vibrationOn = _vibrationOn;
        _currentSettingsData.notifsOn = _notifsOn;
        _currentSettingsData.languageIndex = 0;
        
        settingsScreen.InitScreen(_currentSettingsData);
        settingsScreen.RegisterEventsListener(this);
        
        OpenPopupScreen(settingsScreen);
    }
    
    public void HandleAddCoinsButtonClicked()
    {
        _audioManager.PlaySFXOneShot(SFXType.COIN);
        _currentCoins += 10;
        var homeScreen = GetUIScreen<UIHomeScreen>(UIScreenType.Home);
        homeScreen.SetCoinsText(_currentCoins.ToString());
    }

    public void HandleHomeButtonClicked(bool isLocked, bool isSelected)
    {
        if (isLocked)
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_ERR);
        }
        else
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            if (isSelected)
            {
                Debug.LogWarning("Clicked Selected Home!");
            }
        }
    }

    public void HandleShopButtonClicked(bool isLocked, bool isSelected)
    {
        if (isLocked)
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_ERR);
        }
        else
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            if (isSelected)
            {
                Debug.LogWarning("Clicked Selected Shop!");
            }
        }
    }

    public void HandleMapButtonClicked(bool isLocked, bool isSelected)
    {
        if (isLocked)
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_ERR);
        }
        else
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            if (isSelected)
            {
                Debug.LogWarning("Clicked Selected Map!");
            }
        }
    }

    public void HandleLeftButtonClicked(bool isLocked, bool isSelected)
    {
        if (isLocked)
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_ERR);
        }
        else
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            if (isSelected)
            {
                Debug.LogWarning("Clicked Selected Left!");
            }
        }
    }

    public void HandleRightButtonClicked(bool isLocked, bool isSelected)
    {
        if (isLocked)
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_ERR);
        }
        else
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            if (isSelected)
            {
                Debug.LogWarning("Clicked Selected Right!");
            }
        }
    }
#endregion
    
#region SettingsScreen Events
    public void HandleSoundToggleValueChanged(bool isOn)
    {
        _audioManager.PlaySFXOneShot(isOn ? SFXType.TOGGLE_ON : SFXType.TOGGLE_OFF);
        _audioManager.ToggleSound(isOn);
    }

    public void HandleMusicToggleValueChanged(bool isOn)
    {
        _audioManager.PlaySFXOneShot(isOn ? SFXType.TOGGLE_ON : SFXType.TOGGLE_OFF);
        _audioManager.ToggleMusic(isOn);
    }

    public void HandleVibrationToggleValueChanged(bool isOn)
    {
        _vibrationOn = isOn;
        _audioManager.PlaySFXOneShot(isOn ? SFXType.TOGGLE_ON : SFXType.TOGGLE_OFF);
        Debug.LogWarning($"Vibration was changed to {isOn}");
    }

    public void HandleNotifToggleValueChanged(bool isOn)
    {
        _notifsOn = isOn;
        _audioManager.PlaySFXOneShot(isOn ? SFXType.TOGGLE_ON : SFXType.TOGGLE_OFF);
        Debug.LogWarning($"Notif was changed to {isOn}");
    }

    public void HandleLanguageButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public void HandleTermsAndConditionsButtonClicked()
    {
        _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
        var tncScreen = GetUIScreen<UITermsAndConditionsScreen>(UIScreenType.TermsAndConditions) as UIPopupScreen;
        if(tncScreen == null)
            return;
        tncScreen.RegisterEventsListener(this);
        OpenPopupScreen(tncScreen);
    }

    public void HandlePrivacyButtonClicked()
    {
        _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
        throw new System.NotImplementedException();
    }

    public void HandleSupportButtonClicked()
    {
        _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
        throw new System.NotImplementedException();
    }

    public void HandleCloseButtonClicked()
    {
        _audioManager.PlaySFXOneShot(SFXType.CLICK);
        CloseCurrentPopupScreen();
    }

    public void OpenPopupScreen(UIPopupScreen newPopupScreen)
    {
        var popupScreenBase = GetUIScreen<UIPopupBaseScreen>(UIScreenType.Popup);
        if (!popupScreenBase.isShown)
        {
            OpenPopupBaseScreen();
        }
        
        //hide current popupscreen
        if (_popupScreenStack.Count > 0)
        {
            var currentPopupScreen = _popupScreenStack.Peek();
            currentPopupScreen.UnregisterEventsListener(this);
            currentPopupScreen.HideScreen();
        }
        
        _popupScreenStack.Push(newPopupScreen);
        newPopupScreen.ShowScreen();
    }
    
    public void CloseCurrentPopupScreen()
    {
        var popupScreen = _popupScreenStack.Pop();
        popupScreen.UnregisterEventsListener(this);
        popupScreen.HideScreen();

        if (_popupScreenStack.Count > 0)
        {
            var nextPopupScreen = _popupScreenStack.Peek();
            nextPopupScreen.RegisterEventsListener(this);
            nextPopupScreen.ShowScreen();
        }else if (_popupScreenStack.Count == 0)
        {
            ClosePopupBaseScreen();
        }
    }
    
#endregion
}

