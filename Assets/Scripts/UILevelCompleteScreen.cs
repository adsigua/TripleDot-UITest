using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UILevelCompleteScreen : UIScreen
{
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _playAdButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private UIAnimatedElement _starCountElement;
    [SerializeField] private UIAnimatedElement _coinCountElement;
    [SerializeField] private UIAnimatedElement _crownCountElement;

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(HandleRestartButtonClick);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(HandleRestartButtonClick);
    }
    
    public override void InitScreen(UIScreenInitData uiScreenInitData)
    {
        base.InitScreen(uiScreenInitData);
        if(uiScreenInitData is not UILevelCompleteScreenInitData levelCompleteInitData)
            return;
        
        _starCountElement.SetTargetCount(levelCompleteInitData.starCount);
        _coinCountElement.SetTargetCount(levelCompleteInitData.coinCount);
        _crownCountElement.SetTargetCount(levelCompleteInitData.crownCount);
    }

    private void HandleRestartButtonClick()
    {
        TriggerCustomEventByIndex(1);
        PlayEntryAnimation();
    }

    public override void RegisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUILevelCompleteEventsListener listener)
            return;
        _homeButton.onClick.AddListener(listener.HandleLevelCompleteHomeButtonClicked);
        _playAdButton.onClick.AddListener(listener.HandleLevelCompletePlayAdButtonClicked);
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUILevelCompleteEventsListener listener)
            return;
        _homeButton.onClick.RemoveListener(listener.HandleLevelCompleteHomeButtonClicked);
        _playAdButton.onClick.RemoveListener(listener.HandleLevelCompletePlayAdButtonClicked);
    }
}
