using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChaseTarget : Action
    {
        public const float DISTANCE_NEARBY = 1f;
        
        public SharedGameObject target;

        private bool _chasing;

        private Vector3 _lastDestPosition;
        
        public override TaskStatus OnUpdate()
        {
            var srcEntity = gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(srcEntity);

            GameEntity targetEntity = target.Value.gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(targetEntity);

            var srcPos = new Vector3(srcEntity.position.x, srcEntity.position.y, 0.0f);
            var destPos = target.Value.transform.position;
            bool isTargetNotMove = (destPos == _lastDestPosition); // approximate
            _lastDestPosition = destPos;
            
            if (_chasing)
            {
                if (targetEntity.state.state == CharacterState.Run && !isTargetNotMove)
                {
                    var bestPos = FindBestPosition(srcPos.x, srcPos.y, destPos.x, destPos.y);
                    
                    srcEntity.ReplaceFindPath(bestPos.x, bestPos.y, true);
                    return TaskStatus.Running;
                }
                else
                {
                    if (srcEntity.hasPathfinding)
                    {
                        return TaskStatus.Running;
                    }
                    else
                    {
                        return TaskStatus.Success;
                    }
                }
            }
            else
            {
                var bestPos = FindBestPosition(srcPos.x, srcPos.y, destPos.x, destPos.y);
                
                srcEntity.AddFindPath(bestPos.x, bestPos.y, true);
                _chasing = true;
                
                return TaskStatus.Running;
            }
        }

        private Vector2 FindBestPosition(float srcX, float srcY, float destX, float destY)
        {
            if (srcX > destX)
            {
                float rightX = destX + DISTANCE_NEARBY;
                var rightInfo = AstarPath.active.GetNearest(new Vector3(rightX, destY, 0.0f));
                Vector2 rightPosition = rightInfo.position;

                if (Mathf.Approximately(rightPosition.x, rightX))
                {
                    return rightPosition;
                }
                else
                {
                    float leftX = destX - DISTANCE_NEARBY;
                    var leftInfo = AstarPath.active.GetNearest(new Vector3(leftX, destY, 0.0f));
                    Vector2 leftPosition = leftInfo.position;

                    return leftPosition; 
                }
            }
            else if (srcX < destX)
            {
                float leftX = destX - DISTANCE_NEARBY;
                var leftInfo = AstarPath.active.GetNearest(new Vector3(leftX, destY, 0.0f));
                Vector2 leftPosition = leftInfo.position;

                if (Mathf.Approximately(leftPosition.x, leftX))
                {
                    return leftPosition;
                }
                else
                {
                    float rightX = destX + DISTANCE_NEARBY;
                    var rightInfo = AstarPath.active.GetNearest(new Vector3(rightX, destY, 0.0f));
                    Vector2 rightPosition = rightInfo.position;

                    return rightPosition;
                }
            }
            else
            {
                float leftX = destX - DISTANCE_NEARBY;
                var leftInfo = AstarPath.active.GetNearest(new Vector3(leftX, destY, 0.0f));
                Vector2 leftPosition = leftInfo.position;
                
                float rightX = destX + DISTANCE_NEARBY;
                var rightInfo = AstarPath.active.GetNearest(new Vector3(rightX, destY, 0.0f));
                Vector2 rightPosition = rightInfo.position;

                return 
                    rightPosition.x > leftPosition.x
                    ? rightPosition
                    : leftPosition;
            }
        }

        public override void OnReset()
        {
            _chasing = false;
            _lastDestPosition = target.Value.transform.position;
        }
    }
}
