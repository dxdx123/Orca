using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;

public class ViewController : MonoBehaviour, IViewController
{
    private Transform _transform;
    
    public tk2dSprite displaySprite { get; private set; }
    public tk2dSpriteAnimator displaySpriteAnimator { get; private set; }
    
    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
        
        displaySprite = GetComponent<tk2dSprite>();
        displaySpriteAnimator = GetComponent<tk2dSpriteAnimator>();
    }

    public virtual void Initialize(GameEntity entity)
    {
        gameObject.Link(entity);
    }

    public virtual void Destroy()
    {
        EntityLink link = gameObject.GetEntityLink();
        link.Unlink();

        PoolManager.Instance.ReleaseObject(gameObject);
    } 
}

