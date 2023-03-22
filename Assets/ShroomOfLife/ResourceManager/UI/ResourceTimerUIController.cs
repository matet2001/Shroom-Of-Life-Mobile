using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceTimerUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Button skipButton;

    public void SetTimerText(float time)
    {
        timerText.text = time.ToString("F0");
    }
    public void HideUI() => gameObject.SetActive(false);
    public Button GetButton() => skipButton;
}
