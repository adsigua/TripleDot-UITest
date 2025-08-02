using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPopupBackgroundScreen : UIScreen
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private RawImage _backgroundBlurImage;
    [SerializeField] private bool _blurBackground = false;
    public bool blurBackground => _blurBackground;
    [SerializeField] private Material _blurMaterial;
    
    [SerializeField] private RenderTexture _screenCapTexture;
    public RenderTexture screenCapTexture => _screenCapTexture;
    [SerializeField] private RenderTexture _screenCapTextureBlurred;

    public override void ShowScreen(bool doAnimation = true, Action onComplete = null)
    {
        _backgroundBlurImage.gameObject.SetActive(_blurBackground);
        _backgroundImage.gameObject.SetActive(!_blurBackground);
        base.ShowScreen(doAnimation, onComplete);
    }

    public void InitTexture()
    {
        if (_screenCapTexture == null)
        {
            _screenCapTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _screenCapTextureBlurred = new RenderTexture(Screen.width, Screen.height, 24);
        }else if (_screenCapTexture.width != Screen.width || _screenCapTexture.height != Screen.height)
        {
            _screenCapTexture.Release();
            _screenCapTextureBlurred.Release();
            _screenCapTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _screenCapTextureBlurred = new RenderTexture(Screen.width, Screen.height, 24);
        }
        _screenCapTexture.enableRandomWrite = true;
        _backgroundBlurImage.texture = _screenCapTextureBlurred;
    }
    
    public void BlurTexture()
    {
        Graphics.Blit(_screenCapTexture, _screenCapTextureBlurred, _blurMaterial);
    }
    
    public override void RegisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
            return;
    }

    public override void UnregisterEventsListener<T>(T objectListener)
    {
        if(objectListener is not IUISettingsScreenEventsListener listener)
            return;
    }
}
