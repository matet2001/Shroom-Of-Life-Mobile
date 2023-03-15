using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnMovementController : MonoBehaviour
{
    public static event Action OnRunOutOfYarnResource;
    
    private Transform yarnTrailPrefab;
    private Transform currentYarnTrail;
    [SerializeField] Transform yarnTrailContainer;
    [SerializeField] YarnCrosshairController yarnCrosshairController;

    [SerializeField] float idleSpeed, sprintMultiplier;
    [SerializeField] ResourceAmount idleMovementCost, sprintMovementCost;

    private bool shouldMoveYarn;
    private bool shouldMoveOnArc;
    
    private List<Vector2> mushroomBottonPositions;
    private Vector2 startPosition;

    private Vector3 currentArcVector;
    [Tooltip("It will be divided by 10")]
    [SerializeField] float arcMoveSpeed;

    private void Awake()
    {
        yarnTrailPrefab = Resources.Load<Transform>("pfYarnTrail");
        startPosition = transform.position;
    }
    private void Start()
    {
        SubscribeToEvents();
    }
    #region Event subscriptions
    private void SubscribeToEvents()
    {
        ConquerState.OnConquerStateEnter += StartYarn;
        ConquerState.OnConquerStateExit += ConquerState_OnConquerStateExit;

        ManagerState.OnManagerStateEnter += ManagerState_OnManagerStateEnter;
        ManagerState.OnManagerStateExit += SetClosestPoint;

        YarnCrosshairController.OnYarnCrosshairEnter += YarnCrosshairController_OnYarnCrosshairEnter;
        YarnCrosshairController.OnYarnCrosshairExit += delegate (Vector2 position) { shouldMoveOnArc = false; };

        ConnectionManager.OnMushroomListChange += ConnectionManager_OnMushroomListChange;
    }
    private void StartYarn()
    {
        shouldMoveYarn = true;
        currentYarnTrail = Instantiate(yarnTrailPrefab, transform.position, Quaternion.identity, transform);
    }
    private void ConquerState_OnConquerStateExit()
    {
        StopMoving();
    }
    private void ManagerState_OnManagerStateEnter()
    {
        
    }
    private void ConnectionManager_OnMushroomListChange(List<MushroomController> mushroomList)
    {
        mushroomBottonPositions = new List<Vector2>();

        foreach (MushroomController mushroomController in mushroomList)
        {
            Vector2 mushroomBottomPosition = mushroomController.GetYarnStartPosition();
            mushroomBottonPositions.Add(mushroomBottomPosition);
        }
    }
    #endregion
    private void SetClosestPoint(Transform containerTransform)
    {
        startPosition = GetClosestMushroomBottomPoint(containerTransform);
        transform.position = startPosition;
    }
    private Vector2 GetClosestMushroomBottomPoint(Transform containerTransform)
    {
        Vector2 closestMushroomBottomPoint = Vector2.zero;
        float closestAngle = Mathf.Infinity;
        bool isAnyMushroomFound = false;

        foreach (Vector2 mushroomBottomPoint in mushroomBottonPositions)
        {
            float angle = Vector2.Angle(containerTransform.up, mushroomBottomPoint);

            if (angle < closestAngle)
            {
                closestAngle = angle;
                closestMushroomBottomPoint = mushroomBottomPoint;
                isAnyMushroomFound = true;
            }
        }

        if (isAnyMushroomFound)
        {
            return closestMushroomBottomPoint;
        } 
        return startPosition;
    }
    private void StopMoving()
    {
        shouldMoveYarn = false;
        DetacheYarn();
        transform.position = startPosition;
    }
    private void DetacheYarn()
    {
        if (!currentYarnTrail) return;

        currentYarnTrail.GetComponent<TrailRenderer>().emitting = false;
        currentYarnTrail.parent = yarnTrailContainer;
    }
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        if (!shouldMoveYarn) return;
        

        if (!shouldMoveOnArc) SimpleYarcMovement();
        else MoveYarnAtArc();

        float movementCost = DecideMovementCost() * Time.deltaTime;
        bool isResourceSpent = ResourceManager.resourceData.TryToSpendResource(idleMovementCost.resourceType, movementCost);
        if (!isResourceSpent)
        {
            StopMoving();
            OnRunOutOfYarnResource?.Invoke();
        }
    }
    private void SimpleYarcMovement()
    {
        Vector2 mousePosition = yarnCrosshairController.transform.position;
        Vector3 direction = (mousePosition - (Vector2)transform.position).normalized;

        MoveTransformToDirection(direction, DecideSpeed());
    }
    private void MoveTransformToDirection(Vector3 direction, float moveSpeed)
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    private float DecideSpeed()
    {
        bool isSprintButtonDown = InputManager.IsMouseLeftClick();
        float moveSpeed = (!isSprintButtonDown) ? idleSpeed : idleSpeed * sprintMultiplier;
        return moveSpeed;
    }
    private float DecideMovementCost()
    {
        bool isSprintButtonDown = InputManager.IsMouseLeftClick();
        float movementCost = (!isSprintButtonDown) ? idleMovementCost.amount : idleMovementCost.amount * sprintMultiplier;
        return movementCost;
    }
    private void YarnCrosshairController_OnYarnCrosshairEnter(Vector2 position)
    {
        Vector2 directionVector = (transform.position - yarnCrosshairController.transform.position).normalized;
        currentArcVector = directionVector * (yarnCrosshairController.arcDistace/3f * 2f);
        shouldMoveOnArc = true;
    }
    private void MoveYarnAtArc()
    {
        currentArcVector = Quaternion.Euler(0f, 0f, arcMoveSpeed/10f) * currentArcVector;
        
        Vector3 goalPosition = yarnCrosshairController.transform.position + currentArcVector;
        Vector3 direction = goalPosition - transform.position;
        
        MoveTransformToDirection(direction, arcMoveSpeed/3f);
    }
    private void OnDestroy()
    {
        ConquerState.OnConquerStateEnter -= StartYarn;
        ConquerState.OnConquerStateExit -= ConquerState_OnConquerStateExit;
        ManagerState.OnManagerStateEnter -= ManagerState_OnManagerStateEnter;
    }
}
