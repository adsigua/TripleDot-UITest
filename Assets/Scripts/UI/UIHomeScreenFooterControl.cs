using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdoTrpDotTest.UI
{
    public class UIHomeScreenFooterControl : MonoBehaviour
    {
        [SerializeField] private List<UIHomeScreenFooterButtonGroup> _footerButtonGroups;
    
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _layoutGroupRectTransform;
        [SerializeField] private bool _animate = false;

        private UIHomeScreenFooterButtonGroup _currentButtonGroup = null;
        private UIHomeScreenFooterButtonGroup _prevButtonGroup = null;

        public event Action<int, bool, bool> OnFooterButtonGrpClicked;
    
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

        //for setting individual lock state
        public void SetLockGroupLockValue(int index, bool value)
        {
            if (index >= _footerButtonGroups.Count)
                return;
            _footerButtonGroups[index].SetLockState(value);
        }

        public void ClearFooterSelection()
        {
            if (_currentButtonGroup != null)
            {
                _currentButtonGroup.SetSelected(false, true);
                _prevButtonGroup = null;
                _currentButtonGroup = null;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroupRectTransform);
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
}
