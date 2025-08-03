namespace AdoTrpDotTest.UI
{
    public interface IUIHomeScreenEventsListener
    {
        public void HandleSettingsButtonClicked();
        public void HandleAddCoinsButtonClicked();
        public void HandleHomeButtonClicked(bool isLocked, bool isSelected);
        public void HandleShopButtonClicked(bool isLocked, bool isSelected);
        public void HandleMapButtonClicked(bool isLocked, bool isSelected);
        public void HandleLeftButtonClicked(bool isLocked, bool isSelected);
        public void HandleRightButtonClicked(bool isLocked, bool isSelected);
        public void HandleToggleFooterButtonClicked();
    }

    public interface IUIPopupScreenEventsListener
    {
        public void HandleCloseButtonClicked();
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
    }

    public interface IUILevelCompleteEventsListener
    {
        public void HandleLevelCompleteHomeButtonClicked();
        public void HandleLevelCompletePlayAdButtonClicked();
    }
}