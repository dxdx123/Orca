﻿using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChaseTarget : Action
    {
        public SharedGameObject target;

        private bool _chasing;

        private Vector3 _lastDestPosition;
        
        public override TaskStatus OnUpdate()
        {
            var srcEntity = gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(srcEntity);

            GameEntity targetEntity = target.Value.gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(targetEntity);

            var destPos = target.Value.transform.position;
            bool isTargetNotMove = (destPos == _lastDestPosition); // approximate
            _lastDestPosition = destPos;
            
            if (_chasing)
            {
                if (targetEntity.state.state == CharacterState.Run && !isTargetNotMove)
                {
                    srcEntity.ReplaceFindPath(destPos.x, destPos.y, false);
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
                srcEntity.AddFindPath(destPos.x, destPos.y, false);
                _chasing = true;
                
                return TaskStatus.Running;
            }
        }

        public override void OnReset()
        {
            _chasing = false;
            _lastDestPosition = target.Value.transform.position;
        }
    }
}
