using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonController : MonoBehaviour
{
    public void StartGame()
    {
        LevelSceneManager.NextScene();
    }
    public void RestartGame()
    {
        LevelSceneManager.RestartScene();
    }
}
