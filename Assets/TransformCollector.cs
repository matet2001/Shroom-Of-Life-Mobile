using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TransformCollector : MonoBehaviour
{
    [SerializeField] CollidableObstacle collidableToCollect;
    [Button]
    public void Collect()
    {
        GameObject[] collidables = GameObject.FindGameObjectsWithTag(collidableToCollect.tag);      
        
        foreach (GameObject collidable in collidables)
        {
            collidable.transform.SetParent(transform);
        }
    }
}
