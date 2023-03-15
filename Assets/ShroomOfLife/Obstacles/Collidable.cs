using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collidable : MonoBehaviour
{
    public static event Action OnCollidableCollision;

    public virtual void Collision()
    {
        OnCollidableCollision?.Invoke();
    }
}
