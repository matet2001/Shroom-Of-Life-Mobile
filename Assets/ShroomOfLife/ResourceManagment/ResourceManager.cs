using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private ResourceData resourceData;
    [SerializeField] ResourceDataType resourceDataType;

    private void Awake()
    {
        resourceData = new ResourceData(resourceDataType);
    }
}
