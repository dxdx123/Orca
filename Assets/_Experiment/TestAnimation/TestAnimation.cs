using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    private Animation _animation;

    private AnimationState _animationState;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
        
        string clipName = _animation.clip.name;
        _animationState = _animation[clipName];
    }

    private void Update()
    {
        CheckAnimationOutBounds();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Play();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Reverse();
        }
    }

    private void CheckAnimationOutBounds()
    {
        if (_animation.isPlaying)
        {
            float time = _animationState.normalizedTime;
            bool stop = time < 0.0f || time > 1.0f;

            if (stop)
            {
                _animation.Stop();
            }
            else
            {
                // nothing
            }
        }
        else
        {
            // nothing
        }
    }

    private bool IsAnimationStop()
    {
        float time = _animationState.normalizedTime;

        return time <= 0.0f || time >= 1.0f;
    }

    private void Reverse()
    {
        if (IsAnimationStop())
        {
            _animationState.normalizedTime = 1.0f;
            _animation.Play();
        }
        else
        {
            // nothing
        }

        _animationState.speed = -1.0f;
        float time = Mathf.Clamp(_animationState.normalizedTime, 0.0f, 1.0f);
        _animationState.normalizedTime = time;
    }
    
    private void Play()
    {
        if (IsAnimationStop())
        {
            _animationState.normalizedTime = 0.0f;
            _animation.Play();
        }
        else
        {
            // nothing
        }

        _animationState.speed = 1.0f;
        float time = Mathf.Clamp(_animationState.normalizedTime, 0.0f, 1.0f);
        _animationState.normalizedTime = time;
    }
}
