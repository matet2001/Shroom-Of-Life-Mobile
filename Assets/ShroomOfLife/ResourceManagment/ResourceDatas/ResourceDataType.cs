using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceDataType : ScriptableObject
{  
    public List<ResourceAmount> resourceMax;

    [ExecuteInEditMode]
    private void Awake()
    {
        ReBuild();
    }
    [ContextMenu("Rebuild")]
    public abstract void ReBuild();
}
