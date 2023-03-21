using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceUnit
{
    public ResourceType type;
    public float amount;

    public ResourceUnit(ResourceType resourceType)
    {
        type = resourceType;
        amount = 0;
    }
    public ResourceUnit(ResourceType resourceType, float resourceAmount)
    {
        type = resourceType;
        amount = resourceAmount;
    }
}
