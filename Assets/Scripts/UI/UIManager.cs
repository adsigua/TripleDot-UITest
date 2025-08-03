using System.Collections.Generic;
using AdoTrpDotTest.Audio;
using NaughtyAttributes;
using UnityEngine;

namespace AdoTrpDotTest.UI
{
    public class UIManager : MonoBehaviour, IUIHomeScreenEventsListener, IUISettingsScreenEventsListener, IUIPopupScreenEventsListener, IUILevelCompleteEventsListener
    {
        private enum UIScreenType
        {
            Home, EndGame, Popup, Settings, TermsAndConditions, AdPopup, Privacy, Language
        }
    
        [SerializeField] private RectTransform _rectTransform;    
        [SerializeField] private InitGameDataSO _initGameDataSO;
        [SerializeField] private List<UIScreen> _uiScreenPrefabs = new List<UIScreen>();
    
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private Camera _mainCamera;

        [SerializeField, ReadOnly] private bool _vibrationOn, _notifsOn;
        [SerializeField, ReadOnly] private int _currentCoins;
    
        private readonly Dictionary<UIScreenType, UIScreen> _uiScreens = new();
        private readonly  Stack<UIPopupScreen> _popupScreenStack = new();
    
        private UISettingsScreenInitData _currentSettingsData;

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
            if (_uiScreens.TryGetValue(uiScreenType, out var screen)) return screen as T;
            
            screen = Instantiate(_uiScreenPrefabs[(int)uiScreenType], _rectTransform, false);
            _uiScreens.Add(uiScreenType, screen);
            return screen as T;
        }

        #region  HomeScreenEvents
        public void HandleSettingsButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK);
            OpenPopupBackgroundScreen();
            OpenSettingsScreen();
        }

        private void OpenPopupBackgroundScreen()
        {
            var backgroundScreen = GetUIScreen<UIPopupBackgroundScreen>(UIScreenType.Popup);
            backgroundScreen.transform.SetAsLastSibling();
            InitBackgroundScreen(backgroundScreen);
            backgroundScreen.ShowScreen();
        }

        private void InitBackgroundScreen(UIPopupBackgroundScreen backgroundScreen)
        {
            if (!backgroundScreen.blurBackground)
                return;
            backgroundScreen.InitTexture();
            var currRT = RenderTexture.active;
            RenderTexture.active = backgroundScreen.screenCapTexture;
            _mainCamera.targetTexture = backgroundScreen.screenCapTexture;
            _mainCamera.Render();
            _mainCamera.targetTexture = null;
            RenderTexture.active = currRT;
            backgroundScreen.BlurTexture();
        }

        private void ClosePopupBaseScreen()
        {
            var popupScreen = GetUIScreen<UIPopupBackgroundScreen>(UIScreenType.Popup);
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
            //settingsScreen.RegisterEventsListener(this);
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
                    OpenEndGameScreen();
                }
            }
        }

        private void OpenEndGameScreen()
        {
            var levelCompleteScreen = GetUIScreen<UILevelCompleteScreen>(UIScreenType.EndGame);
            var levelCompleteInitData = new UILevelCompleteScreenInitData();
            if (_initGameDataSO != null)
            {
                levelCompleteInitData.starCount = _initGameDataSO.rewardStarCount;
                levelCompleteInitData.coinCount = _initGameDataSO.rewardCoinCount;
                levelCompleteInitData.crownCount = _initGameDataSO.rewardCrownCount;
            }
            else
            {
                levelCompleteInitData.starCount = 20;
                levelCompleteInitData.coinCount = 100;
                levelCompleteInitData.crownCount = 8;
            }
            levelCompleteScreen.InitScreen(levelCompleteInitData);
            levelCompleteScreen.RegisterEventsListener(this);
            levelCompleteScreen.ShowScreen();
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
    
        public void HandleToggleFooterButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK);
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
            _audioManager.PlaySFXOneShot(SFXType.CLICK);
            var genericScreen = GetUIScreen<UIPopupGenericScreen>(UIScreenType.Language);
            OpenPopupScreen(genericScreen);
        }

        public void HandleTermsAndConditionsButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            var tncScreen = GetUIScreen<UIPopupGenericScreen>(UIScreenType.TermsAndConditions);
            OpenPopupScreen(tncScreen);
        }

        public void HandlePrivacyButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            var genericScreen = GetUIScreen<UIPopupGenericScreen>(UIScreenType.Privacy);
            OpenPopupScreen(genericScreen);
        }

        public void HandleSupportButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            Application.OpenURL("https://gentlepenguin.com/");
        }

        public void HandleCloseButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK);
            CloseCurrentPopupScreen();
        }

        public void OpenPopupScreen(UIPopupScreen newPopupScreen)
        {
            var popupScreenBase = GetUIScreen<UIPopupBackgroundScreen>(UIScreenType.Popup);
            if (!popupScreenBase.isShown)
            {
                OpenPopupBackgroundScreen();
            }
        
            //hide current popupscreen
            if (_popupScreenStack.Count > 0)
            {
                var currentPopupScreen = _popupScreenStack.Peek();
                currentPopupScreen.UnregisterEventsListener(this);
                currentPopupScreen.HideScreen(false);
            }
        
            _popupScreenStack.Push(newPopupScreen);
            newPopupScreen.RegisterEventsListener(this);
            newPopupScreen.ShowScreen();
            newPopupScreen.transform.SetAsLastSibling();
        }
    
        public void CloseCurrentPopupScreen()
        {
            var popupScreen = _popupScreenStack.Pop();
            popupScreen.UnregisterEventsListener(this);
            popupScreen.HideScreen(true, () =>
            {
                if (_popupScreenStack.Count > 0)
                {
                    var nextPopupScreen = _popupScreenStack.Peek();
                    nextPopupScreen.RegisterEventsListener(this);
                    nextPopupScreen.ShowScreen(false);
                }else if (_popupScreenStack.Count == 0)
                {
                    ClosePopupBaseScreen();
                }
            });
        }
    
        #endregion

        #region Level Complete Screen
        public void HandleLevelCompleteHomeButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            var levelCompleteScreen = GetUIScreen<UILevelCompleteScreen>(UIScreenType.EndGame);
            levelCompleteScreen.UnregisterEventsListener(this);
            levelCompleteScreen.HideScreen();
        }

        public void HandleLevelCompletePlayAdButtonClicked()
        {
            _audioManager.PlaySFXOneShot(SFXType.CLICK_BLOOP);
            var genericPopup = GetUIScreen<UIPopupGenericScreen>(UIScreenType.AdPopup) as UIPopupScreen;
            if(genericPopup == null)
                return;
            OpenPopupScreen(genericPopup);
        }
    
        #endregion
    }
}

