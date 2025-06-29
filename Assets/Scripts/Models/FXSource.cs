using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COMBAT_FX
{
    ENV_HIT_BULLET = 0,
    ZOMBIE_HIT_BULLET = 1,
    BOMB_EXPLOSION = 2,
}
public class FXSource : MonoBehaviour
{
    [Header ("VFX")]
    [SerializeField]
    private ParticleSystem bulletHit_Env;
    [SerializeField]
    private ParticleSystem bulletHit_Zombie;
    [SerializeField]
    private ParticleSystem bomb_Explosion;

    [Header ("SFX")]
    [SerializeField]
    private AudioClip bulletHit_Env_SFX;
    [SerializeField]
    private AudioClip bulletHit_Zombie_SFX;
    [SerializeField]
    private AudioClip bomb_Explosion_SFX;
    [SerializeField]
    private AudioSource fxAudioSource;

    private Dictionary<COMBAT_FX, Queue<ParticleSystem>> vfxPools;
    private Dictionary<COMBAT_FX, AudioClip> sfxPools;
    /// <summary>
    /// Pre Spawn the vfx prefabs.
    /// </summary>
    /// <param name="vfxPreset">
    /// Number of prefabs to spawn.
    /// </param>
    public void Init(int vfxPreset = 0)
    {
        vfxPools = new Dictionary<COMBAT_FX, Queue<ParticleSystem>>()
        {
            { COMBAT_FX.ENV_HIT_BULLET, new Queue<ParticleSystem>() },
            { COMBAT_FX.ZOMBIE_HIT_BULLET, new Queue<ParticleSystem>() },
            { COMBAT_FX.BOMB_EXPLOSION, new Queue<ParticleSystem>() }
        };

        for (int i = 0; i < vfxPreset; i++)
        {
            ParticleSystem envHitBullet = Instantiate(bulletHit_Env, transform);
            envHitBullet.Stop();
            vfxPools[COMBAT_FX.ENV_HIT_BULLET].Enqueue(envHitBullet);

            ParticleSystem playerHitHead = Instantiate(bulletHit_Zombie, transform);
            bulletHit_Zombie.Stop();
            vfxPools[COMBAT_FX.ZOMBIE_HIT_BULLET].Enqueue(playerHitHead);

            ParticleSystem playerHitBody = Instantiate(bomb_Explosion, transform);
            playerHitBody.Stop();
            vfxPools[COMBAT_FX.BOMB_EXPLOSION].Enqueue(playerHitBody);
        }

        sfxPools = new Dictionary<COMBAT_FX, AudioClip>()
        {
            { COMBAT_FX.ENV_HIT_BULLET, bulletHit_Env_SFX },
            { COMBAT_FX.ZOMBIE_HIT_BULLET, bulletHit_Zombie_SFX },
            { COMBAT_FX.BOMB_EXPLOSION, bomb_Explosion_SFX }
        };
    }
    private ParticleSystem GetPrefabForVFX(COMBAT_FX vfx)
    {
        switch (vfx)
        {
            case COMBAT_FX.ENV_HIT_BULLET:
                return bulletHit_Env;
            case COMBAT_FX.ZOMBIE_HIT_BULLET:
                return bulletHit_Zombie;
            case COMBAT_FX.BOMB_EXPLOSION:
                return bomb_Explosion;
            default:
                return null;
        }
    }

    /// <summary>
    /// Play the FX at the hit point.
    /// </summary>
    /// <param name="fx">
    /// Type of VFX to play.
    /// </param>
    /// <param name="hit">
    /// The hit point.
    /// </param>
    /// <param name="delay">
    /// Delay before returning the VFX to the pool, aka the duration of the VFX.
    /// </param>
    public void PlayFX(COMBAT_FX fx, RaycastHit hit, float delay = 0.5f)
    {
        if (sfxPools.ContainsKey(fx))
        {
            AudioClip selectedSFX = sfxPools[fx];
            if (selectedSFX != null && fxAudioSource != null)
            {
                fxAudioSource.PlayOneShot(selectedSFX);
            }
        }

        ParticleSystem selectedVFX = GetVFXFromPool(fx);
        if (selectedVFX != null)
        {
            selectedVFX.transform.position = hit.point;
            selectedVFX.transform.rotation = Quaternion.LookRotation(hit.normal);

            selectedVFX.Play();
            StartCoroutine(ReturnToPoolAfterDelay(selectedVFX, fx, delay));
        }
    }

    private ParticleSystem GetVFXFromPool(COMBAT_FX vfx)
    {
        if (vfxPools[vfx].Count > 0)
        {
            ParticleSystem vfxInstance = vfxPools[vfx].Dequeue();
            vfxInstance.Stop();
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
       
    private IEnumerator ReturnToPoolAfterDelay(ParticleSystem particleSystem, COMBAT_FX vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        particleSystem.Stop();
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

