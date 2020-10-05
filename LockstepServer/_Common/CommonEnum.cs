using System.Collections;
using System.Collections.Generic;


public enum Character
{
    Priest,
    Archer,
    ArcherWildcat,
    Ettin,
    Sheep,
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
    Up,
    Right,
    Down,
    Left,
}

public enum AIType
{
    Puppy,
    Enemy,
    Neutral,
}
