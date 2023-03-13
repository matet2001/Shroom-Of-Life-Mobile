using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static void RestartScene()
    {
        int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
