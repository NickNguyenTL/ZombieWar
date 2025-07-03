using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChaserSystem: MonoBehaviour
{
    public Action<bool> OnChaseStateChanged;

    [Header("Chase Settings")]
    [SerializeField]
    private float chaseDistance = 20f;
    [SerializeField]
    private float stopDistance = 0f;
    [SerializeField]
    private float updateRate = 0.1f;

    [Header("Behavior")]
    [SerializeField]
    private bool stopWhenReached = true;   

    private NavMeshAgent agent;
    private float lastUpdateTime;
    private bool isChasing;
    public  bool IsChasing
    {
        get => isChasing;
        private set
        {
            if(isChasing != value)
            {
                OnChaseStateChanged?.Invoke(value);
                isChasing = value;
            }            
        }
    }
    private Transform target;

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            enabled = false;
            return;
        }

        // Set the stopping distance
        agent.stoppingDistance = stopDistance;
        IsChasing = false;
    }    

    void Update()
    {
        if (target == null || agent == null)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Check if target is within chase range
        if (distanceToTarget <= chaseDistance)
        {
            // Update destination at specified rate
            if (Time.time - lastUpdateTime >= updateRate)
            {
                UpdateChase();
                lastUpdateTime = Time.time;
            }
        }
        else
        {
            // Target is too far away
            StopChasing();
        }
    }

    public void SetSpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }

    void UpdateChase()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Only update destination if we're not already close enough
        if (distanceToTarget > stopDistance)
        {
            // Check if the target position is on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target.position, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                StartChasing();
            }
        }
        else if (stopWhenReached)
        {
            // We're close enough, stop if configured to do so
            StopChasing();
        }
    }

    public void StartChasing()
    {
        if (agent != null && target != null)
        {
            agent.isStopped = false;
            IsChasing = true;
        }
    }

    public void StopChasing()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            IsChasing = false;
        }
    }

    #region Getters and Setters
    public void SetTarget(Transform newTarget)
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not initialized!");
            StopChasing();
            return;
        }

        target = newTarget;
        if (target == null)
        {
            OnChaseStateChanged = null;
            StopChasing();
            return;
        }
    }

    public void SetChaseDistance(float distance)
    {
        chaseDistance = distance;
    }

    public void SetStopDistance(float distance)
    {
        stopDistance = distance;
        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
        }
    }
    public float GetDistanceToTarget()
    {
        return target != null ? Vector3.Distance(transform.position, target.position) : float.MaxValue;
    }
    public NavMeshAgent GetAgent()
    {
        return agent;
    }
    #endregion

    // Gizmos for visualization in Scene view
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Draw chase range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // Draw stop distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Draw line to target
        Gizmos.color = isChasing ? Color.green : Color.gray;
        Gizmos.DrawLine(transform.position, target.position);
    }
}