using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnCrosshairController : MonoBehaviour
{
    [SerializeField] YarnMovementController yarnMovementController;
    
    private void Update()
    {
        SetPosition();
    }
    private void SetPosition()
    {
        transform.position = InputManager.GetMouseWorldPosition();
    }
}
