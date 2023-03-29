using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreeUIManager : MonoBehaviour
{
    public static event Action OnFirstUIShow;
    private bool isUIShowned;
    
    public bool isUIVisible { private set; get; }
    [SerializeField] GameObject Canvas;

    public TreeGrowButtonController growButtonController;
    [SerializeField] TextMeshProUGUI growCostText;
    private bool shouldButtonBeVisible = true;
    [SerializeField] Transform selectedCircleTransform;

    private void Start()
    {
        RefreshUIActivity();
        OnFirstUIShow += delegate () { isUIShowned = true; };
    }
    private void Update()
    {
        RotateSelectedUI();
    }
    public void SetUIActive(bool active)
    {
        isUIVisible = active;
        if (active && !isUIShowned) OnFirstUIShow?.Invoke();
        RefreshUIActivity();
    }
    private void RefreshUIActivity()
    {
        SetTargetCircleActive(isUIVisible);

        if (shouldButtonBeVisible) SetCanvasActive(isUIVisible);
        else SetCanvasActive(false);
    }
    private void SetCanvasActive(bool active) => Canvas.gameObject.SetActive(active);
    
    #region Manage Target Circle
    private void RotateSelectedUI()
    {
        if (!isUIVisible) return;
        selectedCircleTransform.Rotate(0f, 0f, 50f * Time.deltaTime, Space.Self);
    }
    private void SetTargetCircleActive(bool active) => selectedCircleTransform.gameObject.SetActive(active);
    #endregion
    #region Manage Grow   
    public void SetGrowButtonShouldVisible(bool active)
    {
        shouldButtonBeVisible = active;
        RefreshUIActivity();
    }
    public void SetGrowCostText(float growCost) => growCostText.text = growCost.ToString();
    #endregion
}
