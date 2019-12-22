using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IViewController
{
    tk2dSprite displaySprite { get; }
    tk2dSpriteAnimator displaySpriteAnimator { get; }

    void Initialize(GameEntity entity);
    void Destroy();
}
