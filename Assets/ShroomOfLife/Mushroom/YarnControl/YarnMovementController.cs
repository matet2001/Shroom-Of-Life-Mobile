using StateManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnMovementController : MonoBehaviour
{
    #region Built In Functions
    private void Awake()
    {
        yarnTrailPrefab = Resources.Load<Transform>("pfYarnTrail");
    }
    private void Start()
    {
        SubscribeToEvents();
    }
    private void Update()
    {
        ManageMovement();
    }
    private void SubscribeToEvents()
    {
        WinState.OnWinGame += ManageGameEnd;
        LoseState.OnLoseGame += ManageGameEnd;
        
        ConquerState.OnConquerStateExit += StopMoving;

        MushroomController.OnTryToStartYarn += TryToStartYarn;

        YarnCrosshairController.OnYarnCrosshairEnter += StartArcMovement;
        YarnCrosshairController.OnYarnCrosshairExit += delegate (Vector2 position) { shouldMoveOnArc = false; };
    }
    private void OnDestroy()
    {
        WinState.OnWinGame -= ManageGameEnd;
        LoseState.OnLoseGame -= ManageGameEnd;
        ConquerState.OnConquerStateExit -= StopMoving;
    }
    #endregion
    #region Manage Movement
    private Transform yarnTrailPrefab;
    private Transform currentYarnTrail;
    private bool shouldMoveYarn;
    private bool isGameEnded = false;
    [SerializeField] Transform yarnTrailContainer;

    public static event Action OnYarnStart;
    
    public void TryToStartYarn(Vector3 startPosition)
    {
        if (isGameEnded) return;
        if (shouldMoveYarn) return;
        if (!ResourceManager.resourceData.CanSpendResource(idleMovementCost))
        {
            JuiceTextCreator.CreateJuiceText(transform.position, "You don't have enough \n energy to start yarn", "BasicTextSO");
            return;
        } 

        StartYarn(startPosition);
    }
    private void StartYarn(Vector3 startPosition)
    {
        shouldMoveYarn = true;
        currentYarnTrail = Instantiate(yarnTrailPrefab, startPosition, Quaternion.identity, transform);
        OnYarnStart?.Invoke();
    }
    private void StopMoving()
    {
        shouldMoveYarn = false;
        DetacheYarn();
    }
    private void DetacheYarn()
    {
        if (!currentYarnTrail) return;

        currentYarnTrail.GetComponent<TrailRenderer>().emitting = false;
        currentYarnTrail.parent = yarnTrailContainer;
    }
    private void ManageMovement()
    {
        if (!shouldMoveYarn) return;

        if (!shouldMoveOnArc) SimpleMovement();
        else MoveYarnAtArc();

        bool isResourceSpent = SpendMovementCost();
        RunOutResource(isResourceSpent);
    }
    private void ManageGameEnd()
    {
        StopMoving();
        isGameEnded = true;
    }
    #endregion
    #region Movement Types
    [SerializeField] YarnCrosshairController yarnCrosshairController;
    
    #region Simple Movement
    [SerializeField] float idleSpeed, sprintMultiplier;
    
    private void SimpleMovement()
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
    #endregion
    #region Arc Movement
    private bool shouldMoveOnArc;
    private Vector3 currentArcVector;

    [Tooltip("It will be divided by 10"), SerializeField]
    private float arcMoveSpeed;

    private void MoveYarnAtArc()
    {
        currentArcVector = Quaternion.Euler(0f, 0f, arcMoveSpeed / 10f) * currentArcVector;

        Vector3 goalPosition = yarnCrosshairController.transform.position + currentArcVector;
        Vector3 direction = goalPosition - transform.position;

        MoveTransformToDirection(direction, arcMoveSpeed / 3f);
    }
    private void StartArcMovement(Vector2 position, float arcDistance)
    {
        Vector2 directionVector = (transform.position - yarnCrosshairController.transform.position).normalized;
        currentArcVector = directionVector * (arcDistance / 3f * 2f);
        shouldMoveOnArc = true;
    }
    #endregion
    #endregion
    #region Resource Management
    public static event Action OnRunOutOfYarnResource;
    
    [SerializeField] ResourceUnit idleMovementCost, sprintMovementCost;
    
    private bool SpendMovementCost()
    {
        float movementCost = DecideMovementCost() * Time.deltaTime;
        ResourceUnit resourceUnit = new ResourceUnit(idleMovementCost.type, movementCost);
        bool isResourceSpent = ResourceManager.Instance.TryToSpendResource(resourceUnit);
        return isResourceSpent;
    }
    private float DecideMovementCost()
    {
        bool isSprintButtonDown = InputManager.IsMouseLeftClick();
        float movementCost = (!isSprintButtonDown) ? idleMovementCost.amount : idleMovementCost.amount * sprintMultiplier;
        return movementCost;
    }
    private void RunOutResource(bool isResourceSpent)
    {
        if (!isResourceSpent)
        {
            StopMoving();
            OnRunOutOfYarnResource?.Invoke();
        }
    }
    #endregion  
}
