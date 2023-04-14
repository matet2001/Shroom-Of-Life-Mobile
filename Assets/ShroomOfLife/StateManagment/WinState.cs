using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : EventableState
{
    public static event Action OnWinGame;
    private void Start()
    {
        ConnectionManager.OnWinGame += TriggerTransitionEvent;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        OnWinGame?.Invoke();
        SoundManager.Instance.PlaySound("Game/Win", transform.position);
    }
    public override void OnUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelSceneManager.RestartScene();
        }
#endif
    }
    public override void OnExit()
    {
        base.OnExit();
    }

}
