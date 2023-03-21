using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceTimerUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    public void SetTimerText(float time)
    {
        timerText.text = time.ToString("F0");
    }
    public void HideUI() => gameObject.SetActive(false);
}
