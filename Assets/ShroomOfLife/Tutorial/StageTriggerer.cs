using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageTriggerer : MonoBehaviour
{
    public static event Action<GameObject> OnTrigger;
    public TutorialStage stage;
    [SerializeField] GameObject stageGameObject;

    [SerializeField] bool isNameSet;

    public void Trigger()
    {
        OnTrigger?.Invoke(stageGameObject);
        Hide();
    }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    private void OnValidate()
    {
        if (isNameSet) return;
        if (!stage) return;

        GameObject textGameObject = transform.GetChild(0).gameObject;

        if (textGameObject.TryGetComponent(out TextMeshPro textMesh))
        {
            textMesh.text = stage.title;
            SetStageNames("Obstacle");
        }
        if (textGameObject.TryGetComponent(out TextMeshProUGUI textMeshUI))
        {
            textMeshUI.text = stage.title;
            SetStageNames("Button");
        }
    }
    private void SetStageNames(string nameType)
    {
        gameObject.name = stage.title + "Triggerer" + nameType;
        
        isNameSet = true;

        if (stageGameObject)
            stageGameObject.name = stage.title + "Stage";
    }
}
