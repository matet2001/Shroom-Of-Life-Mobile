using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManagment
{
    public class StateMachine : MonoBehaviour
    {
        public State currentState;

        public void Start()
        {
            currentState.OnEnter();
        }
        public void Update()
        {
            currentState.OnUpdate();
            TransitionToExitState();
            TransitionToStates();
        }
        private void TransitionToExitState()
        {
            if (currentState == null) return;
            
            if (!currentState.ExitCondiction()) return;

            ChangeState(currentState.exitState);
        }
        private void TransitionToStates()
        {
            if (currentState.stateTransitions.Length == 0) return;

            foreach (State state in currentState.stateTransitions)
            {
                if (!state.TransitionToThisState()) continue;

                ChangeState(state);
            }
        }
        public void ChangeState(State state)
        {
            currentState.OnExit();
            currentState = state;
            currentState.OnEnter();
        }
        public bool CheckState(State state) => state == currentState;
    }
    public abstract class State : MonoBehaviour
    {
        [HideInInspector]
        public State exitState;
        public State[] stateTransitions;

        public abstract void OnUpdate();
        public virtual void OnEnter()
        {
        }
        public virtual void OnExit()
        {

        }
        public virtual bool TransitionToThisState()
        {
            return false;
        }
        public virtual bool ExitCondiction()
        {
            return false;
        }
        public void Activate() => GetComponentInParent<StateMachine>().currentState = this;
        public bool isActive() => GetComponentInParent<StateMachine>().currentState == this;
    }
    public abstract class GameState : State
    {
        public GameObject stateVirtualCamera;

        public override void OnEnter()
        {
            stateVirtualCamera.SetActive(true);
        }
        public override void OnExit()
        {
            stateVirtualCamera.SetActive(false);
        }
    }
}
