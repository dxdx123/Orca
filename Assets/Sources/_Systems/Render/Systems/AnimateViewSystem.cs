using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
public class AnimateViewSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public AnimateViewSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnimatorState.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasAnimatorState && entity.hasView && entity.hasCharacter && entity.hasDirection;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            PlayAnimator(e);

            e.RemoveAnimatorState();
        }
    }

    private void PlayAnimator(GameEntity e)
    {
        if (e.animatorState.state == CharacterState.Run)
        {
            PlayAnimatorRun(e);
        }
        else
        {
            var animator = e.view.viewController.displaySpriteAnimator;
            var stateString = e.animatorState.state.GetCacheString();

            animator.Play(stateString);
        }
    }

    private void PlayAnimatorRun(GameEntity e)
    {
        var character = e.character.character;

        AnimatorRunConfig config = _gameContext.config.animatorRunConfig.GetAnimatorRunConfig(character);

        CharacterDirection direction = e.direction.direction;
        string animatorString = GetAnimatorString(config, direction);

        var animator = e.view.viewController.displaySpriteAnimator;
        animator.Play(animatorString);
    }

    private string GetAnimatorString(AnimatorRunConfig config, CharacterDirection direction)
    {
        switch (direction)
        {
            case CharacterDirection.Up:
                return config.animatorUp;
            
            case CharacterDirection.Right:
                return config.animatorRight;
            
            case CharacterDirection.Down:
                return config.animatorDown;
            
            case CharacterDirection.Left:
                return config.animatorLeft;
            
            default:
                throw new Exception("!!! Invalid direction");
        }
    }
    
}
