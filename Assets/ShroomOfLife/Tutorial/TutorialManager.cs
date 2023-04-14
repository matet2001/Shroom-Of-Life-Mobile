using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static event Action OnStageReveale;
    public static event Action<GameObject> OnStageHide;

    [SerializeField] bool hasSeenTutorial = false;
    private bool isStageActive;
    
    [SerializeField] Transform stageContainerTransform;
    [SerializeField] GameObject[] stages;
    private GameObject currentStage;
    private TutorialStageController currentStageController;

    private void OnValidate()
    {
        GetStages();
    }
    private void Awake()
    {
        HideAllChild();
        Debug();
    }
    private void Start()
    {
        StageTriggerer.OnTrigger += TryToShowStage;
    }
    private void Update()
    {
        HandleMousePress();
    }
    private void HideAllChild()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    private void ShowChilds()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    #region StageControll
    private void GetStages()
    {
        if (!stageContainerTransform) return;

        stages = new GameObject[stageContainerTransform.childCount];

        for (int i = 0; i < stageContainerTransform.childCount; i++)
        {
            stages[i] = stageContainerTransform.GetChild(i).gameObject;
        }
    }
    private void TryToShowStage(GameObject stageGameObject)
    {
        if (hasSeenTutorial) return;
        if (isStageActive) return;

        currentStage = stageGameObject;
        ShowStage();
        
        currentStageController = currentStage.GetComponent<TutorialStageController>();
        currentStageController.FirstPart();
    }
    private void ShowStage() => SetStageActive(currentStage, true);
    private void HideStage() => SetStageActive(currentStage, false);
    private void SetStageActive(GameObject stageGameObject, bool active)
    {
        stageGameObject.SetActive(active);

        if (active) ShowChilds();
        else HideAllChild();

        isStageActive = active;
    }
    #endregion
    #region PartControll
    private void HandleMousePress()
    {
        if (!isStageActive) return;
        if (RaycastUtilities.PointerIsOverUI("Skip")) return;
        if (!InputManager.IsMouseLeftClickPressed()) return;
        
        NextPart();
    }
    private void NextPart()
    {
        if (!currentStageController) return;
        if (currentStageController.IsReachedLastPart())
        {
            HideStage();
            return;
        }

        currentStageController.NextPart();
    }
    #endregion
    public void SkipTutorial()
    {
        hasSeenTutorial = true;
        HideStage();
    }
    private void Debug()
    {
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
        hasSeenTutorial = false;
        UnityEngine.Debug.Log("WEBGL/MOBILE");
#endif
#if UNITY_EDITOR
        //hasSeenTutorial = true;
        //UnityEngine.Debug.Log("EDITOR");
#endif
    }
}