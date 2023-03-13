using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]

public class AutoSave
{
    static AutoSave()
    {
        EditorApplication.playModeStateChanged += SaveOnPlay;
    }

    private static void SaveOnPlay(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode)
        {
            //Debug.Log("Auto-saving...");
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }
    }
}
