using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

public class UICustomToggleSlider : MonoBehaviour
{
    [SerializeField] private Button _sliderButton;
    [SerializeField] private RectTransform _handleRectTransform;
    [SerializeField] private Image _sliderFillImage;    
    [SerializeField] private Color _toggleOnColor;    
    [SerializeField] private Color _toggleOffColor;    
    [SerializeField] private RectTransform _handleOnRectTransform;
    [SerializeField] private RectTransform _handleOffRectTransform;

    [SerializeField] private bool _isOn = false;
    [SerializeField] private bool _animate = false;

    [SerializeField, ShowIf("_animate")] 
    private float _slideAnimationDuration = 0.5f;
    
    private Tween _slideTween;
    private float _tweenTime = 0;
    
    public event System.Action<bool> OnToggleValueChanged;

    private void Start()
    {
        _tweenTime = 0;
        //UpdateSliderButton(true);
    }
    
    private void OnEnable()
    {
        _sliderButton.onClick.AddListener(HandleSliderButtonClick);
    }

    private void OnDisable()
    {
        _sliderButton.onClick.RemoveListener(HandleSliderButtonClick);
    }

    private void HandleSliderButtonClick()
    {
        _isOn = !_isOn;
        UpdateSliderButton();
        OnToggleValueChanged?.Invoke(_isOn);
    }

    public void SetToggleState(bool isOn, bool doInstant = false)
    {
        _isOn = isOn;
        UpdateSliderButton(doInstant);
    }

    private void UpdateSliderButton(bool doInstant = false)
    {
        if (!_animate || doInstant)
        {
            _sliderFillImage.color = _isOn ? _toggleOnColor : _toggleOffColor;
            _handleRectTransform.anchoredPosition = _isOn ? _handleOnRectTransform.anchoredPosition : _handleOffRectTransform.anchoredPosition;
        }
        else
        {
            if(_slideTween is { active: true })
                _slideTween.Kill();
            //_tweenTime = _isOn ? Mathf.Max(0.0f, _tweenTime) : Mathf.Min(_tweenTime, 1.0f);
            float targetValue = _isOn ? 1.0f : 0.0f;
            float animDuration = (Mathf.Abs(targetValue - _tweenTime)) * _slideAnimationDuration;
            _slideTween = DOTween.To(() => _tweenTime, x =>
            {
                _tweenTime = x;
                _sliderFillImage.color = Color.Lerp(_toggleOffColor, _toggleOnColor, x);
                _handleRectTransform.anchoredPosition = Vector2.Lerp(_handleOffRectTransform.anchoredPosition, _handleOnRectTransform.anchoredPosition, x);
            }, targetValue, animDuration);
        }
    }
}
