using StateManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnMovementController : MonoBehaviour
{
    private Transform yarnTrailPrefab;
    private Transform currentYarnTrail;
    [SerializeField] Transform yarnTrailContainer;
    [SerializeField] YarnCrosshairController yarnCrosshairController;

    [SerializeField] float idleSpeed, sprintMultiplier;

    private bool shouldMoveYarn;
    private bool shouldMoveOnArc;
    private Vector2 startPosition;

    private Vector3 currentArcVector;
    [Tooltip("It will be divided by 10")]
    [SerializeField] float arcMoveSpeed;

    private void Awake()
    {
        startPosition = transform.position;
        yarnTrailPrefab = Resources.Load<Transform>("pfYarnTrail");
    }
    private void Start()
    {
        ConquerState.OnConquerStateEnter += ConquerState_OnConquerStateEnter;
        ConquerState.OnConquerStateExit += ConquerState_OnConquerStateExit;
        ManagerState.OnManagerStateEnter += ManagerState_OnManagerStateEnter;

        YarnCrosshairController.OnYarnCrosshairEnter += YarnCrosshairController_OnYarnCrosshairEnter;
        YarnCrosshairController.OnYarnCrosshairExit += delegate (Vector2 position) { shouldMoveOnArc = false; };
    }  
    private void ConquerState_OnConquerStateEnter()
    {
        shouldMoveYarn = true;

        transform.position = startPosition;

        currentYarnTrail = Instantiate(yarnTrailPrefab, transform.position, Quaternion.identity, transform);
    }
    private void ConquerState_OnConquerStateExit()
    {
        DetacheYarn();
    }
    private void ManagerState_OnManagerStateEnter()
    {
        
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
        ConquerState.OnConquerStateEnter -= ConquerState_OnConquerStateEnter;
        ConquerState.OnConquerStateExit -= ConquerState_OnConquerStateExit;
        ManagerState.OnManagerStateEnter -= ManagerState_OnManagerStateEnter;
    }
}
