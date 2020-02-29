using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Character
{
    Priest,
    Archer,
    ArcherWildcat,
}

public enum CharacterAction
{
    LightAttack1,
    LightAttack2, 
    HeavyAttack1,
    HeavyAttack2, 
    LevelUp, 
    Die, 
}

public enum CharacterState
{
    Idle,
    Run,
    LightAttack1,
    LightAttack2, 
    HeavyAttack1,
    HeavyAttack2, 
    LevelUp, 
    Die, 
}

public enum CharacterDirection
{
    Left,
    Right,
}
