using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewController : ViewController, IFSM
{
    private PlayMakerFSM _fsm;
    
    public PlayMakerFSM fsm
    {
        get { return _fsm; }
    }

    protected override void Awake()
    {
        base.Awake();
        
        _fsm = GetComponent<PlayMakerFSM>();
    }

    public override void Initialize(GameEntity entity)
    {
        base.Initialize(entity);

        var containerGo = _fsm.FsmVariables.GetFsmGameObject("containerGo");
        containerGo.Value = gameObject;

        _fsm.enabled = true;
    }

    public override void Destroy()
    {
        _fsm.enabled = false;

        base.Destroy();
    }
}
