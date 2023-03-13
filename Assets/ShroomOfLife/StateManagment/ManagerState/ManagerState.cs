using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManagment
{
    public class ManagerState : GameState
    { 
        public static event Action OnManagerStateEnter;
        private bool isCollisionHappened;

        private ManagerCameraMovementController cameraMovementController;
        private ManagerCameraDistanceChecker distanceChecker;

        [SerializeField] Transform cameraContainerTransform;
       
        private void Start()
        {
            StoneCollisionManager.OnStoneCollision += EnterManagmentState;
            GlobeCollisionController.OnYarnExitGlobe += EnterManagmentState;
            TreeController.OnTreeCollision += (treeController) => EnterManagmentState();//delegate(TreeController treeController) { EnterManagmentState; };

            cameraMovementController = GetComponentInChildren<ManagerCameraMovementController>();
            distanceChecker = GetComponentInChildren<ManagerCameraDistanceChecker>();

            cameraMovementController.cameraTransform = stateVirtualCamera.transform;
            distanceChecker.cameraTransform = stateVirtualCamera.transform;
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
