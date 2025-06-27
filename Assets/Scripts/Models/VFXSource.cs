using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COMBAT_VFX
{
    ENV_HIT_BULLET = 0,
    ZOMBIE_HIT_BULLET = 1,
    BOMB_EXPLOSION = 2,
}
public class VFXSource : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem bulletHit_Env;
    [SerializeField]
    private ParticleSystem bulletHit_Zombie;
    [SerializeField]
    private ParticleSystem bomb_Explosion;

    private Dictionary<COMBAT_VFX, Queue<ParticleSystem>> vfxPools;
    /// <summary>
    /// Pre Spawn the vfx prefabs.
    /// </summary>
    /// <param name="vfxPreset">
    /// Number of prefabs to spawn.
    /// </param>
    public void Init(int vfxPreset = 0)
    {
        vfxPools = new Dictionary<COMBAT_VFX, Queue<ParticleSystem>>()
        {
            { COMBAT_VFX.ENV_HIT_BULLET, new Queue<ParticleSystem>() },
            { COMBAT_VFX.ZOMBIE_HIT_BULLET, new Queue<ParticleSystem>() },
            { COMBAT_VFX.BOMB_EXPLOSION, new Queue<ParticleSystem>() }
        };

        for (int i = 0; i < vfxPreset; i++)
        {
            ParticleSystem envHitBullet = Instantiate(bulletHit_Env, transform);
            envHitBullet.gameObject.SetActive(false);
            vfxPools[COMBAT_VFX.ENV_HIT_BULLET].Enqueue(envHitBullet);

            ParticleSystem playerHitHead = Instantiate(bulletHit_Zombie, transform);
            bulletHit_Zombie.gameObject.SetActive(false);
            vfxPools[COMBAT_VFX.ZOMBIE_HIT_BULLET].Enqueue(playerHitHead);

            ParticleSystem playerHitBody = Instantiate(bomb_Explosion, transform);
            playerHitBody.gameObject.SetActive(false);
            vfxPools[COMBAT_VFX.BOMB_EXPLOSION].Enqueue(playerHitBody);
        }
    }
    private ParticleSystem GetPrefabForVFX(COMBAT_VFX vfx)
    {
        switch (vfx)
        {
            case COMBAT_VFX.ENV_HIT_BULLET:
                return bulletHit_Env;
            case COMBAT_VFX.ZOMBIE_HIT_BULLET:
                return bulletHit_Zombie;
            case COMBAT_VFX.BOMB_EXPLOSION:
                return bomb_Explosion;
            default:
                return null;
        }
    }

    /// <summary>
    /// Play the VFX at the hit point.
    /// </summary>
    /// <param name="vfx">
    /// Type of VFX to play.
    /// </param>
    /// <param name="hit">
    /// The hit point.
    /// </param>
    /// <param name="delay">
    /// Delay before returning the VFX to the pool, aka the duration of the VFX.
    /// </param>
    public void PlayVFX(COMBAT_VFX vfx, RaycastHit hit, float delay = 0.5f)
    {
        ParticleSystem selectedVFX = GetVFXFromPool(vfx);
        if (selectedVFX != null)
        {
            selectedVFX.transform.position = hit.point;
            selectedVFX.transform.rotation = Quaternion.LookRotation(hit.normal);

            selectedVFX.Play();
            StartCoroutine(ReturnToPoolAfterDelay(selectedVFX, vfx, delay));
        }
    }

    private ParticleSystem GetVFXFromPool(COMBAT_VFX vfx)
    {
        if (vfxPools[vfx].Count > 0)
        {
            ParticleSystem vfxInstance = vfxPools[vfx].Dequeue();
            vfxInstance.gameObject.SetActive(true);
            return vfxInstance;
        }
        else
        {
            ParticleSystem prefab = GetPrefabForVFX(vfx);
            if (prefab != null)
            {
                return Instantiate(prefab, transform);
            }
        }
        return null;
    }
       
    private IEnumerator ReturnToPoolAfterDelay(ParticleSystem particleSystem, COMBAT_VFX vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        particleSystem.gameObject.SetActive(false);
        vfxPools[vfx].Enqueue(particleSystem);
    }
       
    private void OnDestroy()
    {
        if (vfxPools != null)
        {
            foreach (var pool in vfxPools.Values)
            {
                while (pool.Count > 0)
                {
                    ParticleSystem instance = pool.Dequeue();
                    if (instance != null && instance.gameObject != null)
                    {
                        Destroy(instance.gameObject);
                    }
                }
            }
            vfxPools.Clear();
        }
    }
}

