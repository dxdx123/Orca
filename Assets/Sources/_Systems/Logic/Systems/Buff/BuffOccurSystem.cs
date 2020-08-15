using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Assertions;

public class BuffOccurSystem : ReactiveSystem<GameEntity>
{
    public BuffOccurSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BuffOccur.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isBuffOccur;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var gameEntity in entities)
        {
            IBuffOccur buff = gameEntity.buffOccurAction.buff;
            Assert.IsNotNull(buff);

            buff.OnOccur(gameEntity);
            
            gameEntity.isBuffOccur = false;
        }
    }
}
