using UnityEngine;

[CreateAssetMenu(fileName = "MusicSO", menuName = "ScriptableObjects/Audio/Music")]
public class MusicSO : ScriptableObject
{
    public AudioClip clip;

    [Range(0, 1f)]
    public float volume = 0.3f;
    [Range(1f, 1.3f)]
    public float pitch = 1;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
