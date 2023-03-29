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

    [SerializeField] GameObject background, stageContainer;
    [SerializeField] Button skipButton;
    
    [SerializeField] GameObject[] stages;
    private int stageIndex = -1;
    [SerializeField] List<GameObject> partList;
    private int partIndex = 0;

    private void Awake()
    {
        background.SetActive(false);
        skipButton.onClick.AddListener(SkipTutorial);
        skipButton.gameObject.SetActive(false);
        Array.ForEach(stages, stage => { stage.SetActive(false); });
        stageContainer.SetActive(true);
    }
    private void Update()
    {
        HandleMousePress();
    }
    public void NextStage()
    {
        if (hasSeenTutorial) return;
        //if (IsReachedLastStage())
        //{
        //    hasSeenTutorial = true;
        //    return;
        //}

        stageIndex++;
        RevealCurrentStage();
    }
    private void RevealCurrentStage()
    {
        background.SetActive(true);
        skipButton.gameObject.SetActive(true);
        stages[stageIndex].SetActive(true);
        isStageActive = true;

        SetCurrentStageList();
        
        partIndex = 0;
        partList[partIndex].SetActive(true);
        OnStageReveale?.Invoke();
    }
    private void SetCurrentStageList()
    {
        partList = new List<GameObject>();
        foreach (Transform child in stages[stageIndex].transform)
        {
            partList.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }
    public void HideCurrentStage()
    {
        stages[stageIndex].SetActive(false);
        background.SetActive(false);
        skipButton.gameObject.SetActive(false);
        isStageActive = false;

        OnStageHide?.Invoke(stages[stageIndex]);
    }
    private bool TryToRevealNextPart()
    {
        if (partIndex < partList.Count - 1)
        {
            partList[partIndex].SetActive(false);
            partIndex++;
            partList[partIndex].SetActive(true);
            return true;
        }

        return false;
    }
    private void SkipTutorial()
    {
        hasSeenTutorial = true;
        HideCurrentStage();
    }
    private void HandleMousePress()
    {
        if (!isStageActive) return;
        if (!InputManager.IsMouseLeftClickPressed()) return;

        if (!TryToRevealNextPart()) HideCurrentStage();
    }
    private bool IsReachedLastStage() { return stageIndex < stages.Length; }
    public GameObject GetNextStage() => stages[stageIndex + 1];
}