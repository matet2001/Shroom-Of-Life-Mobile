using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCollisionManager : MonoBehaviour, ICollidable
{
    public static event Action OnStoneCollision;
    
    public void Collision()
    {
        OnStoneCollision?.Invoke();
    }
}
