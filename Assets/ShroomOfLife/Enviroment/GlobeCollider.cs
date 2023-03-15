using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeCollider : Collidable
{
    public static event Action<Vector2> OnYarnExitGlobe;
    
    public void CollisionExit(Vector2 exitPoint)
    {
        base.Collision();
        OnYarnExitGlobe?.Invoke(exitPoint);
    }
    public override void Collision()
    {
        
    }
}
