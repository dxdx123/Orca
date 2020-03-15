using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DestroyViewSystem : ReactiveSystem<GameEntity>
{
    public DestroyViewSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Destroy.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasView;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            CleanupAsset(e);
            ReleaseGameObject(e);
            
            e.Destroy();
        }
    }

    private void CleanupAsset(GameEntity e)
    {
        CleanAssetManager.Instance.CleanAsset(e);
    }

    private void ReleaseGameObject(GameEntity e)
    {
        var viewController = e.view.viewController;
        viewController.Destroy();
            
        if (e.isPoolAsset)
        {
            AssetPoolManager.Instance.DestroyInstance(viewController.gameObject);
        }
        else
        {
            Object.Destroy(viewController.gameObject);
        }
    }
}
