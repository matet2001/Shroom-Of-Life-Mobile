using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreeUIManager : MonoBehaviour
{
    public bool isUIVisible { private set; get; }
    [SerializeField] GameObject Canvas;

    public event Action OnGrowButtonPressed;

    [SerializeField] Button growButton;
    [SerializeField] TextMeshProUGUI growCostText;
    private bool shouldButtonBeVisible = true;

    [SerializeField] Transform selectedCircleTransform;

    private void Start()
    {
        RefreshUIActivity();

        growButton.onClick.AddListener(() => OnGrowButtonPressed?.Invoke());
    }
    private void Update()
    {
        RotateSelectedUI();
    } 
    public void SetUIActive(bool active)
    {
        isUIVisible = active;
        RefreshUIActivity();
    }
    private void RefreshUIActivity()
    {
        SetTargetCircleActive(isUIVisible);

        if (shouldButtonBeVisible) SetUIACtive(isUIVisible);
        else SetUIACtive(false);
    }
    #region Manage Target Circle
    private void RotateSelectedUI()
    {
        if (!isUIVisible) return;
        selectedCircleTransform.Rotate(0f, 0f, 50f * Time.deltaTime, Space.Self);
    }
    private void SetTargetCircleActive(bool active) => selectedCircleTransform.gameObject.SetActive(active);
    #endregion
    #region Manage Grow 
    private void SetUIACtive(bool active) => Canvas.gameObject.SetActive(active);
    public void SetGrowButtonShouldVisible(bool active)
    {
        shouldButtonBeVisible = active;
        RefreshUIActivity();
    }
    public void SetGrowCostText(float growCost) => growCostText.text = growCost.ToString();
    #endregion
}
