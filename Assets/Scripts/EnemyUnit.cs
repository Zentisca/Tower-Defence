using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    public int currentHealth; // current health of the enemy
    public int maxHealth = 100; // maximum health of the enemy
    public int goldOnDeath = 1;
    [SerializeField] private bool isFlying = false;
    [SerializeField] private float flyingHeight = 5f;
    public NavMeshAgent agent;
    
    private bool isMoving = false; 
    private int waypointIndex = 0;
    private Transform[] waypoints; 
    private Transform targetWaypoint;
    private GameObject _gameObjectEnemySpawner;
    private EnemySpawner enemySpawner;
    private EnemyCollision _enemyCollision;
    public Transform TargetWaypoint => targetWaypoint;
    
    void Start()
    {
        _gameObjectEnemySpawner = GameObject.Find("EnemySpawner");
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        enemySpawner = _gameObjectEnemySpawner.GetComponent<EnemySpawner>();
        agent = GetComponent<NavMeshAgent>();
        _enemyCollision = GetComponent<EnemyCollision>();
        currentHealth = maxHealth;

        if (waypoints != null && waypoints.Length > 0)
        {
            transform.LookAt(waypoints[waypointIndex].transform.position);
            targetWaypoint = waypoints[waypointIndex];
            agent.SetDestination(targetWaypoint.position);
            isMoving = true;
        }
        else
        {
            Debug.LogError("Waypoints not assigned to enemy");
        }

        if (isFlying)
        {
            FlyingUnitHeightSet();
        }
        else
        { 
            WalkingUnit();
        }
    }

    void Update()
    {
        if (isMoving && agent.remainingDistance <= agent.stoppingDistance)
        {
            UnitMovement();
        }
        _enemyCollision.AvoidObsticalRemoval();
        
        Vector3 direction = targetWaypoint.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        IsUnitFlyingOrWalking();
    }

    private void UnitMovement()
    {
        isMoving = false;
            
        waypointIndex++;
        if (waypointIndex >= waypoints.Length)
        {
            ReachedEnd();
            Destroy(gameObject);
            return;
        }
            
        targetWaypoint = waypoints[waypointIndex];
        agent.SetDestination(targetWaypoint.position);
        isMoving = true;

    }
    
    private void IsUnitFlyingOrWalking()
    {
        if (isFlying)
        {
            FlyingUnitHeightSet();
        }
        else
        {
            WalkingUnit();
        }
    }
    
    private void FlyingUnitHeightSet()
    {
        transform.position = new Vector3(transform.position.x, flyingHeight, transform.position.z);
    }

    private void WalkingUnit()
    {
        transform.position = new Vector3(transform.position.x, NavMesh.SamplePosition(transform.position, 
                out NavMeshHit hit, 10f, NavMesh.AllAreas) ? hit.position.y : transform.position.y, 
                transform.position.z);
    }
    
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameObject player = GameObject.Find("Player Elements");
        Player playerComponent = player.GetComponent<Player>();
        playerComponent.AddGold(goldOnDeath);
        enemySpawner.IncreaseKills();
        Destroy(gameObject);
    }   

    public void AssignWaypoints(Transform[] waypoints)
    {
        this.waypoints = waypoints;
        targetWaypoint = waypoints[waypointIndex];
        agent.SetDestination(targetWaypoint.position);
    }
    public void ReachedEnd()
    {
        enemySpawner.EnemyPassed();
        GameObject.Find("Player Elements").GetComponent<Player>().LoseCow();
    }

}