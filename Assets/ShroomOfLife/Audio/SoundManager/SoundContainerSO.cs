using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundContainerSO", menuName = "ScriptableObjects/Audio/SoundContainer")]
public class SoundContainerSO : ScriptableObject
{
    public List<SoundSO> soundList;
}
