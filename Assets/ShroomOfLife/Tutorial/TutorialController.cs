using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{
    public UnityEvent OnGameStart;

    private void Awake()
    {
       
    }
    private void Start()
    {
        OnGameStart?.Invoke();
    }

}
