using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayerMask;

    public Transform FindClosestEnemy(float maxDistance, float maxAngle)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, maxDistance, enemyLayerMask);
        Transform closest = null;
        float minAngle = maxAngle;
        float minDist = maxDistance;

        foreach (var hit in hits)
        {
            Vector3 dirToEnemy = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (angle < minAngle && dist < minDist)
            {
                minAngle = angle;
                minDist = dist;
                closest = hit.transform;
            }
        }
        return closest;
    }
}
