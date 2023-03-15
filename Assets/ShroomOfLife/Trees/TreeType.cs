using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeTypeSO", menuName = "ScriptableObjects/Tree/TreeType")]
public class TreeType : ResourceDataType
{
    public List<ResourceAmount> resourceProduce;

    public ResourceType ugradeResourceType;
    public float[] upgradeResourceAmount;

    public TreeSprite[] treeSprites = new TreeSprite[3];

    public override void ReBuild()
    {
        ResourceTypeContainer resourceTypeContainer = Resources.Load<ResourceTypeContainer>("ResourceTypeContainer");

        resourceProduce = new List<ResourceAmount>();
        resourceMax = new List<ResourceAmount>();

        upgradeResourceAmount = new float[3];

        treeSprites = new TreeSprite[3];    

        foreach (ResourceType resourceType in resourceTypeContainer.resourceTypes)
        {
            ResourceAmount currentResourceAmount = new ResourceAmount(resourceType);

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