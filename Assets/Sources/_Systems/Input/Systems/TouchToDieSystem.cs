using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class TouchToDieSystem : ReactiveSystem<GameEntity>
{
    public TouchToDieSystem(Contexts contexts)
        : base(contexts.game)
    {
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Touch.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasState;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            // entity.ReplaceState(CharacterState.Die);
            // SheepManager.Instance.BlowSheep(entity);
            
            entity.isTouch = false;
        }
    }
}
