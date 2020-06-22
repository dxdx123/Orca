using System;
using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class MoveToAttackTarget : Action
    {
        public const float DISTANCE_NEARBY = 1f;

        public const float EPSILON = 1.19e-05f;
        
        public SharedGameObject target;

        private bool _chasing;

        private Vector3 _lastDestPosition;
        
        public override TaskStatus OnUpdate()
        {
            if (target == null || target.Value == null)
                return TaskStatus.Failure;
            
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
            float rightX = destX + DISTANCE_NEARBY;
            float leftX = destX - DISTANCE_NEARBY;

            float choice1, choice2;
            if (srcX >= destX)
            {
                choice1 = rightX;
                choice2 = leftX;
            }
            else
            {
                choice1 = leftX;
                choice2 = rightX;
            }
            
            // choice1
            var info1 = AstarPath.active.GetNearest(new Vector3(choice1, destY - EPSILON, 0.0f));
            var position1 = info1.position;
            if (Mathf.Approximately(position1.x, choice1))
            {
                return position1;
            }
            
            // choice2
            var info2 = AstarPath.active.GetNearest(new Vector3(choice2, destY - EPSILON, 0.0f));
            var position2 = info2.position;
            if (Mathf.Approximately(position2.x, choice2))
            {
                return position2;
            }
            
            throw new Exception("!!! Not found best choice");
        }

        public override void OnEnd()
        {
            if (target == null || target.Value == null)
                return;
            
            _chasing = false;
            _lastDestPosition = target.Value.transform.position;
        }
    }
}
