using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class StateSystem : ReactiveSystem<GameEntity>
{
    public StateSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.State, GameMatcher.FSM));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasState && entity.hasFSM;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for (int i = 0, length = entities.Count; i < length; ++i)
        {
            var e = entities[i];

            string characterStateStr = e.state.state.GetCacheString();
            e.fSM.fsm.fsm.SendEvent(characterStateStr);
        }
    }
}
