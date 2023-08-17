using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.AI;

public class SeekerEnemyAI : MonoBehaviour
{
    // Humanoid variables
    public NavMeshAgent agent;
    public Transform player;
    public PlayerMovementFPS playerScript;

    public Transform[] wayPoints;
    private int currentWayPointIndex;

    // Patroling
    public Vector3 walkPoint;  // not needed
    bool walkPointSet;
    public float walkPointRange;

    // Attacking not needed
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerInFOV, seekerEnemyActive;
    [Range(0, 360)]
    public float fovAngle;

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
        sightRange = 5f;
        attackRange = 100f;
        agent.speed = 10f;

        walkPointSet = true;
        currentWayPointIndex = 0;

        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        //agent.SetDestination(player.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInAttackRange = (distanceToPlayer <= attackRange);

        //if (playerInFOV && playerInAttackRange) AttackPlayer();

        //if (!playerInFOV) Patrol();
        //if (playerInFOV && !playerInAttackRange) ChasePlayer();

        UpdatePostProc(distanceToPlayer);

        if (!playerInAttackRange || player.GetComponent<PlayerObject>().isPlayerHidden())
        {
            if (player.GetComponent<PlayerObject>().isPlayerHidden())
            {
                Debug.Log("Player hidden: Calling patrol()");
            }
            Patrol();
        }
        else ChasePlayer();
    }

    private void UpdatePostProc(float d)
    {
        Debug.Log("distance: " + d);
        if (vignette)
        {
            float scale = 0.85f - (d / 75.0f) * 0.375f;
            vignette.intensity.Override(scale);
            Debug.Log("intensity: " + vignette.intensity);
        }
        if (filmGrain)
        {
        
        }
        if (chromaticAberration)
        {
        
        }
    }

    IEnumerator FOVRoutine()
    {
        float count = 0;
        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (seekerEnemyActive)
        {
            yield return waitTime;
            CheckFOV();
            count++;
        }
    }

    void CheckFOV()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, sightRange, playerMask);
        if (objectsInRange.Length != 0)
        {
            Transform target = objectsInRange[0].transform;
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < fovAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, target.position);
                playerInFOV = Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask) ? false : true;

            }
            else
            {
                playerInFOV = false;
            }
        }
        else if (playerInFOV)
        {
            playerInFOV = false;
        }
    }

    void Patrol()
    {
       // Debug.Log("inside Patrol()");
        //bool success = false;
        //if (!walkPointSet) SearchForWalkPoint();
        //else success = agent.SetDestination(walkPoint);
        //if (success) Debug.Log("s######scscscs");

        if (!walkPointSet)
        {
            SelectWalkPoint();
        }

        Vector3 destination = wayPoints[currentWayPointIndex].position;
        destination.y = transform.position.y;
        agent.SetDestination(destination);
        
        Vector3 distanceToWalkPoint = transform.position - wayPoints[currentWayPointIndex].position;
        distanceToWalkPoint.y = 0;
        if (distanceToWalkPoint.magnitude < 0.5f) walkPointSet = false;
    }

    void SelectWalkPoint()
    {
        //Debug.Log("Entered SelectWalkPoint");
        float minPlayerDistance = float.MaxValue;
        int minDistanceWayPoint = -1;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            if (i!=currentWayPointIndex &&
                Vector3.Distance(wayPoints[i].position, player.position) < minPlayerDistance)
            {
                minDistanceWayPoint = i;
                minPlayerDistance = Vector3.Distance(wayPoints[i].position, player.position);
            }
        }

        currentWayPointIndex = minDistanceWayPoint;
        //Debug.Log("WayPoint ID: " + currentWayPointIndex);
        walkPointSet = true;
    }

    void ChasePlayer()
    {
        //Debug.Log("inside chase player");
        walkPointSet = false;
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            //playerScript.KillPlayer();
            //Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void SearchForWalkPoint()
    {
        //Debug.Log("inside SearchForWalkPoint()"); 
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, floorMask)) walkPointSet = true;
    }
}
