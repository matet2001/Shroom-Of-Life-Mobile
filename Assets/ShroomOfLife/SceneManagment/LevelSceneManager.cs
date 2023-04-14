using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static event Action OnRestart;
    
    public static void RestartScene()
    {
        OnRestart?.Invoke();
    }
    public static void NextScene()
    {
        int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneBuildIndex + 1);
    }
}
