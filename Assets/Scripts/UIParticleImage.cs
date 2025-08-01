using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

public class UIParticleImage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private bool _looping = false;
    [SerializeField] private float _stageTransitionDuration = 0.2f;
    [SerializeField] private bool _playOnEnable = false;

    [SerializeField] private UIParticleStageAnimations _spawnStageAnimations = new(UIParticleAnimationStage.Spawn);
    [SerializeField] private UIParticleStageAnimations _idleStageAnimations = new(UIParticleAnimationStage.Idle);
    [SerializeField] private UIParticleStageAnimations _despawnStageAnimations = new(UIParticleAnimationStage.Despawn);
    
    private Coroutine _animationCoroutine;
    public static event System.Action<UIParticleImage> OnParticleImageDespawn;
    private UIParticleAnimationStage _currentAnimtionStage = UIParticleAnimationStage.Spawn;

    private void OnEnable()
    {
        if (_playOnEnable)
        {
            PlayParticle(true);
        }
    }

    private void OnDisable()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
    }

    private void PlayParticle(bool overrideIfPlaying)
    {
        _currentAnimtionStage = UIParticleAnimationStage.Spawn;
        if (overrideIfPlaying && _animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }else if (_animationCoroutine != null)
        {
            return;
        }
        _animationCoroutine = StartCoroutine(AnimationCoroutine());
    }

    private void StopParticle(bool doInstant = false)
    {
        if (doInstant)
        {
            OnParticleImageDespawn?.Invoke(this);
        }
        else
        {
            
        }
    }

    private IEnumerator AnimationCoroutine()
    {
        float stageTime = 0;
        float totalFxTime = 0;
        if (_spawnStageAnimations.enabled && _spawnStageAnimations.anyModuleEnabled &&
            _spawnStageAnimations.stageDuration > 0)
        {
            while (_currentAnimtionStage == UIParticleAnimationStage.Spawn && stageTime < _spawnStageAnimations.stageDuration)
            {
                stageTime += Time.deltaTime;
                totalFxTime += Time.deltaTime;
                _idleStageAnimations.ApplyAnimationModules(_image, stageTime, _stageTransitionDuration, true);
                _spawnStageAnimations.ApplyAnimationModules(_image, stageTime, _stageTransitionDuration, false);
                yield return null;
            }
        }
        _currentAnimtionStage = UIParticleAnimationStage.Idle;
        
        stageTime = 0;
        if (_idleStageAnimations.enabled)
        {
            _idleStageAnimations.SetTransitionValues(_image);
            while (_currentAnimtionStage == UIParticleAnimationStage.Idle && 
                   ((stageTime < _idleStageAnimations.stageDuration && _idleStageAnimations.stageDuration > 0) 
                    || _looping))
            {
                stageTime += Time.deltaTime;
                totalFxTime += Time.deltaTime;
                _idleStageAnimations.ApplyAnimationModules(_image, stageTime, _stageTransitionDuration, false);
                yield return null;
            }
        }
        
        _currentAnimtionStage = UIParticleAnimationStage.Despawn;
        if (_despawnStageAnimations.enabled && _despawnStageAnimations.anyModuleEnabled &&
            _despawnStageAnimations.stageDuration > 0)
        {
            stageTime = 0;
            _despawnStageAnimations.SetTransitionValues(_image);
            while (_currentAnimtionStage == UIParticleAnimationStage.Despawn && stageTime < _despawnStageAnimations.stageDuration)
            {
                stageTime += Time.deltaTime;
                totalFxTime += Time.deltaTime;
                _idleStageAnimations.ApplyAnimationModules(_image, stageTime, _stageTransitionDuration, true);
                _despawnStageAnimations.ApplyAnimationModules(_image, stageTime, _stageTransitionDuration, false);
                yield return null;
            }
        }
        OnParticleImageDespawn?.Invoke(this);
        _animationCoroutine = null;
    }


    
}
