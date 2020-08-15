using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffOccur
{
    void OnOccur(GameEntity gameEntity);
}

public interface IBuffRemoved
{
void OnRemoved(GameEntity gameEntity);
}
    
public interface IBuffOnBeHurt
{
    float OnBeHurt(GameEntity attacker, GameEntity victim, float damage);
}
public interface IBuffOnHit
{
    void OnHit(GameEntity attacker, GameEntity victim);
}
    
public interface IBuffOnBeforeKilled
{
    void OnBeforeKilled(GameEntity gameEntity);
}
public interface IBuffOnAfterKilled
{
    void OnAfterKilled(GameEntity gameEntity);
}
