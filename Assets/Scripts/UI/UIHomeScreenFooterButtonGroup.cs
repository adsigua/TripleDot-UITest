using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdoTrpDotTest.UI
{
    public class UIHomeScreenFooterButtonGroup : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _grpButton;
        [SerializeField] private RectTransform _iconRectTransform;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _lockImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _grpText;
        [SerializeField] private float _lockShakeAnimationDuration = 0.4f;

        [SerializeField] private bool _animate = false;
    
        [SerializeField, ShowIf("_animate")] 
        private float _animationDuration = 0.4f;
        [SerializeField, ShowIf("_animate")] 
        private AnimationCurve _animationCuve;
    
        [Header("rect data")]
        [SerializeField] private Vector2 _activeGrpDimensions;
        [SerializeField] private Vector2 _inactiveGrpDimensions;
    
        [SerializeField] private Vector2 _activeGrpIconXAnchorMinMax;
        [SerializeField] private Vector2 _inactiveGrpIconXAnchorMinMax;
    
        [SerializeField] private Vector4 _activeGrpIconRect;
        [SerializeField] private Vector4 _inactiveGrpIconRect;

        public event System.Action<UIHomeScreenFooterButtonGroup> OnButtonGroupClicked;
        public bool isAnimating => _selectionTween is { active: true };

        private Tween _selectionTween;
        private float _tweenTime;
        private Tween _lockShakeTween;
        private bool _isSelected = false;
        private bool _isLocked = true;

        public bool isSelected => _isSelected;
        public bool isLocked => _isLocked;
    
        public void SetLockState(bool setLock)
        {
            _isLocked = setLock;
            _grpButton.targetGraphic = setLock ? _lockImage : _iconImage;
            _lockImage.gameObject.SetActive(setLock);
            _iconImage.gameObject.SetActive(!setLock);
        }

        public void SetSelected(bool setSelected, bool doInstant = false)
        {
            if(_lockShakeTween is { active: true })
                _lockShakeTween.Kill();
            _isSelected = setSelected;
            float targetValue = _isSelected ? 1.0f : 0.0f;
            if (!_animate || doInstant)
            {
                UpdateGroupRect(targetValue);
            }
            else
            {
                if(_selectionTween is { active: true })
                    _selectionTween.Kill();
            
                float animDuration = (Mathf.Abs(targetValue - _tweenTime)) * _animationDuration;
                _selectionTween = DOTween.To(() => _tweenTime, UpdateAnimationState, targetValue, animDuration);
            }
        }

        private void UpdateAnimationState(float tm)
        {
            _tweenTime = tm;
            float smoothTm = _animationCuve.Evaluate(tm);
            UpdateGroupRect(smoothTm);
        }

        private void UpdateGroupRect(float tm)
        {
            var newSizeVec2 = Vector2.Lerp(_inactiveGrpDimensions, _activeGrpDimensions, tm);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSizeVec2.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSizeVec2.y);
        
            _backgroundImage.rectTransform.localScale = Vector3.one;
        
            var newAnchor = Vector2.Lerp(new Vector2(-1, 0), new Vector2(0, 1), tm);
            _backgroundImage.rectTransform.anchorMin = new Vector2(_backgroundImage.rectTransform.anchorMin.x, newAnchor.x);
            _backgroundImage.rectTransform.anchorMax = new Vector2(_backgroundImage.rectTransform.anchorMax.x, newAnchor.y);
        
            _grpText.rectTransform.localScale = Vector3.one * tm;
        }


        public void OnEnable()
        {
            _grpButton.onClick.AddListener(HandleButtonClick);
        }

        public void OnDisable()
        {
            _grpButton.onClick.RemoveAllListeners();
        }

        private void HandleButtonClick()
        {
            if (_isLocked)
            {
                //do lock anim
                if (_lockShakeTween is { active: true })
                {
                    _lockShakeTween.Restart();
                }
                else
                {
                    _lockShakeTween = _lockImage.rectTransform.DOShakeRotation(
                        _lockShakeAnimationDuration,
                        Vector3.forward * 20.0f,   //strength
                        8,
                        50.0f,
                        false,
                        ShakeRandomnessMode.Harmonic
                    );
                }
            }
            OnButtonGroupClicked?.Invoke(this);
        }
    
    }
}
