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
        Debug.Log("Game won!");
    }
    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelSceneManager.RestartScene();
        }
    }
    public override void OnExit()
    {
        base.OnExit();
    }

}
