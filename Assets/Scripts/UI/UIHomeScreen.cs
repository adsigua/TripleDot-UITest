using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdoTrpDotTest.UI
{
    public class UIHomeScreen : UIScreen
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _addCoinsButton;
        [SerializeField] private TextMeshProUGUI _coinsTMProGUI;
        [SerializeField] private TextMeshProUGUI _livesTMProGUI;
        [SerializeField] private TextMeshProUGUI _starTMProGUI;
        [SerializeField] private UIHomeScreenFooterControl _footerControl;
        [SerializeField] private Button _toggleFooterButton;
        [SerializeField] private Sprite _toggleFooterUpIcon;
        [SerializeField] private Sprite _toggleFooterDownIcon;
        [SerializeField] private UIAnimatedElement _footerBarElement;
        private event System.Action<bool,bool> _leftGrpButonClicked;
        private event System.Action<bool,bool> _shopGrpButonClicked;
        private event System.Action<bool,bool> _homeGrpButonClicked;
        private event System.Action<bool,bool> _mapGrpButonClicked;
        private event System.Action<bool,bool> _rightGrpButonClicked;
    
        public override void InitScreen(UIScreenInitData uiScreenInitData)
        {
            base.InitScreen(uiScreenInitData);
            if(uiScreenInitData is not UIHomeScreenInitData homeScreenInitData)
                return;
            _footerControl.SetAllLockState(homeScreenInitData.footerButtonsLockStates);
            _coinsTMProGUI.text = homeScreenInitData.coinsCount.ToString();
            _livesTMProGUI.text = homeScreenInitData.livesCount.ToString();
            _starTMProGUI.text = homeScreenInitData.starCount.ToString();
        }
    
        public override void RegisterEventsListener<T>(T objectListener)
        {
            if(objectListener is not IUIHomeScreenEventsListener listener)
                return;
            _settingsButton.onClick.AddListener(listener.HandleSettingsButtonClicked);
            _leftGrpButonClicked  += listener.HandleLeftButtonClicked;
            _shopGrpButonClicked  += listener.HandleShopButtonClicked;
            _homeGrpButonClicked  += listener.HandleHomeButtonClicked;
            _mapGrpButonClicked   += listener.HandleMapButtonClicked;
            _rightGrpButonClicked += listener.HandleRightButtonClicked;
            _addCoinsButton.onClick.AddListener(listener.HandleAddCoinsButtonClicked);
            _toggleFooterButton.onClick.AddListener(listener.HandleToggleFooterButtonClicked);
        }

        public override void UnregisterEventsListener<T>(T objectListener)
        {
            if(objectListener is not IUIHomeScreenEventsListener listener)
                return;
            _settingsButton.onClick.RemoveListener(listener.HandleSettingsButtonClicked);
            _leftGrpButonClicked  -= listener.HandleLeftButtonClicked;
            _shopGrpButonClicked  -= listener.HandleShopButtonClicked;
            _homeGrpButonClicked  -= listener.HandleHomeButtonClicked;
            _mapGrpButonClicked   -= listener.HandleMapButtonClicked;
            _rightGrpButonClicked -= listener.HandleRightButtonClicked;
            _addCoinsButton.onClick.RemoveListener(listener.HandleAddCoinsButtonClicked);
            _toggleFooterButton.onClick.RemoveListener(listener.HandleToggleFooterButtonClicked);
        }

        private void OnEnable()
        {
            _footerControl.OnFooterButtonGrpClicked += HandleFooterButtonClicked;
            _toggleFooterButton.onClick.AddListener(HandleToggleButtonClick);
        }
    
        private void OnDisable()
        {
            _footerControl.OnFooterButtonGrpClicked -= HandleFooterButtonClicked;
            _toggleFooterButton.onClick.RemoveListener(HandleToggleButtonClick);
        }

        public override void ShowScreen(bool doAnimation = true, System.Action onComplete = null)
        {
            base.ShowScreen(doAnimation, onComplete);
            _footerBarElement.InitElementState();
            _footerBarElement.PlayShowAnimation();
            _toggleFooterButton.image.sprite = _toggleFooterDownIcon;
        }
    
        private void HandleToggleButtonClick()
        {
            if (_footerBarElement.isShown)
            {
                _footerControl.ClearFooterSelection();
                _footerBarElement.PlayExitAnimation();
                _toggleFooterButton.image.sprite = _toggleFooterUpIcon;
            }
            else
            {
                _footerBarElement.PlayShowAnimation();
                _toggleFooterButton.image.sprite = _toggleFooterDownIcon;
            }
        }

        public void SetCoinsText(string text)
        {
            _coinsTMProGUI.text = text;
        }

        private void HandleFooterButtonClicked(int index, bool isLocked, bool isActive)
        {
            switch (index)
            {
                default:
                    _homeGrpButonClicked?.Invoke(isLocked, isActive);
                    break;
                case 0:
                    _leftGrpButonClicked?.Invoke(isLocked, isActive);
                    break;
                case 1:
                    _shopGrpButonClicked?.Invoke(isLocked, isActive);
                    break;
                case 3:
                    _mapGrpButonClicked?.Invoke(isLocked, isActive);
                    break;
                case 4:
                    _rightGrpButonClicked?.Invoke(isLocked, isActive);
                    break;
            }
        }
    }
}
