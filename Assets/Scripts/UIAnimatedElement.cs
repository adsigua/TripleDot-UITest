using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIAnimatedElement : MonoBehaviour
{
    public enum ElementType
    {
        RectTransform, Image, Text, CanvasGroup
    }
    
    [Flags] public enum AnimationType
    {
        Fade = 1 << 0, 
        Slide = 1 << 1, 
        Scale = 1 << 2, 
        Rotate = 1 << 3
    }

    public enum SlideDirection
    {
        Left, Right, Up, Down
    }
    
    [SerializeField] private ElementType _elementType = ElementType.RectTransform;
    [SerializeField] private AnimationType _animationType = AnimationType.Fade;
    [SerializeField] private float _totalDelay = 0.0f;
    [SerializeField] private bool _hasExit = false;
    [SerializeField] private float _stageTransitionDuration = 0.3f;
    [SerializeField] private bool _playOnEnable = false;
    
    [SerializeField, Foldout("Fade"), ShowIf("isFading")] private float _fadeDuration = 0.5f;
    [SerializeField, Foldout("Fade"), ShowIf("isFading")] private float _fadeDelay = 0.0f;
    [SerializeField, Foldout("Fade"), ShowIf("isFading")] private bool _fadeLoop = false;
    [SerializeField, Foldout("Fade"), ShowIf("isFading")] private Vector2 _minMaxAlpha = new Vector2(0.0f,1.0f);
    [SerializeField, Foldout("Fade"), ShowIf("isFadeAndLooping")] private Vector2 _minMaxAlphaPulse = new Vector2(0.0f,1.0f);
    [SerializeField, Foldout("Fade"), ShowIf("isFading")] 
    private AnimationCurve _fadeAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [SerializeField, Foldout("Sliding"), ShowIf("isSliding")] private float _slideDuration = 0.5f;
    [SerializeField, Foldout("Sliding"), ShowIf("isSliding")] private float _slideDelay = 0.0f;
    [SerializeField, Foldout("Sliding"), ShowIf("isSliding")] private bool _slideLoop = false;
    [SerializeField, Foldout("Sliding"), ShowIf("isSliding")] private SlideDirection _slidDirection = SlideDirection.Up;
    [SerializeField, Foldout("Sliding"), ShowIf("isSliding")] private float _slideDistance = 100.0f;
    [SerializeField, Foldout("Sliding"), ShowIf("isSliding")] 
    private AnimationCurve _slideAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [SerializeField, Foldout("Scaling"), ShowIf("isScaling")] private float _scaleDuration = 0.5f;
    [SerializeField, Foldout("Scaling"), ShowIf("isScaling")] private float _scaleDelay = 0.0f;
    [SerializeField, Foldout("Scaling"), ShowIf("isScaling")] private bool _scaleLoop = false;
    [SerializeField, Foldout("Scaling"), ShowIf("isScaling")] private Vector2 _minMaxScaling = new Vector2(0.0f,1.0f);
    [SerializeField, Foldout("Scaling"), ShowIf("isScaleAndLooping")] private Vector2 _minMaxScalingPulse = new Vector2(0.0f,1.0f);
    [SerializeField, Foldout("Scaling"), ShowIf("isScaling")] 
    private AnimationCurve _scaleAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [SerializeField, Foldout("Rotation"), ShowIf("isRotating")] private float _rotateDuration = 0.5f;
    [SerializeField, Foldout("Rotation"), ShowIf("isRotating")] private float _rotateDelay = 0.0f;
    [SerializeField, Foldout("Rotation"), ShowIf("isRotating")] private bool _rotateLoop = false;
    [SerializeField, Foldout("Rotation"), ShowIf("isRotating")] private Vector2 _minMaxRotAngle = new Vector2(30,40);
    [SerializeField, Foldout("Rotation"), ShowIf("isRotAndLooping")] private Vector2 _minMaxRotSpeed = new Vector2(30,40);
    [SerializeField, Foldout("Rotation"), ShowIf("isRotating")] 
    private AnimationCurve _rotateAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [SerializeField, Foldout("Text"), ShowIf("isText")] private int _targetCount;
    [SerializeField, Foldout("Text"), ShowIf("isText")] private float _textDelay = 0.0f;
    [SerializeField, Foldout("Text"), ShowIf("isText")] private float _textDuration = 10.0f;

    [SerializeField] private List<UnityAction> _onCompleteShowActions;

    private bool isSliding => (_animationType & AnimationType.Slide) != 0;
    private bool isSlideAndLooping => isSliding && _slideLoop;
    private bool isRotating => (_animationType & AnimationType.Rotate) != 0;
    private bool isRotAndLooping => isRotating && _rotateLoop;
    private bool isScaling => (_animationType & AnimationType.Scale) != 0;
    private bool isScaleAndLooping => isScaling && _scaleLoop;
    private bool isFading => (_animationType & AnimationType.Fade) != 0;
    private bool isFadeAndLooping => isFading && _fadeLoop;
    
    private bool isText => _elementType == ElementType.Text;
    
    private int _currentCount;
    private Sequence _fadeSequence;
    private Sequence _slideSequence;
    private Sequence _scaleSequence;
    private Sequence _rotateSequence;
    private Sequence _textSequence;
    
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private TextMeshProUGUI _textMeshProGUI;

    private void OnEnable()
    {
        if (_playOnEnable)
        {
            PlayShowAnimation();
        }
    }

    public void SetTargetCount(int count)
    {
        _targetCount = count;
    }

    public void PlayShowAnimation(float delay = 0)
    {
        InitSequences(delay + _totalDelay);

        switch (_elementType)
        {
            case ElementType.RectTransform:
                PlayRectTransformAnimation();
                break;
            case ElementType.Image:
                PlayImageAnimation();
                break;
            case ElementType.Text:
                DoTMProAnimations();
                break;
            case ElementType.CanvasGroup:
                PlayCanvasGroupAnimation();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InitSequences(float delay)
    {
        InitSequence(ref _fadeSequence  , isFading, delay + _fadeDelay);
        InitSequence(ref _slideSequence ,  isFading,delay + _slideDelay);
        InitSequence(ref _scaleSequence , isFading, delay + _scaleDelay);
        InitSequence(ref _rotateSequence, isFading, delay + _rotateDelay);
        InitSequence(ref _textSequence  , isFading, delay + _textDelay);
    }

    private void InitSequence(ref Sequence sequence, bool useSequence, float delay)
    {
        if (sequence is { active: true })
        {
            sequence.Kill();
        }
        if (useSequence)
            sequence ??= DOTween.Sequence();
    }

    private void PlayRectTransformAnimation()
    {
        _rectTransform ??= GetComponent<RectTransform>();

        if (isSliding)
        {
            DoRectTransformSlide(_rectTransform);
        }
        if (isScaling)
        {
            DoRectTransformScale(_rectTransform);
        }
        if (isRotating)
        {
            DoRectTransformRotate(_rectTransform);
        }
    }
    
    private void PlayImageAnimation()
    {
        _image ??= GetComponent<Image>();
        
        if (isFading)
        {
            DoImageFade(_image);
        }
        if (isSliding)
        {
            DoRectTransformSlide(_image.rectTransform);
        }
        if (isScaling)
        {
            DoRectTransformScale(_image.rectTransform);
        }
        if (isRotating)
        {
            DoRectTransformRotate(_image.rectTransform);
        }
    }
    
    private void DoTMProAnimations()
    {
        _textMeshProGUI ??= GetComponent<TextMeshProUGUI>();
        _rectTransform ??= GetComponent<RectTransform>();

        DoTextCountUpdate(_textMeshProGUI);
        
        if (isFading)
        {
            DoTMProFade(_textMeshProGUI);
        }
        if (isSliding)
        {
            DoRectTransformSlide(_textMeshProGUI.rectTransform);
        }
        if (isScaling)
        {
            DoRectTransformScale(_textMeshProGUI.rectTransform);
        }
        if (isRotating)
        {
            DoRectTransformRotate(_textMeshProGUI.rectTransform);
        }
    }

    
    private void PlayCanvasGroupAnimation()
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _rectTransform ??= GetComponent<RectTransform>();

        if (isFading)
        {
            DoCanvasGroupFade(_canvasGroup);
        }
        if (isSliding)
        {
            DoRectTransformSlide(_rectTransform);
        }
        if (isScaling)
        {
            DoRectTransformScale(_rectTransform);
        }
        if (isRotating)
        {
            DoRectTransformRotate(_rectTransform);
        }
    }

    private void DoRectTransformSlide(RectTransform rectTransform)
    {
        if (_slideLoop)
        {
            throw new ArgumentOutOfRangeException();
        }
        else
        {
            Vector2 slidOffset;
            switch (_slidDirection)
            {
                case SlideDirection.Left:
                    slidOffset = Vector2.left;
                    break;
                case SlideDirection.Right:
                    slidOffset = Vector2.right;
                    break;
                case SlideDirection.Up:
                    slidOffset = Vector2.up;
                    break;
                case SlideDirection.Down:
                    slidOffset = Vector2.down;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Vector2 targetPos = rectTransform.anchoredPosition;
            Vector2 startPos = targetPos - slidOffset * _slideDistance;
            rectTransform.anchoredPosition = startPos;
            _slideSequence.Append(rectTransform.DOAnchorPos(targetPos, _slideDuration).SetEase(_slideAnimCurve));
        }
    }
    
    
    private void DoRectTransformScale(RectTransform rectTransform)
    {
        rectTransform.localScale = Vector3.one * _minMaxScaling.x; 
        if (_scaleLoop)
        {
            if (_scaleDuration > 0)
            {
                _scaleSequence.Append(rectTransform.DOScale(_minMaxScaling.y, _scaleDuration).SetEase(_scaleAnimCurve))
                    .Append(rectTransform.DOScale(_minMaxScalingPulse.x, _stageTransitionDuration))
                    .Append(rectTransform.DOScale(_minMaxScalingPulse.y, _scaleDuration)
                        .SetLoops(-1, LoopType.Yoyo));
            }
            else
            {
                rectTransform.localScale = Vector3.one * _minMaxScalingPulse.x; 
                _fadeSequence.Append(rectTransform.DOScale(_minMaxScalingPulse.y, 1.0f)
                    .SetLoops(-1, LoopType.Yoyo));
            }
        }
        else
        {
            _scaleSequence.Append(rectTransform.DOScale(_minMaxScaling.y, _scaleDuration).SetEase(_scaleAnimCurve));
        }
        
    }
    
    private void DoImageFade(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, _minMaxAlpha.x);
        if (_fadeLoop)
        {
            if (_fadeDuration > 0)
            {
                _fadeSequence.Append(image.DOFade(_minMaxAlpha.y, _fadeDuration).SetEase(_fadeAnimCurve))
                    .Append(image.DOFade(_minMaxAlphaPulse.x, _stageTransitionDuration))
                    .Append(image.DOFade(_minMaxAlphaPulse.y, _fadeDuration)
                        .SetLoops(-1, LoopType.Yoyo));
            }
            else
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, _minMaxAlphaPulse.x);
                _fadeSequence.Append(image.DOFade(_minMaxAlphaPulse.y, 1.0f)
                    .SetLoops(-1, LoopType.Yoyo));
            }
        }
        else
        {
            _fadeSequence.Append(image.DOFade(_minMaxAlpha.y, _fadeDuration).SetEase(_fadeAnimCurve));
        }
    }
    
    private void DoCanvasGroupFade(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = _minMaxAlpha.x;
        if (_fadeLoop)
        {
            if (_fadeDuration > 0)
            {
                _fadeSequence.Append(canvasGroup.DOFade(_minMaxAlpha.y, _fadeDuration).SetEase(_fadeAnimCurve))
                    .Append(canvasGroup.DOFade(_minMaxAlphaPulse.x, _stageTransitionDuration))
                    .Append(canvasGroup.DOFade(_minMaxAlphaPulse.y, _fadeDuration)
                        .SetLoops(-1, LoopType.Yoyo));
            }
            else
            {
                canvasGroup.alpha = _minMaxAlphaPulse.x;
                _fadeSequence.Append(canvasGroup.DOFade(_minMaxAlphaPulse.y, 1.0f)
                        .SetLoops(-1, LoopType.Yoyo));
            }
        }
        else
        {
            _fadeSequence.Append(canvasGroup.DOFade(_minMaxAlpha.y, _fadeDuration).SetEase(_fadeAnimCurve));
        }
    }
    
    
    private void DoTMProFade(TextMeshProUGUI tmproGUI)
    {
        
        tmproGUI.alpha = _minMaxAlpha.x;
        if (_fadeLoop)
        {
            if (_fadeDuration > 0)
            {
                _fadeSequence.Append(tmproGUI.DOFade(_minMaxAlpha.y, _fadeDuration).SetEase(_fadeAnimCurve))
                    .Append(tmproGUI.DOFade(_minMaxAlphaPulse.x, _stageTransitionDuration))
                    .Append(tmproGUI.DOFade(_minMaxAlphaPulse.y, _fadeDuration)
                        .SetLoops(-1, LoopType.Yoyo));
            }
            else
            {
                tmproGUI.alpha = _minMaxAlphaPulse.x;
                _fadeSequence.Append(tmproGUI.DOFade(_minMaxAlphaPulse.y, 1.0f)
                    .SetLoops(-1, LoopType.Yoyo));
            }
        }
        else
        {
            _fadeSequence.Append(tmproGUI.DOFade(_minMaxAlpha.y, _fadeDuration).SetEase(_fadeAnimCurve));
        }
    }

    private void DoRectTransformRotate(RectTransform rectTransform)
    {
        rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, _minMaxRotAngle.x);
        if (_rotateLoop)
        {
            float randomSpeed = UnityEngine.Random.Range(_minMaxRotSpeed.x, _minMaxRotSpeed.y);
            if (_rotateDuration > 0)
            {
                _fadeSequence.Append(rectTransform.DORotate(new Vector3(0.0f, 0.0f, _minMaxRotAngle.y), _rotateDuration).SetEase(_rotateAnimCurve))
                    .Append(rectTransform.DORotate(new Vector3(0.0f, 0.0f, rectTransform.eulerAngles.z + randomSpeed), 1.0f)
                        .SetLoops(-1, LoopType.Incremental));
            }
            else
            {
                _fadeSequence.Append(rectTransform.DORotate(new Vector3(0.0f, 0.0f, rectTransform.eulerAngles.z + randomSpeed), 1.0f)
                    .SetLoops(-1, LoopType.Incremental));
            }
        }
        else
        {
            _fadeSequence.Append(rectTransform.DORotate(new Vector3(0.0f, 0.0f, _minMaxRotAngle.y), _rotateDuration).SetEase(_rotateAnimCurve));
        }
    }
    
    private void DoTextCountUpdate(TextMeshProUGUI tmproGUI)
    {
        _currentCount = 0;
        _textSequence.Append(
            DOTween.To(() => _currentCount, x =>
            {
                tmproGUI.text = x.ToString();
            }, _targetCount, _textDuration));
    }

    private void InvokeOnCompleteActions()
    {
        foreach (var action in _onCompleteShowActions)
        {
            action?.Invoke();
        }
    }

    public virtual void PlayHideAnimation()
    {
        
    }

}
