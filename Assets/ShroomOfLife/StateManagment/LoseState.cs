using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StateManagment
{
    public class LoseState : EventableState
    {
        public static event Action OnLoseGame;

        private void Start()
        {
            ResourceManager.OnLoseGame += TriggerTransitionEvent;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            OnLoseGame?.Invoke();
            Debug.Log("Game lost!");
        }
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                LevelSceneManager.RestartScene();
            }
        }
    }
}
