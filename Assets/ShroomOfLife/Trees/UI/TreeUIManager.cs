using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreeUIManager : MonoBehaviour
{
    //For tutorial
    public UnityEvent OnFirstUIShow;
    private bool isUIShowned;
    
    //UI
    public bool isUIVisible { private set; get; }
    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject growPanel, resourcePanel;

    //Grow
    public TreeGrowButtonController growButtonController;
    [SerializeField] TextMeshProUGUI growCostText;

    [SerializeField] Transform selectedCircleTransform;

    private GameObject focusSound;
    private bool isPaused;

    private void Start()
    {
        RefreshUIActivity();

        TutorialManager.OnStageReveale += delegate { isPaused = true; };
        TutorialManager.OnStageHide += delegate { isPaused = false; };
    }
    private void Update()
    {
        RotateSelectedUI();
    }
    //Call from manager camera distance checker
    public void SetUIActive(bool active)
    {
        isUIVisible = active;

        RefreshUIActivity();
        
        //Tutorial
        if (active && !isUIShowned)
        {
            isUIShowned = true;
            OnFirstUIShow?.Invoke();
        }  
    }
    private void RefreshUIActivity()
    {
        SetTargetCircleActive(isUIVisible);
        SetCanvasActive(isUIVisible);

        
    }
    private void SetCanvasActive(bool active) => Canvas.gameObject.SetActive(active);  
    #region Manage Target Circle
    private void RotateSelectedUI()
    {
        if (!isUIVisible)
        {
            if (focusSound)
            {
                SoundManager.Instance.StopSound(focusSound);
                focusSound = null;
            }
            return;
        }
        
        if (isPaused) return;
        selectedCircleTransform.Rotate(0f, 0f, 50f * Time.deltaTime, Space.Self);

        if(!focusSound)
        {
            focusSound = SoundManager.Instance.PlaySoundLooped("Tree/Focus", transform.position, transform);
        }
    }
    private void SetTargetCircleActive(bool active) => selectedCircleTransform.gameObject.SetActive(active);
    #endregion
    #region Manage Grow
    //Call from tree controller
    public void SetGrowButtonShouldVisible(bool active)
    {
        RefreshUIActivity();
        growPanel.gameObject.SetActive(active);
    }
    public void SetGrowCostText(float growCost) => growCostText.text = growCost.ToString();
    #endregion
}
