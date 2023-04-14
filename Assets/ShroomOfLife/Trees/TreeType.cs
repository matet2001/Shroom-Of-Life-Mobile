using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeTypeSO", menuName = "ScriptableObjects/Tree/TreeType")]
public class TreeType : ResourceDataType
{
    public List<ResourceUnitList> resourceProduce;
    public List<ResourceUnitList> resourceUse;
    public List<ResourceUnitList> resourceMax;

    public ResourceUnit[] upgradeResourceCost;
    
    public TreeSprite[] treeSprites = new TreeSprite[3];

    [ContextMenu("Rebuild")]
    public void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceUse = new List<ResourceUnitList>();
        resourceProduce = new List<ResourceUnitList>();
        resourceMax = new List<ResourceUnitList>();

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            resourceUse.Add(new ResourceUnitList(resourceTypeContainer.resourceTypes));
            resourceProduce.Add(new ResourceUnitList(resourceTypeContainer.resourceTypes));
            resourceMax.Add(new ResourceUnitList(resourceTypeContainer.resourceTypes));
        }
    }
}
[Serializable]
public class TreeSprite
{
    public Sprite treeSprite;
    public Sprite rootSprite;
}
[Serializable]
public class ResourceUnitList
{
    public List<ResourceUnit> resourceUnits;

    public ResourceUnitList(ResourceType[] resourceTypes)
    {
        resourceUnits = new List<ResourceUnit>(resourceTypes.Length);
        
        foreach (ResourceType resourceType in resourceTypes)
        {
            resourceUnits.Add(new ResourceUnit(resourceType));
        }
    }
}