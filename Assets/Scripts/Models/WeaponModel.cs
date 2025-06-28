using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [SerializeField]
    private WeaponData weaponData;
    [SerializeField]
    private ParticleSystem attackFx;

    public void Init()
    {
        if (attackFx != null)
        {
            attackFx.Stop();
        }
    }

    public int Damage
    {
        get { return weaponData.damage; }
    }

    public float AttackSpeed
    {
        get { return weaponData.attackSpeed; }
    }

    public string WeaponName
    {
        get { return weaponData.weaponName; }
    }

    public float Range
    {
        get { return weaponData.range; }
    }

    public void PlayAttack()
    {

       if (attackFx != null)
        {
            attackFx.Play();
        }
    }
}
