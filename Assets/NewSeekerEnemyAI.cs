using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NewSeekerEnemyAI : MonoBehaviour
{
    // Humanoid variables
    public NavMeshAgent agent;
    public Transform player;
    public PlayerMovementFPS playerScript;
    private PlayerObject playerObject;
    public float yWhy;

    // Navigation
    public Transform[] wayPoints;
    private int currentWayPointIndex;

    // Patroling
    public Vector3 walkPoint;  // not needed
    bool walkPointSet;
    public float walkPointRange;
    
    // States
    public int seekerMode;
    public int RANDOM_MODE = 0;
    public int SCOUT_MODE = 1;
    public int CHASE_MODE = 2;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerInFOV, seekerEnemyActive;
    [Range(0, 360)]
    
    // Mask variables
    public LayerMask floorMask, playerMask, obstructionMask;

    // Graphics
    private VolumeProfile postProc = null;
    private Vignette vignette = null;
    private FilmGrain filmGrain = null;
    private ChromaticAberration chromaticAberration = null;


    void Awake()
    {
        player = GameObject.Find("PlayerHolder").transform;
        playerScript = player.GetComponent<PlayerMovementFPS>();
        playerObject = player.GetComponent<PlayerObject>();

        postProc = GameObject.Find("PostProcessing")?.GetComponent<Volume>()?.profile;
        if (postProc)
        {
            postProc.TryGet(out Vignette vig);
            postProc.TryGet(out FilmGrain fg);
            postProc.TryGet(out ChromaticAberration ca);

            vignette = vig;
            filmGrain = fg;
            chromaticAberration = ca;
        }

        playerMask = LayerMask.GetMask("Player");
        floorMask = LayerMask.GetMask("Floor");
        obstructionMask = LayerMask.GetMask("Default");

        agent = GetComponent<NavMeshAgent>();
        seekerEnemyActive = true;
        seekerMode = SCOUT_MODE;
        sightRange = 5f;
        attackRange = 100f;
        agent.speed = 20f;

        walkPointSet = true;
        currentWayPointIndex = 0;

        //StartCoroutine(FOVRoutine());
    }

    private void Start()
    {
        GameEvents.current.moveEnemyWaypoint += moveToNearestWayPoint;
    }

    void Update()
    {
        if (seekerMode == CHASE_MODE)
        {
            if (player.GetComponent<PlayerObject>().isPlayerHidden())
            {
                seekerMode = RANDOM_MODE;
                walkPointSet = false;
                return;
            }
            
            agent.SetDestination(player.position);
            walkPointSet = false;
        }
        else if (seekerMode == SCOUT_MODE) 
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            playerInAttackRange = (distanceToPlayer <= attackRange);

            if (playerInAttackRange)
            {
                seekerMode = CHASE_MODE;
                agent.SetDestination(player.position);
                walkPointSet = false;
            }
            else
            {
                float minPlayerDistance = float.MaxValue;
                int minDistanceWayPoint = -1;
                if (wayPoints.Length == 0)
                {
                    Debug.Log("Danger!");
                }
                for (int i = 0; i < wayPoints.Length; i++)
                {
                    //if (i!=currentWayPointIndex &&
                    if (
                        Vector3.Distance(wayPoints[i].position, player.position) < minPlayerDistance)
                    {
                        minDistanceWayPoint = i;
                        minPlayerDistance = Vector3.Distance(wayPoints[i].position, player.position);
                    }
                }

                currentWayPointIndex = minDistanceWayPoint;
            
                Vector3 destination = wayPoints[currentWayPointIndex].position;
                //destination.y = player.position.y;
                destination.y = yWhy;
                agent.SetDestination(destination);
                walkPointSet = false;
            }
        }
        else if (seekerMode == RANDOM_MODE)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            playerInAttackRange = (distanceToPlayer <= attackRange);
            if (playerInAttackRange && !player.GetComponent<PlayerObject>().isPlayerHidden())
            {
                seekerMode = CHASE_MODE;
                agent.SetDestination(player.position);
                walkPointSet = false;
            }
            if (!walkPointSet)
            {
                System.Random rnd = new System.Random();
                currentWayPointIndex = rnd.Next(0, wayPoints.Length);
                walkPointSet = true;
            }
            
            Vector3 destination = wayPoints[currentWayPointIndex].position;
            //destination.y = player.position.y;
            destination.y = yWhy;
            agent.SetDestination(destination);

            Vector3 distanceToWalkPoint = transform.position - wayPoints[currentWayPointIndex].position;
            distanceToWalkPoint.y = 0;
            if (distanceToWalkPoint.magnitude < 0.5f) walkPointSet = false;
        }

        UpdatePostProc();
    }

    private void UpdatePostProc()
    {
        float d = Mathf.Min(Vector3.Distance(transform.position, player.position), 75.0f);
        if (vignette)
        {
            float scale = playerObject.isPlayerHidden() ? Mathf.Max(0.7f, 0.85f - (d / 75.0f) * 0.375f) : 0.85f - (d / 75.0f) * 0.375f;
            vignette.intensity.Override(scale);
        }
        if (filmGrain)
        {
            float scale = 1.0f - (d / 75.0f) * 0.65f;
            filmGrain.intensity.Override(scale);
        }
        if (chromaticAberration)
        {
            float scale = 1.0f - (d / 75.0f);
            chromaticAberration.intensity.Override(scale);
        }
    }

    public void moveToNearestWayPoint()
    {
        int minDistanceWayPoint = -1;
        float minPlayerDistance = float.MaxValue;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            //if (i!=currentWayPointIndex &&
            if (
                Vector3.Distance(wayPoints[i].position, player.position) < minPlayerDistance)
            {
                minDistanceWayPoint = i;
                minPlayerDistance = Vector3.Distance(wayPoints[i].position, player.position);
            }
        }

        transform.position = new Vector3(wayPoints[minDistanceWayPoint].position.x, transform.position.y,
            wayPoints[minDistanceWayPoint].position.z);
        seekerMode = CHASE_MODE;
    }
}