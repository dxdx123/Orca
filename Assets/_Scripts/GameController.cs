using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Systems _systems;

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
        _systems.Execute();
        
        _systems.Cleanup();
    }

    private void OnDestroy()
    {
        _systems.TearDown();
    }
}
