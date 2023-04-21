using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static event Action OnStageReveale;
    public static event Action OnStageHide;

    [SerializeField] bool hasSeenTutorial = false;
    private bool isStageActive;
    
    [SerializeField] StageData[] stageDatas;
    private StageData currentStageData;
    private TutorialStageTriggerer currentTriggerer;

    [SerializeField] Transform stageContainer;
    private GameObject stageControllerPrefab;

    private void Awake()
    {
        SetUpStageObjectsPlayMode();
        HideAllChild();
        PlatformDebug();
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
    [Button]
    private void SetUpStageObjectsEditor()
    {
        for (int i = 0; i < stageDatas.Length; i++)
        {
            TutorialStageSO stageSO = stageDatas[i].stageSO;

            TutorialStageController stageController = stageDatas[i].stageController;
            if(!stageController)
            {
                //if (!stageControllerPrefab) stageControllerPrefab = Resources.Load<GameObject>("Stage");
                ////stageDatas[i].stageController = 
                //Instantiate(stageControllerPrefab, stageContainer);
                //stageController = stageDatas[i].stageController;
                continue;
            }
            stageController.SetStage(stageSO);

            TutorialStageTriggerer stageTriggerer = stageDatas[i].stageTriggerer;
            if (!stageTriggerer) continue;
            stageTriggerer.SetTriggerer(stageSO);
       
        }
    }
    private void SetUpStageObjectsPlayMode()
    {
        for (int i = 0; i < stageDatas.Length; i++)
        {
            TutorialStageSO stageSO = stageDatas[i].stageSO;
            TutorialStageController stageController = stageDatas[i].stageController;
            TutorialStageTriggerer stageTriggerer = stageDatas[i].stageTriggerer;

            stageController.SetStage(stageSO);
            stageController.HideParts();
            stageController.gameObject.SetActive(false);
            if (!stageTriggerer) continue;
            stageTriggerer.SetTriggerer(stageSO);
            stageTriggerer.OnTrigger += delegate { TryToShowStage(stageSO); };
            stageTriggerer.OnActivate += StageTriggerer_OnActivate;
            stageTriggerer.SetTriggererActive(false);
        }
    }
    private void StageTriggerer_OnActivate(TutorialStageTriggerer triggerer)
    {
        TurnOffCurrentStageTriggerer();
        currentTriggerer = triggerer;
    }
    public void TryToShowStage(TutorialStageSO stageSO)
    {
        if (hasSeenTutorial) return;
        if (isStageActive) return;

        currentStageData = StageData.GetCurrentStageData(stageDatas ,stageSO);

        ShowStage();
        currentStageData.stageController.FirstPart();
    }
    private void ShowStage()
    {
        SetStageActive(true);
        ShowChilds();
        OnStageReveale?.Invoke();
    } 
    private void HideStage()
    {
        SetStageActive(false);
        HideAllChild();
        OnStageHide?.Invoke();
    }
    private void SetStageActive(bool active)
    {
        currentStageData.stageController.gameObject.SetActive(active);
        isStageActive = active;
    }
    private void StageOver()
    {
        HideStage();

        if (!currentStageData.shouldActivateNextStageTriggerer) return;
        
        TurnOnNextStageTriggerer();
    }
    private void TurnOnNextStageTriggerer()
    {
        StageData nextStageData = StageData.GetNextStageData(stageDatas, currentStageData);
        TurnOnStageTriggerer(nextStageData);
    }
    private void TurnOffCurrentStageTriggerer()
    {
        if (!currentTriggerer) return;
        StageData triggererStageData = StageData.GetStageData(stageDatas, currentTriggerer);
        TurnOffStageTriggerer(triggererStageData);
    }
    private void TurnOnStageTriggerer(StageData stageData)
    {     
        SetStageTriggererActive(stageData, true);
        currentTriggerer = stageData.stageTriggerer;
    }
    private void TurnOffStageTriggerer(StageData stageData)
    {
        SetStageTriggererActive(stageData, false);
    }  
    private void SetStageTriggererActive(StageData stageData, bool active)
    {
        TutorialStageTriggerer stageTriggerer = stageData.stageTriggerer;
        if (!stageTriggerer) return;
        stageTriggerer.SetTriggererActive(active);
    }
    #endregion
    #region PartControll
    private void HandleMousePress()
    {
        if (!isStageActive) return;
        if (RaycastUtilities.PointerIsOverUI("Skip")) return;
        if (!InputManager.IsMouseLeftClickPressed()) return;
        
        TryNextPart();
    }
    private void TryNextPart()
    {
        if (!currentStageData.stageController) return;
        if (currentStageData.stageController.IsReachedLastPart())
        {
            StageOver();
            return;
        }

        currentStageData.stageController.NextPart();
    }
    #endregion
    public void SkipTutorial()
    {
        hasSeenTutorial = true;
        StageOver();
    }
    private void PlatformDebug()
    {
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
        hasSeenTutorial = false;
        Debug.Log("WEBGL/MOBILE");
#endif
#if UNITY_EDITOR
        //hasSeenTutorial = true;
        //UnityEngine.Debug.Log("EDITOR");
#endif
    }
}
[Serializable]
public class StageData
{
    public TutorialStageSO stageSO;
    public TutorialStageController stageController;
    public TutorialStageTriggerer stageTriggerer;
    public bool shouldActivateNextStageTriggerer;
   

    public static StageData GetCurrentStageData(StageData[] stageDatas, TutorialStageSO stageSO)
    {
        foreach (StageData stageData in stageDatas)
        {
            if (stageData.stageSO == stageSO)
            {
                return stageData;
            }
        }

        return null;
    }
    public static StageData GetStageData(StageData[] stageDatas, TutorialStageTriggerer triggerer)
    {
        foreach (StageData stageData in stageDatas)
        {
            if (stageData.stageTriggerer == triggerer)
            {
                return stageData;
            }
        }

        return null;
    }
    public static StageData GetNextStageData(StageData[] stageDatas, StageData currentStageData)
    {
        for (int i = 0; i < stageDatas.Length;i++)
        {
            if(currentStageData == stageDatas[i])
            {
                if(i == stageDatas.Length - 1)
                {
                    Debug.Log("There is no stage after " + currentStageData);
                    return null;
                }
                return stageDatas[i + 1];
            }
        }

        return null;
    }
    public static StageData GetPreviousStageData(StageData[] stageDatas, StageData currentStageData)
    {
        for (int i = 0; i < stageDatas.Length; i++)
        {
            if (currentStageData == stageDatas[i])
            {
                if (i == 0)
                {
                    Debug.Log("There is no stage before " + currentStageData);
                    return null;
                }
                return stageDatas[i - 1];
            }
        }

        return null;
    }
    [Button]
    public void AddStageData()
    {

    }
}