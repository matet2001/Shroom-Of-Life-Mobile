using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeCollisionController : MonoBehaviour
{
    public static event Action OnYarnExitGlobe;
    public void CollisionExit()
    {
        OnYarnExitGlobe?.Invoke();
    }
}
