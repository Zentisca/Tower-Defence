using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{
    public int maxHealth = 100; // maximum health of the enemy
    public int currentHealth; // current health of the enemy
    public int goldOnDeath = 1;
    private GameObject _gameObjectEnemySpawner;
    private EnemySpawner enemySpawner;
    public NavMeshAgent agent;
    private Transform targetWaypoint; // current waypoint the enemy is moving towards
    public Transform TargetWaypoint => targetWaypoint;
    private Transform[] waypoints; // array of waypoints for the enemy to move through
    private int waypointIndex = 0; // current index in the waypoints array
    private bool isMoving = false; // flag to check if enemy is currently moving
    [SerializeField] private bool isFlying = false; // true if the enemy can fly
    [SerializeField] private float flyingHeight = 5f; // height at which the enemy will fly
    private EnemyCollision _enemyCollision;
    

    void Start()
    {
        _enemyCollision = GetComponent<EnemyCollision>();

        currentHealth = maxHealth; // set the current health to the maximum health at the start
        _gameObjectEnemySpawner = GameObject.Find("EnemySpawner");
        enemySpawner = _gameObjectEnemySpawner.GetComponent<EnemySpawner>();
        agent = GetComponent<NavMeshAgent>();
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        //AssignWaypoints(waypoints);

        if (isFlying)
        {
            // if the enemy can fly, adjust the starting position to be at the specified height
            transform.position = new Vector3(transform.position.x, flyingHeight, transform.position.z);
        }
        else
        {
            // if the enemy cannot fly, adjust the starting position to be on the ground
            transform.position = new Vector3(transform.position.x, NavMesh.SamplePosition(transform.position, out 
                NavMeshHit hit, 10f, NavMesh.AllAreas) ? hit.position.y : 
                transform.position.y, transform.position.z);
        }

        transform.LookAt(waypoints[waypointIndex].transform.position);

        // set the NavMeshAgent's destination to the first waypoint
        targetWaypoint = waypoints[waypointIndex];
        agent.SetDestination(targetWaypoint.position);

        // set the isMoving flag to true
        isMoving = true;
    }

    void Update()
    {
        // check if the enemy has reached its current waypoint
        if (isMoving && agent.remainingDistance <= agent.stoppingDistance)
        {
            
            isMoving = false;

            // increment the waypoint index and check if we have reached the end
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
            {
                // if we have reached the last waypoint, do something (e.g. take away health from the player)
                ReachedEnd();
                //GameObject.Find("Player Elements").GetComponent<Player>().LoseCow();
                Destroy(gameObject);
                return;
            }

            // set the next waypoint as the target and start moving again
            targetWaypoint = waypoints[waypointIndex];
            agent.SetDestination(targetWaypoint.position);
            //_enemyCollision.ResetAvoidObsitcalValue();
            isMoving = true;

            // play the walk animation
            //animator.SetBool("isWalking", true);
        }
        _enemyCollision.AvoidObsticalRemoval();
        
            // rotate towards the next waypoint
            Vector3 direction = targetWaypoint.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
        if (isFlying)
        {
            // if the enemy can fly, adjust the position to be at the specified height
            transform.position = new Vector3(transform.position.x, flyingHeight, transform.position.z);
        }
        else
        {
            // if the enemy cannot fly, adjust the position to be at the sampled height of the NavMesh
            transform.position = new Vector3(transform.position.x, NavMesh.SamplePosition(transform.position, 
            out NavMeshHit hit, 10f, NavMesh.AllAreas) ? hit.position.y : transform.position.y, 
            transform.position.z);
        }
        
    }

    
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // subtract damage from current health
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // do something when the enemy dies (e.g. give the player points or money)
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
        // Implement any logic when the enemy reaches the end, e.g. reduce player lives
    }

}
