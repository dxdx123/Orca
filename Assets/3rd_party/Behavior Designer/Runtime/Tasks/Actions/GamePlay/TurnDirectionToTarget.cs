using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TurnDirectionToTarget : Action
    {
        public SharedGameObject target;
        
        public override TaskStatus OnUpdate()
        {
            var srcEntity = gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(srcEntity);
            
            GameEntity targetEntity = target.Value.gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(targetEntity);

            float srcX = srcEntity.position.x;
            float destX = targetEntity.position.x;

            if (srcX > destX)
            {
                srcEntity.ReplaceDirection(CharacterDirection.Left);
                
                return TaskStatus.Success;
            }
            else if(srcX < destX)
            {
                srcEntity.ReplaceDirection(CharacterDirection.Right);
            
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
