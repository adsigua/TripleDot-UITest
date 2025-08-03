using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdoTrpDotTest.UI
{
    public class UIPopupBackgroundScreen : UIScreen
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RawImage _backgroundBlurImage;
        [SerializeField] private bool _blurBackground = false;
        public bool blurBackground => _blurBackground;
        [SerializeField] private Material _blurMaterial;
    
        private RenderTexture _screenCapTexture;
        public RenderTexture screenCapTexture => _screenCapTexture;

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
            }else if (_screenCapTexture.width != Screen.width || _screenCapTexture.height != Screen.height)
            {
                _screenCapTexture.Release();
                _screenCapTexture = new RenderTexture(Screen.width, Screen.height, 24);
            }
            _screenCapTexture.enableRandomWrite = true;
            _backgroundBlurImage.texture = _screenCapTexture;
        }
    
        public void BlurTexture()
        {
            var tempRT = RenderTexture.GetTemporary(_screenCapTexture.width, _screenCapTexture.height);
            Graphics.Blit(_screenCapTexture, tempRT, _blurMaterial);
            Graphics.Blit(tempRT, _screenCapTexture);
            tempRT.Release();
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
}
