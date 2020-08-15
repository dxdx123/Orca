using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IViewController
{
    tk2dSprite displaySprite { get; }
    tk2dSpriteAnimator displaySpriteAnimator { get; }

    GameObject gameObject { get; }

    void Initialize(GameEntity entity);
    void Destroy();

    void AddBuff(string buffName);
    void RemoveBuff(string buffName);
    void TriggerBuff(string buffName);
}
