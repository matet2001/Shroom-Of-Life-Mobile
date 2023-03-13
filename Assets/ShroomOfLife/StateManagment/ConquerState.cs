using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace StateManagment
{
    public class ConquerState : GameState
    {
        public static event Action OnConquerStateEnter;
        public static event Action OnConquerStateExit;

        [SerializeField] GameObject stopCamera;

        private void Start()
        {
            YarnCrosshairController.OnYarnCrosshairEnter += (Vector2 position) => SwitchToStopCamera(position);
            YarnCrosshairController.OnYarnCrosshairExit += (Vector2 position) => SwitchToFollowCamera(position);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            OnConquerStateEnter?.Invoke();
        }
        public override void OnUpdate()
        {
             
        }
        public override void OnExit()
        {
            base.OnExit();
            OnConquerStateExit?.Invoke();
        }
        private void SwitchToStopCamera(Vector2 positionToSet)
        {
            Debug.Log("Switched to stop camera");
            stopCamera.transform.position = new Vector3(positionToSet.x, positionToSet.y, -10f);
            stopCamera.SetActive(true);
            stateVirtualCamera.SetActive(false);
        }
        private void SwitchToFollowCamera(Vector2 positionToSet)
        {
            Debug.Log("Switched to follow camera");
            stopCamera.transform.position = new Vector3(positionToSet.x, positionToSet.y, -10f);
            stopCamera.SetActive(false);
            stateVirtualCamera.SetActive(true);
        }
        public override bool TransitionToThisState()
        {
            if (InputManager.IsMouseRightClick())
            {
                return true;
            }
            return false;
        }
    }
}
