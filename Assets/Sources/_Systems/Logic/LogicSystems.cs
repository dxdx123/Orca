using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicSystems : Feature
{
    public LogicSystems(Contexts contexts)
        : base("Initialize Systems")
    {
        Add(new InitializeSystem(contexts));
        Add(new PoolSystem(contexts));
        
        Add(new CreatePlayerSystem(contexts));
        
        Add(new CreateMapSystem(contexts));
        Add(new CreateQuadTreeMapSystem(contexts));

        Add(new VelocitySystem(contexts));
        Add(new PositionSystem(contexts));
        Add(new PathfindingPositionSystem(contexts));

        Add(new MovingAttempSystem(contexts));
        Add(new MoveStateSystem(contexts));
        Add(new TranslateMovingSystem(contexts));

        Add(new UpdateQuadTreeMapSystem(contexts));
        Add(new CameraFollowSystem(contexts));
        
        // Pathfinding
        Add(new DummyTestMoveSystem(contexts));
        Add(new FindPathSystem(contexts));

        Add(new PuppySystem(contexts));
    }
}
