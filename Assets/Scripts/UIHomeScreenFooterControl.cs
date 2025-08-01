using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIHomeScreenFooterControl : MonoBehaviour
{
    [SerializeField] private List<UIHomeScreenFooterButtonGroup> _footerButtonGroups;
    
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _layoutGroupRectTransform;
    [SerializeField] private bool _animate = false;

    private UIHomeScreenFooterButtonGroup _currentButtonGroup = null;
    private UIHomeScreenFooterButtonGroup _prevButtonGroup = null;

    //index, isLocked, isSelected
    public event Action<int, bool, bool> OnFooterButtonGrpClicked;
    // public event Action OnLeftActiveButtonClicked;
    // public event Action OnShopActiveButtonClicked;
    // public event Action OnHomeActiveButtonClicked;
    // public event Action OnMapActiveButtonClicked;
    // public event Action OnRightActiveButtonClicked;
    
    private void Start()
    {
        _currentButtonGroup = null;
        _prevButtonGroup = null;
    }
    
    private void OnEnable()
    {
        foreach (var uiHomeScreenFooterControl in _footerButtonGroups)
        {
            uiHomeScreenFooterControl.OnButtonGroupClicked += HandleFooterButtonClick;
        }
    }
    
    private void OnDisable()
    {
        foreach (var uiHomeScreenFooterControl in _footerButtonGroups)
        {
            uiHomeScreenFooterControl.OnButtonGroupClicked -= HandleFooterButtonClick;
        }
    }

    private void LateUpdate()
    {
        if(!_animate)
            return;
        bool doForceUpdate = _currentButtonGroup != null && _currentButtonGroup.isAnimating;
        doForceUpdate |= _prevButtonGroup != null && _prevButtonGroup.isAnimating;
        if (doForceUpdate)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroupRectTransform);
        }
    }

    public void SetAllLockState(bool[] lockStates)
    {
        for (int i = 0; i < lockStates.Length; i++)
        {
            if (i >= _footerButtonGroups.Count)
                return;
            _footerButtonGroups[i].SetLockState(lockStates[i]);
        }
    }

    public void SetLockGroupLockValue(int index, bool value)
    {
        if (index >= _footerButtonGroups.Count)
            return;
        _footerButtonGroups[index].SetLockState(value);
    }
    
    private void HandleFooterButtonClick(UIHomeScreenFooterButtonGroup footerButtonGroup)
    {
        OnFooterButtonGrpClicked?.Invoke(_footerButtonGroups.IndexOf(footerButtonGroup), footerButtonGroup.isLocked, footerButtonGroup.isSelected);
        if (footerButtonGroup.isSelected || footerButtonGroup.isLocked)
            return;
        
        bool doInstant = !_animate;
        if (_currentButtonGroup != null)
        {
            _currentButtonGroup.SetSelected(false, doInstant);
            _prevButtonGroup = _currentButtonGroup;
        }

        _currentButtonGroup = footerButtonGroup;
        _currentButtonGroup.SetSelected(true, doInstant);

        if (doInstant)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroupRectTransform);
        }
    }
}
