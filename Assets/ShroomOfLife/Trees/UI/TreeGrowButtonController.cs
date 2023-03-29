using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreeGrowButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnGrowButtonPressed;
    public event Action OnPointerEnterButton;
    public event Action OnPointerExitButton;

    public Button growButton;

    private void Start()
    {
        growButton.onClick.AddListener(() => OnGrowButtonPressed?.Invoke());
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterButton?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitButton?.Invoke();
    }
}
