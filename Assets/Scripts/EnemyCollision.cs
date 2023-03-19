using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCollision : MonoBehaviour
{
    
    [SerializeField] private float timePriorityMultiplier = 1.0f;
    [SerializeField] private float distancePriorityWeight = 0.5f;
    [SerializeField] private float timePriorityWeight = 0.5f;
    
    private EnemyUnit _enemyUnit;
    private NavMeshAgent _agent;
    private int startAvodancePriority;
    private float timeInScene;
    private float _spawnTime;

    private void Start()
    {
        _enemyUnit = GetComponent<EnemyUnit>();
        _agent = GetComponent<NavMeshAgent>();
        startAvodancePriority = _agent.avoidancePriority;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        TimeObjectHasBeenInSceneUpdater();
        AvoidObsticalRemoval();
    }

    private void TimeObjectHasBeenInSceneUpdater()
    {
        timeInScene = Time.time - _spawnTime;
    }

    public void AvoidObsticalRemoval()
    {
        if (_enemyUnit != null)
        {
            float singleDistance = Vector3.Distance(transform.position, _enemyUnit.TargetWaypoint.position);


            float timeBasedPriority = timeInScene * timePriorityMultiplier;

            int singleAvodancePriority = (int)(1 / singleDistance);

            float combinedPriority =
                distancePriorityWeight * singleAvodancePriority + timePriorityWeight * timeBasedPriority;

            int newAvodancePriority = Mathf.RoundToInt(combinedPriority);
            _agent.avoidancePriority = newAvodancePriority;

        }
        else
        {
            return;
        }
    }
    
}


