using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MushroomController : MonoBehaviour
{
    public MushroomResourceData resourceData { get; private set; }
    [SerializeField] MushroomResourceDataType resourceDataType;

    [SerializeField] Transform yarnStartPoint;

    [SerializeField] Button startButton;
    public static event Action<Vector3> OnTryToStartYarn;

    public static MushroomController CreateMushroom(Vector2 createPosition)
    {
        GameObject mushroomPrefab = Resources.Load<GameObject>("MushroomBase");
        GameObject mushroom = Instantiate(mushroomPrefab, createPosition, Quaternion.identity);
        return mushroom.GetComponent<MushroomController>();
    }
    private void Awake()
    {
        resourceData = new MushroomResourceData(resourceDataType);
    }
    private void Start()
    {
        ConquerState.OnConquerStateEnter += HideStartButton;
    }
    public void TryToStartYarn()
    {
        OnTryToStartYarn?.Invoke(yarnStartPoint.position);
    }
    private void HideStartButton() => SetStartButtonActive(false);
    public void SetStartButtonActive(bool active) => startButton.gameObject.SetActive(active);
}
