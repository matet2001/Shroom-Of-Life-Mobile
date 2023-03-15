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
        ResourceManager.OnResourceManagerInit += ResourceManager_OnResourceManagerInit;
        ResourceManager.OnResourceAmountChange += ResourceManager_OnResourceAmountChange;
        ResourceManager.OnResourceAmountMaxChange += ResourceManager_OnResourceAmountMaxChange;
    }
    private void ResourceManager_OnResourceManagerInit(ResourceManagerData resourceData)
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
    private void ResourceManager_OnResourceAmountChange(Dictionary<ResourceType, float> resourceAmount)
    {
        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetSliderValue(resourceAmount[sliderResourceType]);
        }
    }
    private void ResourceManager_OnResourceAmountMaxChange(Dictionary<ResourceType, float> resourceAmountMax)
    {
        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetSliderValue(resourceAmountMax[sliderResourceType]);
        }
    }
}
[Serializable]
public class SliderData
{
    public ResourceType resourceType;
    public ResourceSliderController sliderController;
}
