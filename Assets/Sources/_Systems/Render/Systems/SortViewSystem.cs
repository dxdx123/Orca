using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class SortViewSystem : IExecuteSystem
{
    private IGroup<GameEntity> _displayGroup;
    private List<GameEntity> _cleanCache = new List<GameEntity>();
    
    public SortViewSystem(Contexts contexts)
    {
        _displayGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.View));
    }

    public void Execute()
    {
        SortCharacterSortingOrder();
    }

    // maybe we can Reduce DrawCall by topological sorting, just like uGUI
    private void SortCharacterSortingOrder()
    {
        var list = _displayGroup.GetEntities(_cleanCache);

        list.Sort((a, b) => a.position.y <= b.position.y ? 1 : -1);

        for (int i = 0, length = list.Count; i < length; ++i)
        {
            GameEntity e = list[i];

            tk2dSprite sprite = e.view.viewController.displaySprite;
            sprite.SortingOrder = i;
        }
    }
}
