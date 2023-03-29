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

        TutorialManager.OnStageReveale += PauseYarn;
        TutorialManager.OnStageHide += TutorialManager_OnStageHide;
    }

    private void TutorialManager_OnStageHide(GameObject obj)
    {
        ContinueYarn();
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
    private TrailRenderer currentYarnTrail;
    private bool shouldMoveYarn;
    private bool isPaused = true;
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
        transform.position = startPosition;
        currentYarnTrail = Instantiate(yarnTrailPrefab, startPosition, Quaternion.identity, transform).GetComponent<TrailRenderer>();
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

        currentYarnTrail.emitting = false;
        currentYarnTrail.transform.parent = yarnTrailContainer;
    }
    private void ContinueYarn()
    {
        isPaused = false;
        
        if (!currentYarnTrail) return;
        currentYarnTrail.emitting = true;
    }
    private void PauseYarn()
    {
        isPaused = true;
        
        if (!currentYarnTrail) return;
        currentYarnTrail.emitting = false;
    }
    private void ManageMovement()
    {
        if (!shouldMoveYarn) return;
        if (isPaused) return;

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
    [SerializeField] float doubleClickdelayTime;

    private bool isSprint, isSimpleClick;
    private Coroutine DoubleClickCoroutine;

    private void SimpleMovement()
    {
        SetIsSprint();
        
        Vector2 mousePosition = yarnCrosshairController.transform.position;
        Debug.DrawLine(transform.position, mousePosition, Color.red);
        Vector3 direction = (mousePosition - (Vector2)transform.position).normalized;

        MoveTransformToDirection(direction, DecideSpeed());
    }
    private void MoveTransformToDirection(Vector3 direction, float moveSpeed)
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    private float DecideSpeed()
    {
        float moveSpeed = (!isSprint) ? idleSpeed : idleSpeed * sprintMultiplier;
        return moveSpeed;
    }
    private void SetIsSprint()
    {
        bool isClick = InputManager.IsMouseLeftClickPressed();
        bool isClickHold = InputManager.IsMouseLeftClick();

        //Cancel sprint
        if (!isClickHold) isSprint = false;

        //Start double click
        if (!isSimpleClick && isClick)
        {
            isSimpleClick = true;
            DoubleClickCoroutine = StartCoroutine(DoubleClickDelay());
            return;
        }

        //Succesfull double click
        if (isSimpleClick && isClick)
        {
            isSimpleClick = false;
            isSprint = true;
            StopCoroutine(DoubleClickCoroutine);
        }
    }
    //Cancel double click with delay
    private IEnumerator DoubleClickDelay()
    {
        yield return new WaitForSeconds(doubleClickdelayTime);
        isSimpleClick = false;
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
        float movementCost = (!isSprint) ? idleMovementCost.amount : idleMovementCost.amount * sprintMultiplier;
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
