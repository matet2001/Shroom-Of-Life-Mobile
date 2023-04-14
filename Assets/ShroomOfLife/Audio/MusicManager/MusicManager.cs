using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;

    [SerializeField] List<SoundSO> musicList;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        audioSource = GetComponent<AudioSource>();

        PlayMusic("Background");
    }
    public void PlayMusic(string name)
    {
        audioSource.clip = GetClip(name);
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StopMusic()
    {
        audioSource.Stop();
    }

    private AudioClip GetClip(string nameString)
    {
        foreach (SoundSO sound in musicList)
        {
            if (nameString == sound.soundName) return sound.clip;
        }

        Debug.Log("Cannot find clip with name: " + nameString);
        return null;
    }
}
