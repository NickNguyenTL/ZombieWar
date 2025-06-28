using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayerMask;
    [SerializeField]
    private LayerMask obstacleLayerMask;

    [SerializeField]
    private Transform weaponTransform;

    private WeaponModel currentWeapon;
    private float attackCooldown = 0f;
    public void Init()
    {
        
    }

    public void SetWeapon(WeaponModel weapon)
    {
        currentWeapon = weapon;
        attackCooldown = weapon.AttackSpeed;
    }

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

    public void Attack(EnemyControl enemyControl)
    {
        if (currentWeapon == null || enemyControl == null)
            return;

        // Implement attack logic here        
        if (enemyControl != null)
        {            
            enemyControl.GetDamage(currentWeapon.Damage);
        }
        currentWeapon.PlayAttack();

        Debug.Log($"Attacking {enemyControl.name} with {currentWeapon.WeaponName}");
    }

    private void RotateToward(Vector3 dir)
    {
        Quaternion weaponLookRotation = Quaternion.LookRotation(dir);
        weaponTransform.rotation = weaponLookRotation;

        Quaternion playerLookRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = playerLookRotation;
    }

    /// <summary>
    /// Raycast forward from the player to detect enemies within range and angle.
    /// </summary>
    private void HitScanForward(Transform target)
    {
        Vector3 raycastDir = (target != null) ? (target.position - transform.position).normalized : transform.forward;
        RaycastHit hit;
        LayerMask layerMask = enemyLayerMask | obstacleLayerMask;

        if (target != null)
        {
            //Rotate towards the target
            RotateToward(raycastDir);
        }

        // Perform a raycast to check for the first enemy in the direction of the raycast
        if (Physics.Raycast(transform.position, raycastDir, out hit, currentWeapon.Range, layerMask))
        {
            EnemyControl enemyControl = hit.collider.GetComponent<EnemyControl>();
            if (enemyControl != null)
            {
                Attack(enemyControl);
            }
        }
    }

    private void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (attackCooldown <= 0)
        {
            Transform target = FindClosestEnemy(currentWeapon.Range, 60f);

            
            HitScanForward(target);
            attackCooldown = currentWeapon.AttackSpeed; // Reset cooldown
        }
    }
}
