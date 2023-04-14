using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using StateManagment;

public class ResourceRefreshTimer : MonoBehaviour
{
    public static event Action OnResourceRefresh;

    [SerializeField]
    ResourceTimerUIController resourceTimerUIController;

    [SerializeField] float refreshTimer;
    private float refreshTimeMax;
    private bool shouldCountDown = true;

    private void Start()
    {
        WinState.OnWinGame += ManageGameEnd;
        LoseState.OnLoseGame += ManageGameEnd;
        
        TutorialManager.OnStageReveale += Pause;
        TutorialManager.OnStageHide += delegate (GameObject gm) { Continue(); };

        LevelSceneManager.OnRestart += RestartTimer;

        refreshTimeMax = refreshTimer;

        resourceTimerUIController.GetButton().onClick.AddListener(SkipCountdown);
    }
    private void Update()
    {
        ManageTimer();
    }
    private void ManageTimer()
    {
        if (!shouldCountDown) return;
        
        RefreshCountdown();
        SetUI();
    }
    private void RefreshCountdown()
    {
        if (refreshTimer <= 0)
        {
            refreshTimer = refreshTimeMax;
            SoundManager.Instance.PlaySound("Game/TurnHappen", transform.position);
            OnResourceRefresh?.Invoke();
        }
        else refreshTimer -= Time.deltaTime;
    }
    public void SkipCountdown()
    {
        refreshTimer = 0;
    }
    private void SetUI() => resourceTimerUIController.SetTimerText(refreshTimer);
    private void ManageGameEnd()
    {
        Pause();
        resourceTimerUIController.HideUI();
    }
    private void Pause() => shouldCountDown = false;
    private void Continue() => shouldCountDown = true;
    private void RestartTimer()
    {
        refreshTimer = refreshTimeMax;
        resourceTimerUIController.ShowUI();
        Continue();
        SetUI();
    }
}
