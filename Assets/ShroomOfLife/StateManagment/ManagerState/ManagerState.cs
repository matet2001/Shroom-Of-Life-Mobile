using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManagment
{
    public class ManagerState : GameState
    { 
        public static event Action OnManagerStateEnter;
        public static event Action<Transform> OnManagerStateExit;
        
        private bool isCollisionHappened;

        private ManagerCameraMovementController cameraMovementController;
        private ManagerCameraDistanceChecker distanceChecker;

        [SerializeField] Transform cameraContainerTransform;
       
        private void Start()
        {
            Collidable.OnCollidableCollision += EnterManagmentState;

            cameraMovementController = GetComponentInChildren<ManagerCameraMovementController>();
            distanceChecker = GetComponentInChildren<ManagerCameraDistanceChecker>();

            cameraMovementController.cameraContainerTransform = cameraContainerTransform;
            distanceChecker.cameraContainerTransform = cameraContainerTransform;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            OnManagerStateEnter?.Invoke();

            cameraMovementController.ResetCameraSpeed();
        }
        public override void OnUpdate()
        {
            cameraMovementController.MoveCamera();
            distanceChecker.CheckDistanceFromTrees();
        }            
        public override void OnExit()
        {
            base.OnExit();
            OnManagerStateExit?.Invoke(cameraContainerTransform);
            
            cameraMovementController.ResetCamera();
            isCollisionHappened = false;
        }
        private void EnterManagmentState()
        {
            isCollisionHappened = true;
        }
        public override bool TransitionToThisState()
        {
            if (isCollisionHappened) return true;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                return true;
            }
            return false;
        }
    }
}
