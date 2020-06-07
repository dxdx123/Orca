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
        return context.CreateCollector(GameMatcher.CreatePlayer.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        var playerEntity = CreatePlayer();
        
        var puppyEntity = CreatePuppy(playerEntity);
        var enemyEntity = CreateEnemy(playerEntity);
        
        playerEntity.AddTarget(enemyEntity);
        
        var sheepList = CreateSheep();
        SheepManager.Instance.Initialize(sheepList);
    }

    private List<GameEntity> CreateSheep()
    {
        int length = 100;

        var list = new List<GameEntity>(length);

        for (int i = 0; i < length; ++i)
        {
            GameEntity e = _gameContext.CreateEntity();

            e.AddCharacter(Character.Sheep);

            float offsetX = Random.Range(-4f, 4f);
            float offsetY = Random.Range(-4f, 4f);
            
            e.AddPosition(10f + offsetX, 22f + offsetY);
            e.AddState(CharacterState.Idle);
            e.AddDirection(CharacterDirection.Right);
            
            list.Add(e);
        }

        return list;
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
    
    private GameEntity CreatePuppy(GameEntity entity)
    {
        GameEntity e = _gameContext.CreateEntity();

        e.AddAI(AIType.Puppy);
        e.AddTarget(entity);
        e.AddCharacter(Character.ArcherWildcat);
        e.AddPosition(12.0f, 21.5f);
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);

        return e;
    }

    private GameEntity CreateEnemy(GameEntity entity)
    {
        GameEntity e = _gameContext.CreateEntity();

        e.AddAI(AIType.Enemy);
        e.AddTarget(entity);
        e.AddCharacter(Character.Ettin);
        e.AddPosition(14.0f, 21.5f);
        e.AddState(CharacterState.Idle);
        e.AddDirection(CharacterDirection.Right);

        return e;
    }
}
