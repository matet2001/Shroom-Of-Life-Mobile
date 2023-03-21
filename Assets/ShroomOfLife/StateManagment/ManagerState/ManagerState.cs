using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManagment
{
    public class ManagerState : EventableState
    {
        public static event Action<Transform> OnManagerStateInit;
        public static event Action OnManagerStateEnter;
        public static event Action<Transform> OnManagerStateExit;      

        [SerializeField] Transform cameraContainerTransform;
       
        private void Start()
        {
            Collidable.OnCollidableCollision += TriggerTransitionEvent;
            YarnMovementController.OnRunOutOfYarnResource += TriggerTransitionEvent;

            OnManagerStateInit?.Invoke(cameraContainerTransform);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            OnManagerStateEnter?.Invoke();
        }
        public override void OnUpdate()
        {
           
        }            
        public override void OnExit()
        {
            base.OnExit();
            OnManagerStateExit?.Invoke(cameraContainerTransform);
        }      
        public override bool TransitionToThisState()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                return true;
            }
            return false;
        }
    }
}
