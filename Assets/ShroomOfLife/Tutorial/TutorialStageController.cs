using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TutorialStageController : MonoBehaviour
{
    [SerializeField] List<GameObject> parts;
    [SerializeField] Transform staticElements;

    private int currentPartIndex = 0;

    public void SetStage(TutorialStageSO stageSO)
    {
        SetName(stageSO.name);
        GetParts();
    }
    [Button]
    private void GetParts()
    {
        parts = new List<GameObject>();

        int i = 1;

        foreach (Transform child in transform)
        {
            if (child == staticElements)
            {
                child.name = "StaticElements";
                continue;
            }
            child.gameObject.name = "StagePart" + i;
            parts.Add(child.gameObject);
            i++;
        }
    }
    private void SetName(string name) => gameObject.name = name + "Stage";
    public void HideParts()
    {
        string name = gameObject.name;
        
        foreach (GameObject part in parts)
        {
            part.SetActive(false);
        }
    }
    public bool IsReachedLastPart()
    {
        return currentPartIndex >= parts.Count - 1;
    }
    public void FirstPart()
    {
        HideParts();
        currentPartIndex = 0;
        parts[currentPartIndex].SetActive(true);
    }
    public void NextPart()
    {
        parts[currentPartIndex].SetActive(false);
        currentPartIndex++;
        parts[currentPartIndex].SetActive(true);
    }
}
