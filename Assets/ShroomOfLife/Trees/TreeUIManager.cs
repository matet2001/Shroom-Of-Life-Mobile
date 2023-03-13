using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeUIManager : MonoBehaviour
{
    [SerializeField] Transform selectedCircleTransform;
    [SerializeField] Button growButton;

    public bool isUIVisible { private set; get; }
    private bool shouldButtonBeVisible = true;

    public event Action OnGrowButtonPressed;

    private void Start()
    {
        RefreshUIActivity();

        growButton.onClick.AddListener(() => OnGrowButtonPressed?.Invoke());
    }
    private void Update()
    {
        RotateSelectedUI();
    }
    private void RotateSelectedUI()
    {
        if (!isUIVisible) return;
        selectedCircleTransform.Rotate(0f, 0f, 50f * Time.deltaTime, Space.Self);
    }
    public void SetUIActive(bool active)
    {
        isUIVisible = active;
        RefreshUIActivity();
    }
    private void RefreshUIActivity()
    {
        SetTargetCircleActive(isUIVisible);

        if (shouldButtonBeVisible) SetGrowButtonActive(isUIVisible);
        else SetGrowButtonActive(false);
    }
    private void SetTargetCircleActive(bool active) => selectedCircleTransform.gameObject.SetActive(active);
    private void SetGrowButtonActive(bool active) => growButton.gameObject.SetActive(active);
    public void SetGrowButtonShouldVisible(bool active)
    {
        shouldButtonBeVisible = active;
        RefreshUIActivity();
    }
}
