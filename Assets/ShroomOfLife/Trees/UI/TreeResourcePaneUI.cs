using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class TreeResourcePaneUI : MonoBehaviour
{
    [SerializeField] SliderData[] resourceSliders;
    [SerializeField] TextMeshProUGUI panelText;

    public bool shouldDisplayGrowData { private get; set; }

    private static string normalPanelText = "Current Resource Data";
    private static string nextLevelPanelText = "Next Level Resource Data";

    public void RefreshSliderValues(TreeResourceData resourceData)
    {
        panelText.text = normalPanelText;

        Dictionary<ResourceType, float> resourceAmount = new Dictionary<ResourceType, float>
        {
            { resourceSliders[0].resourceType, resourceData.resourceUse[resourceSliders[0].resourceType] },
            { resourceSliders[1].resourceType, resourceData.resourceProduce[resourceSliders[1].resourceType] },
            { resourceSliders[2].resourceType, resourceData.resourceProduce[resourceSliders[2].resourceType] }
        };

        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetIconImage(sliderResourceType.iconImage);
            //sliderController.SetSliderColor(sliderResourceType.sliderColor);

            
            sliderController.SetUpSlider(resourceAmount[sliderResourceType], resourceData.resourceMax[sliderResourceType]);
        }
    }
    public void RefreshSliderValues(TreeType treeType, int growLevel)
    {
        panelText.text = nextLevelPanelText;

        int nextGrowLevel = (growLevel + 1) * 2;

        Dictionary<ResourceType, float> resourceAmount = new Dictionary<ResourceType, float>
        {
            { resourceSliders[0].resourceType, treeType.resourceUse.Find(x => x.type == resourceSliders[0].resourceType).amount * nextGrowLevel},
            { resourceSliders[1].resourceType, treeType.resourceProduce.Find(x => x.type == resourceSliders[1].resourceType).amount * nextGrowLevel},
            { resourceSliders[2].resourceType, treeType.resourceProduce.Find(x => x.type == resourceSliders[2].resourceType).amount * nextGrowLevel}
        };

        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetIconImage(sliderResourceType.iconImage);
            //sliderController.SetSliderColor(sliderResourceType.sliderColor);

            float sliderValue = resourceAmount[sliderResourceType];
            float sliderMaxValue = treeType.resourceMax.Find(x => x.type == sliderData.resourceType).amount;
            sliderController.SetUpSlider(sliderValue, sliderMaxValue * nextGrowLevel);
        }
    }
}
