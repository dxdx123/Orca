﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicSystems : Feature
{
    public LogicSystems(Contexts contexts)
        : base("Initialize Systems")
    {
        Add(new InitializeSystem(contexts));
        Add(new CreatePlayerSystem(contexts));

        Add(new VelocitySystem(contexts));
        Add(new PositionSystem(contexts));

        Add(new MovingAttempSystem(contexts));
        Add(new MoveStateSystem(contexts));
        Add(new TranslateMovingSystem(contexts));
    }
}
