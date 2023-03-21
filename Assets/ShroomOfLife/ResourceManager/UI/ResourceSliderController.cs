using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSliderController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image sliderFillImage;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI valueText;

    private float sliderValue;
    private float sliderValueMax;

    public void SetSliderMax(float value)
    {
        slider.maxValue = value;
        sliderValueMax = value;
        SetValueText(sliderValue.ToString("0"), sliderValueMax.ToString("0"));
    }
    public void SetSliderValue(float value)
    {
        slider.value = value;
        sliderValue = value;
        SetValueText(sliderValue.ToString("0"), sliderValueMax.ToString("0"));
    }
    public void SetSliderColor(Color color) => sliderFillImage.color = color;
    public void SetIconImage(Sprite sprite) => iconImage.sprite = sprite;
    public void SetValueText(string value, string max) => valueText.text = value + "/" + max;
    public void SetUpSlider(float value, float maxValue)
    {      
        SetSliderMax(maxValue);
        SetSliderValue(value);
    }
}
