using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIManager : MonoBehaviour
{
    [SerializeField] SliderData[] resourceSliders;

    private void Awake()
    {
        WinState.OnWinGame += HideAllUI;
        LoseState.OnLoseGame += HideAllUI;
        LevelSceneManager.OnRestart += ShowAllUI;

        ResourceManager.OnResourceManagerInit += SetUpSliders;
        ResourceManager.OnResourceAmountChange += RefreshSliderValue;
        ResourceManager.OnResourceAmountMaxChange += RefreshSliderMax;
    }
    private void SetUpSliders(ResourceManagerData resourceData)
    {
        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetIconImage(sliderResourceType.iconImage);
            //sliderController.SetSliderColor(sliderResourceType.sliderColor);
            sliderController.SetUpSlider(resourceData.resourceAmount[sliderResourceType], resourceData.resourceMax[sliderResourceType]);
        }
    }
    private void RefreshSliderValue(Dictionary<ResourceType, float> resourceAmount)
    {
        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetSliderValue(resourceAmount[sliderResourceType]);
        }
    }
    private void RefreshSliderMax(Dictionary<ResourceType, float> resourceAmountMax)
    {
        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetSliderMax(resourceAmountMax[sliderResourceType]);
        }
    }
    private void HideAllUI() => gameObject.SetActive(false);
    private void ShowAllUI() => gameObject.SetActive(true);
}
[Serializable]
public class SliderData
{
    public ResourceType resourceType;
    public ResourceSliderController sliderController;
}
