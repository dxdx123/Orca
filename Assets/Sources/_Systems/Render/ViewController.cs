using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;

public class ViewController : MonoBehaviour, IViewController, IFSM
{
    private Transform _transform;
    private PlayMakerFSM _fsm;
    
    public tk2dSprite displaySprite { get; private set; }
    public tk2dSpriteAnimator displaySpriteAnimator { get; private set; }
    
    public PlayMakerFSM fsm
    {
        get { return _fsm; }
    }

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _fsm = GetComponent<PlayMakerFSM>();
        
        displaySprite = GetComponent<tk2dSprite>();
        displaySpriteAnimator = GetComponent<tk2dSpriteAnimator>();
    }

    public void Initialize(GameEntity entity)
    {
        gameObject.Link(entity);

        var containerGo = _fsm.FsmVariables.GetFsmGameObject("containerGo");
        containerGo.Value = gameObject;
    }

    public void Destroy()
    {
        EntityLink link = gameObject.GetEntityLink();
        link.Unlink();

        PoolManager.Instance.ReleaseObject(gameObject);
    }
}
