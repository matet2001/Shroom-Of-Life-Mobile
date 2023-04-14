using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public List<SoundContainerSO> soundLists;
    private List<GameObject> soundPlayerList;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        soundPlayerList = new List<GameObject>();
    }
    public GameObject PlaySound(string nameString, Vector3 position)
    {
        GameObject gameObject = GetSoundPlayer();

        gameObject.transform.position = position;
        gameObject.transform.SetParent(transform);

        SoundSO sound = GetClip(nameString);
        AudioClip clip = sound.clip;
        
        gameObject.GetComponent<AudioSource>().PlayOneShot(clip, sound.volume);
        
        return gameObject;
    }
    public GameObject PlaySoundLooped(string nameString, Vector3 position, Transform parentTransform)
    {
        GameObject gameObject = GetSoundPlayer();

        gameObject.transform.position = position;
        gameObject.transform.SetParent(parentTransform);

        SoundSO sound = GetClip(nameString);
        AudioClip clip = sound.clip;

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        audioSource.loop = true;
        audioSource.Play();

        return gameObject;
    }
    public void StopSound(GameObject gameObject)
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }
    private SoundSO GetClip(string nameString)
    {
        string[] nameElements = nameString.Split("/");

        foreach (SoundContainerSO soundContainer in soundLists)
        {
            if (nameElements[0] == soundContainer.name)
            {
                foreach (SoundSO sound in soundContainer.soundList)
                {
                    if (nameElements[1] == sound.name)
                    {
                        return sound;
                    } 
                }
            }
        }

        Debug.Log("Cannot find clip with name: " + nameString);
        return null;
    }
    private GameObject GetSoundPlayer()
    {
        foreach(GameObject gameObject in soundPlayerList)
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying) continue;
            else return gameObject;
        }

        GameObject newGameObject = new GameObject("Sound", typeof(AudioSource));
        soundPlayerList.Add(newGameObject);
        return newGameObject;
    }
}
