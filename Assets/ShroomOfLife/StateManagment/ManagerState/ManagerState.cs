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

        private Coroutine transitionCountDownCoroutine;

        private void Start()
        {
            Collidable.OnCollidableCollision += TriggerTransitionEvent;
            YarnMovementController.OnRunOutOfYarnResource += TriggerTransitionEvent;

            LevelSceneManager.OnRestart += TriggerTransitionEvent;

            OnManagerStateInit?.Invoke(cameraContainerTransform);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            transitionCountDownCoroutine = StartCoroutine(TransitionCountDown());
        }

        public override void OnUpdate()
        {
           
        }            
        public override void OnExit()
        {
            base.OnExit();
            if(transitionCountDownCoroutine != null) StopCoroutine(transitionCountDownCoroutine);
            OnManagerStateExit?.Invoke(cameraContainerTransform);
        }      
        public IEnumerator TransitionCountDown()
        {
            yield return new WaitForSeconds(transitionTime);
            OnManagerStateEnter?.Invoke();
        }
    }
}
