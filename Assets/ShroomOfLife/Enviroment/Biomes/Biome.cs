using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeType", menuName = "ScriptableObjects/Enviroment/Biome")]
public class Biome : ScriptableObject
{
    public float fromAngle;
    public float toAngle;
    public Color color;
}
