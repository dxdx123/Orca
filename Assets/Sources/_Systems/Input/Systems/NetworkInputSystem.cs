using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class NetworkInputSystem : ReactiveSystem<InputEntity>, ICleanupSystem
{
    private InputContext _inputContext;
    
    private readonly IGroup<InputEntity> _networkInputGroup;
    private readonly List<InputEntity> _cleanBuffer = new List<InputEntity>();
    
    public NetworkInputSystem(Contexts contexts)
        : base(contexts.input)
    {
        _inputContext = contexts.input;
        
        _networkInputGroup = _inputContext.GetGroup(InputMatcher.AnyOf(InputMatcher.NetworkMove, InputMatcher.NetworkAction));
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.NetworkMove.Added(), InputMatcher.NetworkAction.Added());
    }

    protected override bool Filter(InputEntity entity)
    {
        return true;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var e in entities)
        {
            if (e.hasNetworkAction)
            {
                InputEntity inputEntity = _inputContext.CreateEntity();
                
                inputEntity.AddInputAction(e.networkAction.characterAction);
            }
            else if (e.hasNetworkMove)
            {
                InputEntity inputEntity = _inputContext.CreateEntity();
                inputEntity.AddInputMove(e.networkMove.x, e.networkMove.y);
            }
            else
            {
                // nothing
            }
        }
    }

    public void Cleanup()
    {
        foreach (var e in _networkInputGroup.GetEntities(_cleanBuffer))
        {
            e.Destroy();
        }
    }
}
