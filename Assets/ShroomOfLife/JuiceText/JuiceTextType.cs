using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JuiceTextType", menuName = "ScriptableObjects/UI/JuiceTextType", order = 1)]
public class JuiceTextType : ScriptableObject
{
    public Color textColor;
    public float duration;
    public float heightLimit;
    public float fontSize;

    public enum type { easeIn, easeOut, exponential };
    public type choosen;
}
