using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeScreen : UIScreen
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _addCoinsButton;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private UIHomeScreenFooterControl _footerControl;
    
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
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUIHomeScreenEventsListener listener)
            return;
        _settingsButton.onClick.RemoveAllListeners();
        _leftGrpButonClicked  -= listener.HandleLeftButtonClicked;
        _shopGrpButonClicked  -= listener.HandleShopButtonClicked;
        _homeGrpButonClicked  -= listener.HandleHomeButtonClicked;
        _mapGrpButonClicked   -= listener.HandleMapButtonClicked;
        _rightGrpButonClicked -= listener.HandleRightButtonClicked;
        _addCoinsButton.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        _footerControl.OnFooterButtonGrpClicked += HandleFooterButtonClicked;
    }
    
    private void OnDisable()
    {
        _footerControl.OnFooterButtonGrpClicked -= HandleFooterButtonClicked;
    }

    public void SetCoinsText(string text)
    {
        _coinsText.text = text;
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
