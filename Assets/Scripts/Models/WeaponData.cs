using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponModel", menuName = "ScriptableObjects/WeaponModel", order = 4)]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float attackSpeed; // Time in seconds between attacks
    public float range; // Range of the weapon
}
