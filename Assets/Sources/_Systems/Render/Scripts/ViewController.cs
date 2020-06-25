using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

public class ViewController : MonoBehaviour, IViewController
{
    public tk2dSprite displaySprite { get; private set; }
    public tk2dSpriteAnimator displaySpriteAnimator { get; private set; }
    
    private Transform _transform;

    private GameEntity _gameEntity;
    
    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
        
        displaySprite = GetComponent<tk2dSprite>();
        Assert.IsNotNull(displaySprite);
        
        displaySpriteAnimator = GetComponent<tk2dSpriteAnimator>();
        // may be null

        if (displaySpriteAnimator != null)
        {
            displaySpriteAnimator.AnimationEventTriggered += (animator, clip, frameIndex) =>
            {
                var frame = clip.GetFrame(frameIndex);
                Assert.IsNotNull(frame);
                // Debug.Log($"{clip.name} {frame.eventInfo} {frame.eventFloat} {frame.eventInt}");

                if (_gameEntity != null)
                {
                    _gameEntity.AddViewEventTrigger(clip.name, frame.eventInfo, frame.eventFloat, frame.eventInt);
                }
                else
                {
                    // nothing
                }
            };
        }
    }

    public virtual void Initialize(GameEntity entity)
    {
        _gameEntity = entity;
        
        gameObject.Link(entity);
    }

    public virtual void Destroy()
    {
        EntityLink link = gameObject.GetEntityLink();
        link.Unlink();
        
        _gameEntity = null;
    } 
}

