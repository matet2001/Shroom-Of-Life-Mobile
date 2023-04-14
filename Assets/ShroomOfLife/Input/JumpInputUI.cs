using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpInputUI : MonoBehaviour
{
    [SerializeField] ManagerCameraDistanceChecker distanceChecker;
    [SerializeField] ManagerCameraMovementController movementController;

    [SerializeField] Button rightArrow, leftArrow;

    private void Start()
    {
        rightArrow.onClick.AddListener(() => JumpToDirection(1));
        leftArrow.onClick.AddListener(() => JumpToDirection(-1));
    }
    private void Update()
    {
        rightArrow.gameObject.SetActive(IsThereObjectToDirection(1));
        leftArrow.gameObject.SetActive(IsThereObjectToDirection(-1));
    }
    private bool IsThereObjectToDirection(float direction)
    {
        Transform closestObjectTransform = distanceChecker.GetClosestObjectToDirection(direction);
        return closestObjectTransform != null;
    }
    private void JumpToDirection(float direction)
    {
        Transform closestObjectTransform = distanceChecker.GetClosestObjectToDirection(direction);
        if (closestObjectTransform == null) return;

        movementController.SetCameraPosition(closestObjectTransform.position);
    }
}