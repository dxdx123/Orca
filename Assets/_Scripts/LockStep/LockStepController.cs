using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(-999)]
public class LockStepController : MonoBehaviour
{
    private static LockStepController _instance;

    public static LockStepController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LockStepController>();
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }
    }

    private PlayerLockStep _playerLockStep;
    
    private Systems _systems;

    private bool _canMoveNext = true;
    public bool GameStart;
    
    private void Awake()
    {
        _playerLockStep = new PlayerLockStep(this, true);
    }
    
    private void Start()
    {
        var contexts = Contexts.sharedInstance;

        _systems = CreateSystems(contexts);
        
        _systems.Initialize();
    }

    private Systems CreateSystems(Contexts contexts)
    {
        return new Feature("Systems")
                .Add(new InputSystems(contexts))
                .Add(new LogicSystems(contexts))
                .Add(new RenderSystems(contexts))
            ;
    }

    private void Update()
    {
        if (GameStart)
        {
            _playerLockStep.OnUpdate(Time.deltaTime);
        }
        else
        {
            // nothing
        }

        if (_canMoveNext)
        {
            UpdateSystems();
            _canMoveNext = false;
        }
        else
        {
            // nothing
        }
    }

    private void UpdateSystems()
    {
        _systems.Execute();
        _systems.Cleanup();
    }

    public void OnGameTurn()
    {
        _canMoveNext = true;
    }

    private void OnDestroy()
    {
        _systems.TearDown();
    }
    
    public void OnOtherUserSendActionRes(LockstepPacketRes res)
    {
        _playerLockStep.OnOtherUserSendActionRes(res);
    }

    public void OnServerConfirmAction(LockstepConformRes res)
    {
        _playerLockStep.OnServerConfirmAction(res);
    }

    public void AddAction(IAction action)
    {
        _playerLockStep.AddAction(action);
    }
}
