using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Assertions;

public class EntityPathfinding : AIPath
{
    private GameEntity _entity;
    
    protected override void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
    {
        base.MovementUpdateInternal(deltaTime, out nextPosition, out nextRotation);

        if (_entity != null)
        {
            _entity.ReplacePathfindingMove(deltaTime, nextPosition);
        }
        else
        {
            // when stop has one chance callback
        }
    }

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        
        // cleanup & return to pool
        Cleanup();
        ReturnToPool();
    }

    public void GotoDestination(GameEntity entity, Vector3 oldPosition, Vector3 newPosition, bool exactDest)
    {
        Assert.IsNotNull(entity);
        
        _entity = entity;

        this.whenCloseToDestination =
            exactDest ? CloseToDestinationMode.ContinueToExactDestination : CloseToDestinationMode.Stop;
        
        if (IsPathfinding())
        {
            transform.position = oldPosition;
            this.destination = newPosition;
        }
        else
        {
            transform.position = oldPosition;
            this.destination = newPosition;
            EnablePathfinding();
        }
    }

    private bool IsPathfinding()
    {
        return !this.isStopped;
    }

    public void EnablePathfinding()
    {
        this.canMove = true;
        this.canSearch = true;
        this.isStopped = false;
    }

    public void DisablePathfinding()
    {
        this.canMove = false;
        this.canSearch = false;
        this.isStopped = true;
    }

    public void Cleanup()
    {
        _entity.RemovePathfinding();
        _entity.RemovePathfindingMove();
        
        _entity = null;
        
        this.destination = Vector3.zero;
        transform.position = Vector3.zero;
        DisablePathfinding();
    }

    private void ReturnToPool()
    {
        PoolCacheManager.Instance.ReturnEntityPathfinding(this);
    }
}
