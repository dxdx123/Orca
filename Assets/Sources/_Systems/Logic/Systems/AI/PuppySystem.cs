using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entitas;
using UnityEngine;

public class PuppySystem : ReactiveSystem<GameEntity>
{
    private readonly IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public PuppySystem(Contexts contexts)
        : base(contexts.game)
    {
        var gameContext = contexts.game;

        _underControlGroup = gameContext.GetGroup(GameMatcher.UnderControl);
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BehaviorTree.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        var list = _underControlGroup.GetEntities(_cleanBuffer);
        var underControlEntity = list.SingleEntity();
        
        var gameObject = underControlEntity.view.viewController.gameObject;
        
        foreach (var e in entities)
        {
            var behaviorTree = e.behaviorTree.behaviorTree;
            // behaviorTree.SetVariable("Master", (SharedGameObject)gameObject);
            behaviorTree.EnableBehavior();
        }
    }
}
