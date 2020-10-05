using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicSystems : Feature
{
    public LogicSystems(Contexts contexts)
        : base("Initialize Systems")
    {
        Add(new InitializeSystem(contexts));

        Add(new LoadSceneSystem(contexts));
        Add(new DestroySceneSystem(contexts));
        
        Add(new CreatePlayerSystem(contexts));
        
        Add(new DestroyMapSystem(contexts));
        Add(new CreateMapSystem(contexts));
        Add(new CreateQuadTreeMapSystem(contexts));

        // Update
        Add(new EffectVelocitySystem(contexts));
        Add(new EffectDestroySystem(contexts));

        Add(new CharacterVelocitySystem(contexts));
        Add(new PositionSystem(contexts));

        Add(new CharacterDirectionSystem(contexts));
        Add(new EffectDirectionSystem(contexts));

        Add(new MovingAttempSystem(contexts));
        Add(new TranslateMovingSystem(contexts));
        Add(new MoveStateSystem(contexts));
        Add(new StopPathfindingSystem(contexts));
        Add(new PathfindingPositionSystem(contexts));

        Add(new UpdateQuadTreeMapSystem(contexts));
        Add(new CameraFollowSystem(contexts));
        
        // Pathfinding
        Add(new FindPathSystem(contexts));

        Add(new PuppySystem(contexts));

        // Attack
        Add(new AttackCoolDownSystem(contexts));
        Add(new AttackSystem(contexts));
        Add(new AttackEffectSystem(contexts));

        Add(new DestroyPathfindingSystem(contexts));
        Add(new DestroyViewSystem(contexts));
        
        // Buff
        Add(new BuffOccurSystem(contexts));
        
        // Debug
        Add(new DummyTestMoveSystem(contexts));
    }
}
