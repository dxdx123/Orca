
using System;
using System.Collections.Generic;
using Entitas;

public class TranslateInputActionSystem : ReactiveSystem<InputEntity>
{
    private GameContext _gameContext;

    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public TranslateInputActionSystem(Contexts contexts)
        : base(contexts.input)
    {
        _gameContext = contexts.game;

        _underControlGroup = _gameContext.GetGroup(GameMatcher.UnderControl);
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.InputAction.Added());
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasInputAction;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        var input = entities.SingleEntity();

        CharacterAction action = input.inputAction.action;
        ChacaterState state = ParseActionState(action);
        
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            e.ReplaceState(state);
        }
    }

    private ChacaterState ParseActionState(CharacterAction action)
    {
        switch (action)
        {
            case CharacterAction.LightAttack1:
                return ChacaterState.LightAttack1;
            
            case CharacterAction.LightAttack2:
                return ChacaterState.LightAttack2;
            
            case CharacterAction.HeavyAttack1:
                return ChacaterState.HeavyAttack1;
            
            case CharacterAction.HeavyAttack2:
                return ChacaterState.HeavyAttack2;
            
            case CharacterAction.LevelUp:
                return ChacaterState.LevelUp;
            
            case CharacterAction.Die:
                return ChacaterState.Die;
            
            default:
                throw new Exception($"Invalid {action}");
        }
    }
}