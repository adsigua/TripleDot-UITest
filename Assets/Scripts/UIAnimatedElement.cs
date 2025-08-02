using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
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

    [SerializeField] private ElementType _elementType = ElementType.RectTransform;
    [SerializeField] private AnimationType _animationType = AnimationType.Fade;
    [SerializeField] private float _totalDelay = 0.0f;
    [SerializeField] private float _stageTransitionDuration = 0.3f;
    [SerializeField] private bool _playOnEnable = false;
    
    [SerializeField, ShowIf("isFading")] private AnimationFadeData _fadeData;
    [SerializeField, ShowIf("isSliding")] private AnimationSlideData _slideData;
    [SerializeField, ShowIf("isScaling")] private AnimationScaleData _scaleData;
    [SerializeField, ShowIf("isRotating")] private AnimationRotateData _rotateData;
    
    [SerializeField, Foldout("Text"), ShowIf("isText")] private int _targetCount;
    [SerializeField, Foldout("Text"), ShowIf("isText")] private float _textDelay = 0.0f;
    [SerializeField, Foldout("Text"), ShowIf("isText")] private float _textDuration = 10.0f;

    private bool isSliding => (_animationType & AnimationType.Slide) != 0;
    private bool isRotating => (_animationType & AnimationType.Rotate) != 0;
    private bool isScaling => (_animationType & AnimationType.Scale) != 0;
    private bool isFading => (_animationType & AnimationType.Fade) != 0;

    public bool isShown { get; private set; }

    private Vector2 _startAnchoredPos;

    [System.Serializable]
    private abstract class AnimationTypeData
    {
        public abstract AnimationType animationType { get; }
        public float duration = 0.5f;
        public float delay = 0.0f;
        public bool loop = false;
        public bool hasExit = false;
        public bool exitOnly = false;
        public float loopCycleDuration = 0.5f;
        public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }
    
    [System.Serializable]
    private class AnimationFadeData : AnimationTypeData
    {
        public override AnimationType animationType => AnimationType.Fade;
        public Vector2 minMaxAlphaEntry = new Vector2(0.0f, 1.0f);
        public Vector2 minMaxAlphaPulse = new Vector2(0.0f, 1.0f);
    }
    
    [System.Serializable]
    private class AnimationSlideData : AnimationTypeData
    {
        public override AnimationType animationType => AnimationType.Slide;
        public Vector2 slideDirection = Vector2.up;
        public float slideDistance = 100.0f;
    }
    
    [System.Serializable]
    private class AnimationScaleData : AnimationTypeData
    {
        public override AnimationType animationType => AnimationType.Scale;
        public Vector2 minMaxScaleEntry = new Vector2(0.0f, 1.0f);
        public Vector2 minMaxScalePulse = new Vector2(0.95f, 1.05f);
    }
    
    [System.Serializable]
    private class AnimationRotateData : AnimationTypeData
    {
        public override AnimationType animationType => AnimationType.Rotate;
        public Vector2 minMaxRotationAngle = new Vector2(0.0f, 40.0f);
        public Vector2 minMaxRotationAnglePulse = new Vector2(30.0f, 40.0f);
    }

    private bool isText => _elementType == ElementType.Text;
    
    private int _currentCount;
    
    private Tween _fadeTween  ;
    private Tween _slideTween ;
    private Tween _scaleTween ;
    private Tween _textTween  ;
    private Tween _rotateTween;
    
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private TextMeshProUGUI _textMeshProGUI;

    private void Awake()
    {
        _rectTransform ??= GetComponent<RectTransform>();
        _startAnchoredPos = _rectTransform.anchoredPosition;
    }
    
    private void OnEnable()
    {
        if (_playOnEnable)
        {
            PlayShowAnimation();
        }
    }
    
    private void OnDisable()
    {
        _fadeTween  ?.Kill();
        _slideTween ?.Kill();
        _scaleTween ?.Kill();
        _textTween  ?.Kill();
        _rotateTween?.Kill();
    }

    public void SetTargetCount(int count)
    {
        _targetCount = count;
    }

    [Button]
    public void PlayShowAnimation(float delay = 0)
    {
        InitElementState();
        InitTweens(delay + _totalDelay);
        isShown = true;

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
    
    [Button]
    public void PlayExitAnimation()
    {
        InitTweens(0);
        isShown = false;
        switch (_elementType)
        {
            case ElementType.RectTransform:
                PlayRectTransformAnimation(true);
                break;
            case ElementType.Image:
                PlayImageAnimation(true);
                break;
            case ElementType.Text:
                DoTMProAnimations(true);
                break;
            case ElementType.CanvasGroup:
                PlayCanvasGroupAnimation(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void InitElementState()
    {
        isShown = false;
        switch (_elementType)
        {
            case ElementType.RectTransform:
                InitRectTransformState();
                break;
            case ElementType.Image:
                InitImageState();
                break;
            case ElementType.Text:
                InitTextState();
                break;
            case ElementType.CanvasGroup:
                InitCanvasGroupState();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InitRectTransformState()
    {
        _rectTransform ??= GetComponent<RectTransform>();
        if (isSliding)
        {
            InitSlideRectTransform(_rectTransform);
        }
        if (isScaling)
        {
            InitScaleRectTransform(_rectTransform);
        }
        if (isRotating)
        {
            InitRotationRectTransform(_rectTransform);
        }
    }
    
    private void InitImageState()
    {
        _image ??= GetComponent<Image>();
        if (isFading)
        {
            float initAlpha = _fadeData.exitOnly ? _fadeData.minMaxAlphaEntry.y : _fadeData.minMaxAlphaEntry.x;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, initAlpha);
        }
        if (isSliding)
        {
            InitSlideRectTransform(_image.rectTransform);
        }
        if (isScaling)
        {
            InitScaleRectTransform(_image.rectTransform);
        }
        if (isRotating)
        {
            InitRotationRectTransform(_image.rectTransform);
        }
    }
    
    private void InitCanvasGroupState()
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _rectTransform ??= GetComponent<RectTransform>();
        if (isFading)
        {
            float initAlpha = _fadeData.exitOnly ? _fadeData.minMaxAlphaEntry.y : _fadeData.minMaxAlphaEntry.x;
            _canvasGroup.alpha = initAlpha;
        }
        if (isSliding)
        {
            InitSlideRectTransform(_rectTransform);
        }
        if (isScaling)
        {
            InitScaleRectTransform(_rectTransform);
        }
        if (isRotating)
        {
            InitRotationRectTransform(_rectTransform);
        }
    }
    
    private void InitTextState()
    {
        _textMeshProGUI ??= GetComponent<TextMeshProUGUI>();
        if (isFading)
        {
            float initAlpha = _fadeData.exitOnly ? _fadeData.minMaxAlphaEntry.y : _fadeData.minMaxAlphaEntry.x;
            _textMeshProGUI.alpha = initAlpha;
        }
        if (isSliding)
        {
            InitSlideRectTransform(_textMeshProGUI.rectTransform);
        }
        if (isScaling)
        {
            InitScaleRectTransform(_textMeshProGUI.rectTransform);
        }
        if (isRotating)
        {
            InitRotationRectTransform(_textMeshProGUI.rectTransform);
        }
    }

    private void InitSlideRectTransform(RectTransform rectTransform)
    {
        Vector2 startPos = _startAnchoredPos - _slideData.slideDirection.normalized * _slideData.slideDistance;
        rectTransform.anchoredPosition = _slideData.exitOnly ? _startAnchoredPos : startPos;
    }
    
    private void InitScaleRectTransform(RectTransform rectTransform)
    {
        rectTransform.localScale = Vector3.one * (_scaleData.exitOnly ? _scaleData.minMaxScaleEntry.y : _scaleData.minMaxScaleEntry.x);
    }
    
    private void InitRotationRectTransform(RectTransform rectTransform)
    {
        rectTransform.localRotation = Quaternion.Euler(Vector3.forward * (_rotateData.exitOnly ? _rotateData.minMaxRotationAngle.y : _rotateData.minMaxRotationAngle.x)); 
    }
    
    private void InitTweens(float delay)
    {
        InitTween(ref _fadeTween);
        InitTween(ref _slideTween);
        InitTween(ref _scaleTween);
        InitTween(ref _rotateTween);
        InitTween(ref _textTween);   
    }

    private void InitTween(ref Tween tween)
    {
        if (tween is { active: true })
        {
            tween.Kill();
        }
    }

    private bool ExitCheck(AnimationTypeData animationTypeData, bool isExit)
    {
        return (isExit && (animationTypeData.hasExit || animationTypeData.exitOnly)) || (!isExit && !animationTypeData.exitOnly);
    }
    
    private bool CanExit(AnimationTypeData animationTypeData, bool isExit)
    {
        return isExit && !(animationTypeData.exitOnly || animationTypeData.hasExit);
    }
    
    private void PlayRectTransformAnimation(bool isExit = false)
    {
        _rectTransform ??= GetComponent<RectTransform>();

        if (isSliding)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoRectTransformSlide(_rectTransform, isExit);
            }else if (isExit)
            {
                _slideTween.Kill();
            }
        }
        
        if (isScaling)
        {
            if (ExitCheck(_scaleData, isExit))
            {
                DoRectTransformScale(_rectTransform, isExit);
            }else if (isExit)
            {
                _scaleTween.Kill();
            }
        }
        
        if (isRotating)
        {
            if (ExitCheck(_rotateData, isExit))
            {
                DoRectTransformRotate(_rectTransform, isExit);
            }else if (isExit)
            {
                _rotateTween.Kill();
            }
        }
    }
    
    private void PlayImageAnimation(bool isExit = false)
    {
        _image ??= GetComponent<Image>();
        
        if (isFading)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoImageFade(_image, isExit);
            }else if (isExit)
            {
                _fadeTween.Kill();
            }
        }
        
        if (isSliding)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoRectTransformSlide(_image.rectTransform, isExit);
            }else if (isExit)
            {
                _slideTween.Kill();
            }
        }
        
        if (isScaling)
        {
            if (ExitCheck(_scaleData, isExit))
            {
                DoRectTransformScale(_image.rectTransform, isExit);
            }else if (isExit)
            {
                _scaleTween.Kill();
            }
        }
        
        if (isRotating)
        {
            if (ExitCheck(_rotateData, isExit))
            {
                DoRectTransformRotate(_image.rectTransform, isExit);
            }else if (isExit)
            {
                _rotateTween.Kill();
            }
        }
    }
    
    private void DoTMProAnimations(bool isExit = false)
    {
        _textMeshProGUI ??= GetComponent<TextMeshProUGUI>();
        if(!isExit)
            DoTextCountUpdate(_textMeshProGUI);
        if (isFading)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoTMProFade(_textMeshProGUI, isExit);
            }else if (isExit)
            {
                _fadeTween.Kill();
            }
        }
        
        if (isSliding)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoRectTransformSlide(_textMeshProGUI.rectTransform, isExit);
            }else if (isExit)
            {
                _slideTween.Kill();
            }
        }
        
        if (isScaling)
        {
            if (ExitCheck(_scaleData, isExit))
            {
                DoRectTransformScale(_textMeshProGUI.rectTransform, isExit);
            }else if (isExit)
            {
                _scaleTween.Kill();
            }
        }
        
        if (isRotating)
        {
            if (ExitCheck(_rotateData, isExit))
            {
                DoRectTransformRotate(_textMeshProGUI.rectTransform, isExit);
            }else if (isExit)
            {
                _rotateTween.Kill();
            }
        }
    }
    
    private void PlayCanvasGroupAnimation(bool isExit = false)
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _rectTransform ??= GetComponent<RectTransform>();

        if (isFading)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoCanvasGroupFade(_canvasGroup, isExit);
            }else if (isExit)
            {
                _fadeTween.Kill();
            }
        }
        
        if (isSliding)
        {
            if (ExitCheck(_slideData, isExit))
            {
                DoRectTransformSlide(_rectTransform, isExit);
            }else if (isExit)
            {
                _slideTween.Kill();
            }
        }
        
        if (isScaling)
        {
            if (ExitCheck(_scaleData, isExit))
            {
                DoRectTransformScale(_rectTransform, isExit);
            }else if (isExit)
            {
                _scaleTween.Kill();
            }
        }
        
        if (isRotating)
        {
            if (ExitCheck(_rotateData, isExit))
            {
                DoRectTransformRotate(_rectTransform, isExit);
            }else if (isExit)
            {
                _rotateTween.Kill();
            }
        }
    }

    private void DoRectTransformSlide(RectTransform rectTransform, bool isExit)
    {
        if (_slideData.loop)
        {
            throw new ArgumentOutOfRangeException();
        }
        else
        {
            Vector2 targetPos = _startAnchoredPos;
            Vector2 startPos = _startAnchoredPos - _slideData.slideDirection.normalized * _slideData.slideDistance;
            if (!isExit)
            {
                rectTransform.anchoredPosition = startPos;
            }
            _slideTween = rectTransform.DOAnchorPos(isExit ? startPos : targetPos, _slideData.duration)
                .SetDelay(_totalDelay + _slideData.delay)
                .SetEase(_slideData.animationCurve);
        }
    }
    
    private void DoRectTransformScale(RectTransform rectTransform, bool isExit)
    {
        if (!isExit)
            rectTransform.localScale = Vector3.one * _scaleData.minMaxScaleEntry.x; 
        if (_scaleData.loop && !isExit)
        {
            if (_scaleData.duration > 0)
            {
                _scaleTween = rectTransform.DOScale(_scaleData.minMaxScaleEntry.y, _scaleData.duration)
                    .SetEase(_scaleData.animationCurve)
                    .SetDelay(_totalDelay + _scaleData.delay)
                    .OnComplete(() =>
                {
                    _scaleTween = rectTransform.DOScale(_scaleData.minMaxScalePulse.x, _stageTransitionDuration).OnComplete(() =>
                    {
                        _scaleTween = rectTransform.DOScale(_scaleData.minMaxScalePulse.y, _scaleData.loopCycleDuration)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
                });
            }
            else
            {
                rectTransform.localScale = Vector3.one * _scaleData.minMaxScalePulse.x; 
                _scaleTween = rectTransform.DOScale(_scaleData.minMaxScalePulse.y, _scaleData.loopCycleDuration)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            _scaleTween = rectTransform.DOScale(isExit ? _scaleData.minMaxScaleEntry.x : _scaleData.minMaxScaleEntry.y, _scaleData.duration)
                .SetDelay(_totalDelay + _scaleData.delay)
                .SetEase(_scaleData.animationCurve);
        }
    }

    private void DoImageFade(Image image, bool isExit)
    {
        if (!isExit)
            image.color = new Color(image.color.r, image.color.g, image.color.b, _fadeData.minMaxAlphaEntry.x);
        if (_fadeData.loop && !isExit)
        {
            if (_fadeData.duration > 0)
            {
                _fadeTween = image.DOFade(_fadeData.minMaxAlphaEntry.y, _fadeData.duration)
                    .SetDelay(_totalDelay + _fadeData.delay)
                    .SetEase(_fadeData.animationCurve).OnComplete(() =>
                    {
                        _fadeTween =  image.DOFade(_fadeData.minMaxAlphaPulse.x, _stageTransitionDuration).OnComplete(() =>
                        {
                            _fadeTween = image.DOFade(_fadeData.minMaxAlphaPulse.y, _fadeData.loopCycleDuration)
                                .SetLoops(-1, LoopType.Yoyo);
                        });
                    });
            }
            else
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, _fadeData.minMaxAlphaPulse.x);
                _slideTween = image.DOFade(_fadeData.minMaxAlphaPulse.y, _fadeData.loopCycleDuration)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            _fadeTween = image.DOFade(isExit ? _fadeData.minMaxAlphaEntry.x : _fadeData.minMaxAlphaEntry.y, _fadeData.duration)
                .SetDelay(_totalDelay + _fadeData.delay)
                .SetEase(_fadeData.animationCurve);
        }
    }
    
    private void DoCanvasGroupFade(CanvasGroup canvasGroup, bool isExit)
    {
        if (!isExit)
            canvasGroup.alpha = _fadeData.minMaxAlphaEntry.x;
        if (_fadeData.loop && !isExit)
        {
            if (_fadeData.duration > 0)
            {
                _fadeTween = canvasGroup.DOFade(_fadeData.minMaxAlphaEntry.y, _fadeData.duration)
                    .SetDelay(_totalDelay + _fadeData.delay)
                    .SetEase(_fadeData.animationCurve)
                    .OnComplete(() =>
                    {
                        _fadeTween = canvasGroup.DOFade(_fadeData.minMaxAlphaPulse.x, _stageTransitionDuration).OnComplete(() =>
                        {
                            _fadeTween = canvasGroup.DOFade(_fadeData.minMaxAlphaPulse.y, _fadeData.loopCycleDuration)
                                .SetLoops(-1, LoopType.Yoyo);
                        });
                    });
            }
            else
            {
                canvasGroup.alpha = _fadeData.minMaxAlphaPulse.x;
                _slideTween = canvasGroup.DOFade(_fadeData.minMaxAlphaPulse.y, _fadeData.loopCycleDuration)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            _fadeTween = canvasGroup.DOFade(isExit ? _fadeData.minMaxAlphaEntry.x : _fadeData.minMaxAlphaEntry.y, _fadeData.duration)
                .SetDelay(_totalDelay + _fadeData.delay)
                .SetEase(_fadeData.animationCurve);
        }
    }
    
    private void DoTMProFade(TextMeshProUGUI tmproGUI, bool isExit)
    {
        if (!isExit)
            tmproGUI.alpha = _fadeData.minMaxAlphaEntry.x;
        if (_fadeData.loop && !isExit)
        {
            if (_fadeData.duration > 0)
            {
                _fadeTween = tmproGUI.DOFade(_fadeData.minMaxAlphaEntry.y, _fadeData.duration)
                    .SetDelay(_totalDelay + _fadeData.delay)
                    .SetEase(_fadeData.animationCurve)
                    .OnComplete(() =>
                    {
                        _fadeTween = tmproGUI.DOFade(_fadeData.minMaxAlphaPulse.x, _stageTransitionDuration).OnComplete(() =>
                        {
                            _fadeTween = tmproGUI.DOFade(_fadeData.minMaxAlphaPulse.y, _fadeData.loopCycleDuration)
                                .SetLoops(-1, LoopType.Yoyo);
                        });
                    });
            }
            else
            {
                tmproGUI.alpha = _fadeData.minMaxAlphaPulse.x;
                _slideTween = tmproGUI.DOFade(_fadeData.minMaxAlphaPulse.y, _fadeData.loopCycleDuration)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            _fadeTween = tmproGUI.DOFade(isExit ? _fadeData.minMaxAlphaEntry.x : _fadeData.minMaxAlphaEntry.y, _fadeData.duration)
                .SetDelay(_totalDelay + _fadeData.delay)
                .SetEase(_fadeData.animationCurve);
        }
    }

    private void DoRectTransformRotate(RectTransform rectTransform, bool isExit)
    {
        if(isExit)
            rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, _rotateData.minMaxRotationAngle.x);
        if (_rotateData.loop && !isExit)
        {
            //float randomSpeed = UnityEngine.Random.Range(_rotateData.minMaxRotationSpeed.x, _rotateData.minMaxRotationSpeed.y);
            if (_rotateData.duration > 0)
            {
                _rotateTween = rectTransform.DORotate(new Vector3(0.0f, 0.0f, _rotateData.minMaxRotationAngle.y), _rotateData.duration)
                    .SetDelay(_totalDelay + _rotateData.delay)
                    .SetEase(_rotateData.animationCurve)
                    .OnComplete(() =>
                {
                    _rotateTween = rectTransform.DORotate(new Vector3(0.0f, 0.0f, _rotateData.minMaxRotationAnglePulse.x), _stageTransitionDuration)
                    .OnComplete(()=>
                    {
                        _rotateTween = rectTransform.DORotate(new Vector3(0.0f, 0.0f, rectTransform.localRotation.z + _rotateData.minMaxRotationAnglePulse.y), _rotateData.loopCycleDuration)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Incremental);
                    });
                });
            }
            else
            {
                rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, _rotateData.minMaxRotationAnglePulse.x);
                _rotateTween = rectTransform.DORotate(new Vector3(0.0f, 0.0f,  rectTransform.localRotation.z + _rotateData.minMaxRotationAnglePulse.y), _rotateData.loopCycleDuration)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
            }
        }
        else
        {
            _rotateTween = rectTransform.DORotate(new Vector3(0.0f, 0.0f, isExit ? _rotateData.minMaxRotationAngle.x : _rotateData.minMaxRotationAngle.y), _rotateData.duration)
                .SetDelay(_totalDelay + _rotateData.delay)
                .SetEase(_rotateData.animationCurve);
        }
    }
    
    private void DoTextCountUpdate(TextMeshProUGUI tmproGUI)
    {
        _currentCount = 0;
        _textTween =DOTween.To(() => _currentCount, x =>
            {
                tmproGUI.text = x.ToString();
            }, _targetCount, _textDuration)
            .SetDelay(_totalDelay + _textDelay)
            .SetEase(Ease.Linear);
    }
}
