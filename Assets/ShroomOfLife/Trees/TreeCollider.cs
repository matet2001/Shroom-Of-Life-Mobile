using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCollider : Collidable
{
    public event Action OnYarnCollided;

    private new PolygonCollider2D collider;
    
    private void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
        
    }
    public override void Collision()
    {
        base.Collision();
        OnYarnCollided?.Invoke();
    }
    public void ResetCollider()
    {
        Destroy(collider);
        collider = gameObject.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
    }
}
