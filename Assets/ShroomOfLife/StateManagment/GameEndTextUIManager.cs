using StateManagment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndTextUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] TextMeshProUGUI loseText;

    private void Start()
    {
        WinState.OnWinGame += RevealWinText;
        LoseState.OnLoseGame += RevealLoseText;

        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
    }

    private void RevealWinText() => winText.gameObject.SetActive(true);
    private void RevealLoseText() => loseText.gameObject.SetActive(true);
}
