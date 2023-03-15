using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "TreeTypeSO", menuName = "ScriptableObjects/Resources/DatatType")]
public class ResourceDataType : ScriptableObject
{
    public List<ResourceAmount> resourceAmount;
    public List<ResourceAmount> resourceUse;
    public List<ResourceAmount> resourceProcude;
    public List<ResourceAmount> resourceMax;

    [ExecuteInEditMode]
    private void Awake()
    {
        ReBuild();
    }
    [ContextMenu("Rebuild")]
    public virtual void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceAmount = new List<ResourceAmount>();
        resourceUse = new List<ResourceAmount>();
        resourceProcude = new List<ResourceAmount>();
        resourceMax = new List<ResourceAmount>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceAmount currentResourceAmount = new ResourceAmount(resourceType);

            resourceAmount.Add(currentResourceAmount);
            resourceUse.Add(currentResourceAmount);
            resourceProcude.Add(currentResourceAmount);
            resourceMax.Add(currentResourceAmount);
        }
    }
}

