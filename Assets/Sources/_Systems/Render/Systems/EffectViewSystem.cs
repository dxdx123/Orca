using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class EffectViewSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public EffectViewSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Effect.Added());
    }

    protected override bool Filter(GameEntity entity)
    { 
        return entity.hasEffect && entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var effectName = e.effect.effectName;
            var position = e.position;
         
            CreateEffect(e, effectName, position.x, position.y);
        }
    }

    private void CreateEffect(GameEntity e, string effectName, float x, float y)
    {
        GameObject gameObject = AssetPoolManager.Instance.SpawnEffect();
        gameObject.transform.position = new Vector3(x, y, 0);
        e.isPoolAsset = true;

        var animPath = _gameContext.config.effectConfig.GetEffectConfig(effectName);
        ResourceManager.Instance.GetAsset<GameObject>(animPath, this)
            .Then(spriteAnimationGo =>
            {
                tk2dSpriteAnimator tk2dAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();

                tk2dSpriteAnimation spriteAnimation = spriteAnimationGo.GetComponent<tk2dSpriteAnimation>();
                tk2dAnimator.Library = spriteAnimation;
                
                // Add Components
                e.AddTransform(gameObject.transform);
                
                var viewController = gameObject.GetComponent<ViewController>();
                viewController.Initialize(e);
                e.AddView(viewController);

                tk2dAnimator.AnimationCompleted = (animator, clip) => { e.isDestroy = true; };
                
                tk2dAnimator.Play(effectName);
            })
            .Catch(ex => Debug.LogException(ex));

        CleanAssetManager.Instance.RegisterCleanAssetActions(e,
            () => { ResourceManager.Instance.DestroyAsset(animPath, this); });
    }
}
