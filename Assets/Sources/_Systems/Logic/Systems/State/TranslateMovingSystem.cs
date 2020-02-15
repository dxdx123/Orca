
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class TranslateMovingSystem : IExecuteSystem
{
    private InputContext _inputContext;
    private GameContext _gameContext;
    
    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanGameBuffer = new List<GameEntity>();

    private IGroup<InputEntity> _inputMoveGroup;
    private readonly List<InputEntity> _cleanInputBuffer = new List<InputEntity>();
    
    public TranslateMovingSystem(Contexts contexts)
    {
        _inputContext = contexts.input;
        _gameContext = contexts.game;
        
        _underControlGroup = _gameContext.GetGroup(GameMatcher.UnderControl);
        _inputMoveGroup = _inputContext.GetGroup(InputMatcher.InputMove);
    }
    
    public void Execute()
    {
        var buffer = _inputMoveGroup.GetEntities(_cleanInputBuffer);

        bool hasInputMove = buffer.Count > 0;

        SetControlGroupMoving(hasInputMove);

        TranslateIdleToRunState();
    }

    private void TranslateIdleToRunState()
    {
        var buffer = _underControlGroup.GetEntities(_cleanGameBuffer);

        for (int i = 0, length = buffer.Count; i < length; ++i)
        {
            var e = buffer[i];

            if (e.isAttempMove && e.state.state == CharacterState.Idle)
            {
                e.ReplaceState(CharacterState.Run);
            }
        }

    }

    private void SetControlGroupMoving(bool hasInputMove)
    {
        var buffer = _underControlGroup.GetEntities(_cleanGameBuffer);

        for (int i = 0, length = buffer.Count; i < length; ++i)
        {
            var e = buffer[i];

            bool moving = hasInputMove || e.hasPathfindingMove;
            e.isAttempMove = moving;
        }
    }
}