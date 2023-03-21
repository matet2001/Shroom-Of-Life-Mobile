using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeTypeSO", menuName = "ScriptableObjects/Tree/TreeType")]
public class TreeType : ResourceDataType
{
    public List<ResourceUnit> resourceProduce;
    public List<ResourceUnit> resourceUse;

    public ResourceUnit[] upgradeResourceCost;
    public TreeSprite[] treeSprites = new TreeSprite[3];

    [ContextMenu("Rebuild")]
    public void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceUse = new List<ResourceUnit>();
        resourceProduce = new List<ResourceUnit>();
        resourceMax = new List<ResourceUnit>();  

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceUnit currentResourceAmount = new ResourceUnit(resourceType);

            resourceUse.Add(currentResourceAmount);
            resourceProduce.Add(currentResourceAmount);
            resourceMax.Add(currentResourceAmount);
        }
    }
}
[Serializable]
public class TreeSprite
{
    public Sprite treeSprite;
    public Sprite rootSprite;
}