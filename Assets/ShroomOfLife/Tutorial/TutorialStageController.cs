using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStageController : MonoBehaviour
{
    [SerializeField] List<GameObject> parts;

    private int currentPartIndex = 0;

    private void OnValidate()
    {
        GetParts();
    }
    private void GetParts()
    {
        parts = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.gameObject.name = "StagePart" + (i + 1);
            parts.Add(child);
        }
    }
    private void HideParts()
    {
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
