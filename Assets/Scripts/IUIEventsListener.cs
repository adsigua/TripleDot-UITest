using UnityEngine;

public interface IUIHomeScreenEventsListener
{
    public void HandleSettingsButtonClicked();
    public void HandleAddMoneyButtonClicked();
    public void HandleHomeButtonClicked();
    public void HandleShopButtonClicked();
    public void HandleMapButtonClicked();
    public void HandleLeftButtonClicked();
    public void HandleRightButtonClicked();
}

public interface IUISettingsScreenEventsListener
{
    public void HandleSoundToggleValueChanged(bool isOn);
    public void HandleMusicToggleValueChanged(bool isOn);
    public void HandleVibrationToggleValueChanged(bool isOn);
    public void HandleNotifToggleValueChanged(bool isOn);
    public void HandleLanguageButtonClicked();
    public void HandleTermsAndConditionsButtonClicked();
    public void HandlePrivacyButtonClicked();
    public void HandleSupportButtonClicked();
    public void HandleCloseButtonClicked();
}
