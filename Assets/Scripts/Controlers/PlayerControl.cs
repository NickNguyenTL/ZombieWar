using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlayerControl : MonoBehaviour
{
    public Action<int> OnPlayerDamageTaken;

    [SerializeField]
    private LayerMask enemyLayerMask;
    [SerializeField]
    private LayerMask obstacleLayerMask;

    [Header("Player Data")]
    [SerializeField]
    private CharacterController playerController; // Character controller for the player character
    [SerializeField]
    private Animator playerAnimator; // Animator for the player character
    [SerializeField]
    private Collider playerCollider;
    [SerializeField]
    private float invisibleTime = 0.5f; // Time in seconds for which the player is invisible after taking damage
    [SerializeField]
    private int maxHealth = 5; // Maximum health of the player

    [SerializeField]
    private Transform weaponTransform;
    [SerializeField]
    private List<WeaponModel> Weapons;

    private WeaponModel currentWeapon;
    private float attackCooldown = 0f;
    private int currentHealth;
    private FXSource fxSource;
    public void Init(FXSource _fXSource)
    {
        playerCollider.enabled = true;
        currentHealth = maxHealth;

        SetWeapon(0); // Set the initial weapon, assuming the first weapon in the list is the default one
        fxSource = _fXSource;

        attackCooldown = 0f;
    }

    public void SetWeapon(int weaponId)
    {
        for(int i = 0; i < Weapons.Count; i++)
        {
            Weapons[i].gameObject.SetActive(i == weaponId);
        }
        currentWeapon = Weapons[weaponId];
        currentWeapon.Init(); // Initialize the weapon
        attackCooldown = currentWeapon.AttackSpeed;
    }

    public Transform FindClosestEnemy(float maxDistance, float maxAngle)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, maxDistance, enemyLayerMask);
        Transform closest = null;
        float minDist = maxDistance;

        foreach (var hit in hits)
        {
            Vector3 dirToEnemy = (hit.transform.position - transform.position).normalized;
            //float angle = Vector3.Angle(transform.forward, dirToEnemy);
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }
        return closest;
    }

    public void Attack(RaycastHit hit)
    {
        if (currentWeapon == null)
            return;

        if (hit.collider != null)
        {
            EnemyControl enemyControl = hit.collider.GetComponent<EnemyControl>();
            if (enemyControl != null)
            {
                enemyControl.TakeDamage(currentWeapon.Damage);
                Debug.Log($"Attacking {enemyControl.name} with {currentWeapon.WeaponName}");
                fxSource.PlayFX(COMBAT_FX.ZOMBIE_HIT_BULLET, hit, 0.5f); // Play attack VFX
            }
            else
            {
                fxSource.PlayFX(COMBAT_FX.ENV_HIT_BULLET, hit, 0.5f); // Play attack VFX
            }
        }            

        currentWeapon.PlayAttack();
        playerAnimator.SetTrigger("Shoot");
    }

    private void RotateToward(Vector3 dir)
    {
        //Quaternion weaponLookRotation = Quaternion.LookRotation(dir);
        //weaponTransform.rotation = weaponLookRotation;
        Quaternion playerLookRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = playerLookRotation;
    }

    RaycastHit hit;
    /// <summary>
    /// Raycast forward from the player to detect enemies within range and angle.
    /// </summary>
    private RaycastHit HitScanForward(Transform target)
    {
        Vector3 raycastDir = (target != null) ? (target.position - transform.position).normalized : transform.forward;
        LayerMask layerMask = enemyLayerMask | obstacleLayerMask;

        // Perform a raycast to check for the first enemy in the direction of the raycast
        if (Physics.Raycast(transform.position, raycastDir, out hit, currentWeapon.Range, layerMask))
        {
            if (target != null)
            {
                //Rotate towards the target
                RotateToward(raycastDir);
            }
            return hit;
        }

        return new RaycastHit(); // Return an empty hit if no enemy is found
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyControl enemyControl = other.GetComponent<EnemyControl>();
            if (enemyControl != null)
            {
                TakeDamage(enemyControl.GetEnemyData().curDamage); // Take damage from the enemy
            }
        }
    }

    Coroutine invisibleCoroutine;
    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
       // Handle player taking damage
        OnPlayerDamageTaken?.Invoke(currentHealth);

        if(invisibleCoroutine != null)
        {
           StopCoroutine(invisibleCoroutine);
        }
        invisibleCoroutine = StartCoroutine(InvisibleProcess(invisibleTime)); // Start the coroutine to make the player invisible
    }

    IEnumerator InvisibleProcess(float timer)
    {
        playerCollider.enabled = false; // Make the player invisible
        yield return new WaitForSeconds(timer);
        playerCollider.enabled = true; // Make the player visible again
    }

    private void Update()
    {
        if (attackCooldown <= 0)
        {
            Transform target = FindClosestEnemy(currentWeapon.Range, 100f);
            
            var hit = HitScanForward(target);            
            Attack(hit);
            attackCooldown = currentWeapon.AttackSpeed; // Reset cooldown
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void PlayerDeath()
    {
        if (invisibleCoroutine != null)
        {
            StopCoroutine(invisibleCoroutine);
        }
        playerCollider.enabled = false; // Make the player invisible
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere to visualize the attack range
        // Draw a ray in the direction of the attack
        if (currentWeapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeapon.Range);
            Vector3 raycastDir = transform.forward * currentWeapon.Range;
            Gizmos.DrawRay(transform.position, raycastDir);
        }

        if(hit.collider != null)
        {             
            // Draw a line to the hit point
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.DrawSphere(hit.point, 0.1f); // Draw a small sphere at the hit point
        }
    }
}
