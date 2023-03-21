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

        refreshTimeMax = refreshTimer;
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
            OnResourceRefresh?.Invoke();
        }
        else refreshTimer -= Time.deltaTime;
    }
    private void SetUI() => resourceTimerUIController.SetTimerText(refreshTimer);
    private void ManageGameEnd()
    {
        shouldCountDown = false;
        resourceTimerUIController.HideUI();
    } 
}
