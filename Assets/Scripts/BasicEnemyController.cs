using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening; 
using Random = UnityEngine.Random;


public class BasicEnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;

    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public GameObject projectile;
    public float forwardShotPower;
    public float verticalShotPower;
    public bool strafeAgent;
    private bool strafe_dir;

    public bool allowStrafe;
    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // UI elements
    public Canvas playerSpotted;
    private Animator playerSpottedAnimation;
    public EnemyHealthBar enemyHealthBar;

    // Enemy Variables
    public float maxHealth;
    public float health;

    // Misc. variables
    bool animationPlayed;

    public Transform player;
    public PlayerObject playerObject;

    public bool dead = false;
    public bool underAttack;

    private void Awake()
    {
        playerSpotted = transform.Find("PlayerSpotted").GetComponent<Canvas>();
        playerSpottedAnimation = playerSpotted.gameObject.GetComponent<Animator>();
        enemyHealthBar = transform.Find("EnemyHealth").transform.Find("HealthBar").GetComponent<EnemyHealthBar>();
        animationPlayed = false;
        strafe_dir = true;
        underAttack = false;
    }

    private void Patrol()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f * transform.localScale.y, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void AttackPlayer()
    {

        if (!strafeAgent)
            agent.SetDestination(transform.position);
        else
        {
            if (allowStrafe)
            {
                Strafe(strafe_dir);
                strafe_dir = !strafe_dir;
                allowStrafe = false;
                Invoke(nameof(AllowStrafe), 6f);
            }
        }


        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * forwardShotPower, ForceMode.Impulse);
            rb.AddForce(transform.up * verticalShotPower, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    public void TakeDamage(float damage)
    {
      
        health = Math.Max(health - damage, 0);
        
        float destroyTime = 0;
        destroyTime = enemyHealthBar.UpdateHealthBar(damage);
        underAttack = true;

        if (health == 0)
        {
            dead = true;
        }
        if (dead)
        {
            Invoke(nameof(DestroyEnemy), destroyTime);
        }
        else
        {
            Invoke(nameof(ResetUnderAttack), 5f);
        }
        
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
   
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange && !playerObject.isPlayerHidden())
        {
            AttackPlayer();
        }
        else if ((playerInSightRange && !playerObject.isPlayerHidden()) || underAttack)
        {
            playerSpotted.transform.LookAt(player);
            enemyHealthBar.transform.parent.LookAt(player);

            if (!animationPlayed)
            {
                playerSpotted.gameObject.SetActive(true);
                enemyHealthBar.transform.parent.gameObject.SetActive(true);
                DOTween.To(() => enemyHealthBar.transform.parent.GetComponent<CanvasGroup>().alpha, x => enemyHealthBar.transform.parent.GetComponent<CanvasGroup>().alpha = x, 1, 0.5f);
                playerSpottedAnimation.Play("PlayerSpottedAnimation", -1, 0);
                animationPlayed = true;
            }
            ChasePlayer();
        }
        else
        {
            Patrol();
            if (animationPlayed)
            {
                DOTween.To(() => enemyHealthBar.transform.parent.GetComponent<CanvasGroup>().alpha, x => enemyHealthBar.transform.parent.GetComponent<CanvasGroup>().alpha = x, 0, 0.5f).OnComplete(() => enemyHealthBar.transform.parent.gameObject.SetActive(false));

            }
            animationPlayed = false;
        }

    }

    public void Strafe(bool dir_left)
    {
        Vector3 left = -agent.transform.right;
        Vector3 right = agent.transform.right;
        
        if (dir_left)
            agent.SetDestination(agent.transform.position + left * 5);
        else
            agent.SetDestination(agent.transform.position + right * 5);
    }

    void AllowStrafe()
    {
        allowStrafe = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    void ResetUnderAttack()
    {
        underAttack = false;
    }
}


