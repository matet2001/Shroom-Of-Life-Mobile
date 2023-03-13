using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeTypeSO", menuName = "ScriptableObjects/Tree/TreeType")]
public class TreeType : ResourceDataType
{
    public List<ResourceAmount> resourceTrade;
    public TreeSprite[] treeSprites  = new TreeSprite[3];

    public override void ReBuild()
    {
        base.ReBuild();   
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");
        resourceTrade = new List<ResourceAmount>();
        treeSprites = new TreeSprite[3];

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceAmount currentResourceAmount = new ResourceAmount(resourceType);
            resourceTrade.Add(currentResourceAmount);
        }


    }
}
[Serializable]
public class TreeSprite
{
    public Sprite treeSprite;
    public Sprite rootSprite;
}