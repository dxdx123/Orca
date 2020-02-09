using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CreatePlayerSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public CreatePlayerSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Config.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        CreatePlayer();
    }
    
    private void CreatePlayer()
    {
        GameEntity e = _gameContext.CreateEntity();

        e.isUnderControl = true;
        e.AddCharacter(Character.Priest);
        e.AddPosition(15.0f, 20.0f);
        e.isCameraTarget = true;
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);
    }

}
