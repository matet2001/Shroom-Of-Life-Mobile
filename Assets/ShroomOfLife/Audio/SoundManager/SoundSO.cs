using UnityEngine;

[CreateAssetMenu(fileName = "SoundSO", menuName = "ScriptableObjects/Audio/Sound")]
public class SoundSO : ScriptableObject
{
    public string soundName;
    public AudioClip clip;

    [Range(0, 1f)]
    public float volume = 0.3f;
    [Range(1f, 1.3f)]
    public float pitch = 1;

    [HideInInspector]
    public AudioSource source;
}
