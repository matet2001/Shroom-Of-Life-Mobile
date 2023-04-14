using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace StateManagment
{
    public class ConquerState : EventableState
    {
        public static event Action OnConquerStateEnter;
        public static event Action OnConquerStateExit;

        private CinemachineVirtualCamera virtualCamera;
        [SerializeField] Transform cameraContainerTransform;

        private void Start()
        {        
            YarnMovementController.OnYarnStart += OnYarnStart;
        }
        private void OnYarnStart(Vector2 position)
        {
            stateVirtualCamera.transform.position = new Vector3(position.x, position.y, -10f);

            Transform globeTransform = cameraContainerTransform;
            Vector2 globeCenter = globeTransform.position;
            Vector2 yarnToCenterVector = position - globeCenter;
            float angle = Vector2.SignedAngle(Vector2.up, yarnToCenterVector.normalized);

            stateVirtualCamera.transform.eulerAngles = new Vector3(0f, 0f, angle);
            TriggerTransitionEvent();
        }
        public override void OnEnter()
        {
            base.OnEnter();

            OnConquerStateEnter?.Invoke();
            SetCameraPositionToYarn();
        }
        private void SetCameraPositionToYarn()
        {
            if(!virtualCamera) virtualCamera = stateVirtualCamera.GetComponent<CinemachineVirtualCamera>();

            Vector3 positionToSet = virtualCamera.Follow.position;
            positionToSet.z = -10f;
            stateVirtualCamera.transform.position = positionToSet;
        }
        public override void OnUpdate()
        {
             
        }
        public override void OnExit()
        {
            base.OnExit();
            OnConquerStateExit?.Invoke();
        }
        //private void SwitchToStopCamera(Vector2 positionToSet)
        //{
        //    Debug.Log("Switched to stop camera");
        //    stopCamera.transform.position = new Vector3(positionToSet.x, positionToSet.y, -10f);
        //    stopCamera.SetActive(true);
        //    stateVirtualCamera.SetActive(false);
        //}
        //private void SwitchToFollowCamera(Vector2 positionToSet)
        //{
        //    Debug.Log("Switched to follow camera");
        //    stopCamera.transform.position = new Vector3(positionToSet.x, positionToSet.y, -10f);
        //    stopCamera.SetActive(false);
        //    stateVirtualCamera.SetActive(true);
        //}
    }
}
