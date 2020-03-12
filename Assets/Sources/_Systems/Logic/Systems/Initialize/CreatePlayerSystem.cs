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
        var entity = CreatePlayer();
        
        // CreatePuppy(entity);
        // CreateEnemy(entity);
    }

    private GameEntity CreatePlayer()
    {
        GameEntity e = _gameContext.CreateEntity();

        e.isUnderControl = true;
        e.AddCharacter(Character.Priest);
        e.AddPosition(10.0f, 22.0f);
        e.isCameraTarget = true;
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);

        return e;
    }
    
    private void CreatePuppy(GameEntity entity)
    {
        GameEntity e = _gameContext.CreateEntity();

        e.AddAI(AIType.Puppy);
        e.AddTarget(entity);
        e.AddCharacter(Character.ArcherWildcat);
        e.AddPosition(12.0f, 21.5f);
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);
    }

    private void CreateEnemy(GameEntity entity)
    {
        GameEntity e = _gameContext.CreateEntity();

        e.AddAI(AIType.Enemy);
        e.AddTarget(entity);
        e.AddCharacter(Character.Ettin);
        e.AddPosition(14.0f, 21.5f);
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);
    }
}
