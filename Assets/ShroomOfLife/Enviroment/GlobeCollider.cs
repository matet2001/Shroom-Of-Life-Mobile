using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeCollider : Collidable
{
    public static event Action<Vector3> OnYarnExitGlobe;

    public static Transform globeTransform;

    private void Awake()
    {
        globeTransform = transform;
    }
    public void CollisionExit(Vector3 exitPoint)
    {
        base.Collision();
        OnYarnExitGlobe?.Invoke(exitPoint);
    }
    public override void Collision()
    {
        
    }
}
