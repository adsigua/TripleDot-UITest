using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public enum UIParticleAnimationStage
{
    Spawn, Idle, Despawn
}

[System.Serializable]
public class UIParticleStageAnimations
{
    public bool enabled = false;
    private UIParticleAnimationStage _animationStage;
    [ShowIf("enabled"), AllowNesting] public float stageDuration = 0.3f;
    [ShowIf("enabled"), AllowNesting] public UIParticleColorAnimationModule colorAnimation = new();
    [ShowIf("enabled"), AllowNesting] public UIParticleScaleAnimationModule scaleAnimation = new();
    [ShowIf("enabled"), AllowNesting] public UIParticleRotationAnimationModule rotationAnimation = new();
    [ShowIf("enabled"), AllowNesting] public UIParticlePositionAnimationModule positionAnimation = new();

    public UIParticleStageAnimations(UIParticleAnimationStage animStage)
    {
        _animationStage = animStage;
    }
    
    public bool anyModuleEnabled => colorAnimation.enabled || scaleAnimation.enabled || rotationAnimation.enabled || positionAnimation.enabled;
    
    public void SetTransitionValues(Image image)
    {
        if (colorAnimation.enabled)
        {
            colorAnimation.transitionColor = image.color;
        }
        if (scaleAnimation.enabled)
        {
            scaleAnimation.transitionScale = image.rectTransform.localScale;
            //colorAnimation.SetTransitionColor(image.color);
        }
        if (rotationAnimation.enabled)
        {
            //rotationAnimation.ApplyAnimation(image, _animationStage, time, stageDuration, transitionTime);
        }
        if (positionAnimation.enabled)
        {
            //positionAnimation.ApplyAnimation(image, _animationStage, time, stageDuration, transitionTime);
        }
    }
    
    public void ApplyAnimationModules(Image image, float time, float transitionTime, bool fullTimeOnly = false)
    {
        if (colorAnimation.enabled && (!fullTimeOnly || (colorAnimation.fullTime)))
        {
            colorAnimation.ApplyAnimation(image, _animationStage, time, stageDuration, transitionTime);
        }
        if (scaleAnimation.enabled && (!fullTimeOnly || (scaleAnimation.fullTime)))
        {
            scaleAnimation.ApplyAnimation(image, _animationStage, time, stageDuration, transitionTime);
        }
        if (rotationAnimation.enabled && (!fullTimeOnly || (rotationAnimation.fullTime)))
        {
            rotationAnimation.ApplyAnimation(image, _animationStage, time, stageDuration, transitionTime);
        }
        if (positionAnimation.enabled && (!fullTimeOnly || (positionAnimation.fullTime)))
        {
            positionAnimation.ApplyAnimation(image, _animationStage, time, stageDuration, transitionTime);
        }
    }
}

[System.Serializable]
public abstract class UIParticleAnimationModule
{
    public bool enabled = false;
    [ShowIf("enabled"), AllowNesting] public AnimationCurve animationCurve = AnimationCurve.Linear(0,0,1,1);
    [ShowIf("enabled"), AllowNesting] public bool useNoise = false;
    [ShowIf("enabled"), AllowNesting] public float idleSpeed = 1.0f;
    [ShowIf("enabled"), AllowNesting] public bool fullTime = false;

    [SerializeField, ReadOnly, ShowIf("enabled"), AllowNesting] 
    protected float _currentAnimationValuePos = 0;

    public void SetCurrentValue(float value)
    {
        _currentAnimationValuePos = value;
    }
    public abstract void ApplyAnimation(Image image, UIParticleAnimationStage stage, float time, float stageDuration, float transitionDuration);
    
    public static float ScrollingNoise(float x, float scrollSpeed, float time)
    {
        // Scroll the noise along x over time
        x += time * scrollSpeed;

        // Integer lattice points
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;

        // Fractional part
        float t = x - x0;

        // Get pseudo-random gradients at the points
        float g0 = Gradient(x0);
        float g1 = Gradient(x1);

        // Dot product (1D: gradient * offset)
        float v0 = g0 * (t);
        float v1 = g1 * (t - 1);

        // Smoothstep interpolation
        float fadeT = Fade(t);
        return Mathf.Lerp(v0, v1, fadeT);
    }

    // Generate a pseudo-random gradient (-1 to 1) from an integer coordinate
    private static float Gradient(int x)
    {
        int hash = x * 374761393; // some large prime
        hash = (hash ^ (hash >> 13)) * 1274126177;
        hash ^= (hash >> 16);
        return (hash & 0xFFFF) / 32768f - 1f; // maps to [-1, 1]
    }

    // Smoothstep interpolation function
    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}

[System.Serializable]
public class UIParticleColorAnimationModule : UIParticleAnimationModule
{
    [ShowIf("enabled"), AllowNesting] public Gradient colorOverAnimationLifetime;

    public Color transitionColor;
    
    public override void ApplyAnimation(Image image, UIParticleAnimationStage stage, float time, float stageDuration, float transitionDuration)
    {
        float timeRatio = time / stageDuration;
        float smoothTime = animationCurve.Evaluate(timeRatio);
        switch (stage)
        {
            case UIParticleAnimationStage.Spawn:
            case UIParticleAnimationStage.Despawn:
                _currentAnimationValuePos = smoothTime;
                break;
            case UIParticleAnimationStage.Idle:
                _currentAnimationValuePos = 
                    useNoise ? ScrollingNoise(0, idleSpeed, time) : 
                    Mathf.Sin(time * idleSpeed);
                _currentAnimationValuePos = _currentAnimationValuePos * 0.5f + 0.5f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
        }

        Color newColor = colorOverAnimationLifetime.Evaluate(_currentAnimationValuePos);
        image.color = transitionDuration > 0 ? 
            Color.Lerp(transitionColor, newColor, Mathf.Clamp01(time / transitionDuration)) : 
            newColor;
    }
}

[System.Serializable]
public class UIParticleScaleAnimationModule : UIParticleAnimationModule
{
    [ShowIf("enabled"), AllowNesting] public Vector3 startScale;
    [ShowIf("enabled"), AllowNesting] public Vector3 endScale;
    public Vector3 transitionScale;

    public override void ApplyAnimation(Image image, UIParticleAnimationStage stage, float time, float stageDuration, float transitionDuration)
    {
        float timeRatio = time / stageDuration;
        float smoothTime = animationCurve.Evaluate(timeRatio);
        
        switch (stage)
        {
            case UIParticleAnimationStage.Spawn:
            case UIParticleAnimationStage.Despawn:
                _currentAnimationValuePos = smoothTime;
                break;
            case UIParticleAnimationStage.Idle:
                _currentAnimationValuePos = 
                    useNoise ? ScrollingNoise(0, idleSpeed, time) : 
                        Mathf.Sin(time * idleSpeed) * 0.5f + 0.5f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
        }
        Vector3 newScale = Vector3.Lerp(startScale, endScale, _currentAnimationValuePos);
        image.rectTransform.localScale = transitionDuration > 0 ? 
            Vector3.Lerp(transitionScale, newScale, Mathf.Clamp01(time / transitionDuration)) : 
            newScale;
    }
}

[System.Serializable]
public class UIParticleRotationAnimationModule : UIParticleAnimationModule
{
    [ShowIf("enabled"), AllowNesting] public Vector3 startRot;
    [ShowIf("enabled"), AllowNesting] public Vector3 endRot;
    [ShowIf("enabled"), AllowNesting] public bool continuousRotation = false;
    [ShowIf("enabled"), AllowNesting] public Vector3 rotationAxis;
    
    public Quaternion transitionRotation;

    public override void ApplyAnimation(Image image, UIParticleAnimationStage stage, float time, float stageDuration, float transitionDuration)
    {
        float timeRatio = time / stageDuration;
        float smoothTime = animationCurve.Evaluate(timeRatio);
        Quaternion newRot;
        switch (stage)
        {
            case UIParticleAnimationStage.Spawn:
            case UIParticleAnimationStage.Despawn:
                _currentAnimationValuePos = smoothTime;
                newRot = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), _currentAnimationValuePos);
                image.rectTransform.localRotation = transitionDuration > 0 ? 
                    Quaternion.Lerp(transitionRotation, newRot, Mathf.Clamp01(time / transitionDuration)) : 
                    newRot;
                break;
            case UIParticleAnimationStage.Idle:
                if (continuousRotation)
                {
                    image.rectTransform.localRotation *= Quaternion.Euler(rotationAxis * (idleSpeed * Time.deltaTime));
                }
                else
                {
                    _currentAnimationValuePos = 
                        useNoise ? ScrollingNoise(0, idleSpeed, time) : 
                            Mathf.Sin(time * idleSpeed) * 0.5f + 0.5f;
                    newRot = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), _currentAnimationValuePos);
                    image.rectTransform.localRotation = transitionDuration > 0 ? 
                        Quaternion.Lerp(transitionRotation, newRot, Mathf.Clamp01(time / transitionDuration)) : 
                        newRot;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
        }
    }
}

[System.Serializable]
public class UIParticlePositionAnimationModule : UIParticleAnimationModule
{
    [ShowIf("enabled"), AllowNesting] public Vector3 startPos;
    [ShowIf("enabled"), AllowNesting] public Vector3 endPos;
    
    public override void ApplyAnimation(Image image, UIParticleAnimationStage stage, float time, float stageDuration, float transitionDuration)
    {
        throw new NotImplementedException();
    }
}


