using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceAmount
{
    public ResourceType resourceType;
    public float amount;

    public ResourceAmount(ResourceType _resourceType)
    {
        resourceType = _resourceType;
        amount = 0;
    }
}