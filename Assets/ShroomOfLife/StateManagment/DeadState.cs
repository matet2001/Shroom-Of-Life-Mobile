using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManagment
{
    public class DeadState : GameState
    {
        public override void OnUpdate()
        {
            if (Input.anyKeyDown)
            {
                LevelSceneManager.RestartScene();
            }
        }
        public override bool TransitionToThisState()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                return true;
            }
            return false;
        }
    }
}
