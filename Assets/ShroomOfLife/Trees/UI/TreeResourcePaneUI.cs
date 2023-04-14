using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreeResourcePaneUI : MonoBehaviour
{
    [SerializeField] SliderData[] resourceSliders;
    [SerializeField] TextMeshProUGUI panelText;

    public bool shouldDisplayGrowData { private get; set; }

    private static string normalPanelText = "Current Resource Data";
    private static string nextLevelPanelText = "Next Level Resource Data";

    //Show current level values
    public void RefreshSliderValues(TreeController tree)
    {
        panelText.text = normalPanelText;

        Dictionary<ResourceType, float> resourceAmount = new Dictionary<ResourceType, float>
        {
            { resourceSliders[0].resourceType, tree.GetCurrentResourceUse(resourceSliders[0].resourceType).amount },
            { resourceSliders[1].resourceType, tree.GetCurrentResourceProduce(resourceSliders[1].resourceType).amount },
            { resourceSliders[2].resourceType, tree.GetCurrentResourceProduce(resourceSliders[2].resourceType).amount }
        };

        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetIconImage(sliderResourceType.iconImage);
            //sliderController.SetSliderColor(sliderResourceType.sliderColor);

            sliderController.SetUpSlider(resourceAmount[sliderResourceType], tree.GetCurrentResourceMax(sliderResourceType).amount);
        }
    }
    //Show next level values
    public void RefreshSliderValues(TreeController tree, int growLevel)
    {
        panelText.text = nextLevelPanelText;

        int nextGrowLevel = growLevel + 1;

        Dictionary<ResourceType, float> resourceAmount = new Dictionary<ResourceType, float>
        {
            { resourceSliders[0].resourceType, tree.GetResourceUseAtLevel(resourceSliders[0].resourceType, nextGrowLevel)},
            { resourceSliders[1].resourceType, tree.GetResourceProduceAtLevel(resourceSliders[1].resourceType, nextGrowLevel)},
            { resourceSliders[2].resourceType, tree.GetResourceProduceAtLevel(resourceSliders[2].resourceType, nextGrowLevel)}
        };

        foreach (SliderData sliderData in resourceSliders)
        {
            ResourceType sliderResourceType = sliderData.resourceType;
            ResourceSliderController sliderController = sliderData.sliderController;

            sliderController.SetIconImage(sliderResourceType.iconImage);
            //sliderController.SetSliderColor(sliderResourceType.sliderColor);

            float sliderValue = resourceAmount[sliderResourceType];
            float sliderMaxValue = tree.GetResourceMaxAtLevel(sliderResourceType, nextGrowLevel);
            sliderController.SetUpSlider(sliderValue, sliderMaxValue * nextGrowLevel);
        }
    }
}
