using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialStageTriggerer : MonoBehaviour
{
    public event Action OnTrigger;
    public event Action<TutorialStageTriggerer> OnActivate;
    
    [SerializeField] GameObject textGameObject;

    private TextMeshProUGUI textMeshPro;
    public void Trigger()
    {
        OnTrigger?.Invoke();
        Hide();
    }
    private void Update()
    {
        //if (!textMeshPro) return;
        //Debug.Log(textMeshPro.text);
    }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false); 
    public void SetTriggerer(TutorialStageSO stage)
    {
        if (!stage) return;

        if (textGameObject.TryGetComponent(out TextMeshPro textMesh))
        {
            textMesh.text = stage.title;
            SetStageNames(stage, "Obstacle");
        }
        if (textGameObject.TryGetComponent(out TextMeshProUGUI textMeshUI))
        {
            textMeshUI.text = stage.title;
            SetStageNames(stage, "Button");
            textMeshPro = textMeshUI;
        }
    }
    private void SetStageNames(TutorialStageSO stage, string nameType)
    {
        gameObject.name = stage.name + "Triggerer" + nameType;
    }
    public void SetTriggererActiveEvent(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            OnActivate?.Invoke(this);
        }
    }
    public void SetTriggererActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
