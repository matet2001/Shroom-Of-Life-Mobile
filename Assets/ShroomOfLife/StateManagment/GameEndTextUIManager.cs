using StateManagment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndTextUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] TextMeshProUGUI loseText;
    [SerializeField] GameObject otherUI;

    private void Start()
    {
        WinState.OnWinGame += RevealWinText;
        LoseState.OnLoseGame += RevealLoseText;

        LevelSceneManager.OnRestart += HideEverything;

        otherUI.SetActive(false);
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
    }
    private void RevealWinText()
    {
        otherUI.SetActive(true);
        winText.gameObject.SetActive(true);
    }
    private void RevealLoseText()
    {
        otherUI.SetActive(true);
        loseText.gameObject.SetActive(true);
    }
    private void HideEverything()
    {
        otherUI.SetActive(false);
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);

    }
}
