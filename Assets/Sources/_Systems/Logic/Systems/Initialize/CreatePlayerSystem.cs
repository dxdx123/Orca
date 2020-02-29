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
        CreatePuppy();
    }

    private void CreatePlayer()
    {
        GameEntity e = _gameContext.CreateEntity();

        e.isUnderControl = true;
        e.AddCharacter(Character.Priest, false);
        e.AddPosition(24.0f, 31.5f);
        e.isCameraTarget = true;
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);
    }
    
    private void CreatePuppy()
    {
        GameEntity e = _gameContext.CreateEntity();

        e.isAI = true;
        e.AddCharacter(Character.ArcherWildcat, true);
        e.AddPosition(23.0f, 31.5f);
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);
    }

}
