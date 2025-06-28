using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public int damage;
    public float speed;
    public float exp;
}

public class EnemyGameData
{
    public string enemyName;
    public int baseHealth;
    public int curHealth;
    public int baseDamage;
    public int curDamage;
    public float baseSpeed;
    public float curSpeed;
    public float maxExp;
    public float curExp;

    public EnemyGameData(EnemyGameData data)
    {
        enemyName = data.enemyName;
        baseHealth = data.baseHealth;
        baseDamage = data.baseDamage;
        baseSpeed = data.baseSpeed;
        maxExp = data.maxExp;

        curHealth = this.baseHealth;
        curSpeed = this.baseSpeed;
        curExp = 0f;
    }

    public EnemyGameData(EnemyData data)
    {
        enemyName = data.enemyName;
        baseHealth = data.health;
        baseDamage = data.damage;
        baseSpeed = data.speed;
        maxExp = data.exp;

        curHealth = this.baseHealth;
        curSpeed = this.baseSpeed;
        curExp = 0f;
    }
}
