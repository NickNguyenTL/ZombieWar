using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public float speed;
    public float exp;
}

public class EnemyGameData
{
    public string enemyName;
    public int baseHealth;
    public int curHealth;
    public float baseSpeed;
    public float curSpeed;
    public float maxExp;
    public float curExp;

    public EnemyGameData(string name, int health, float speed, float exp)
    {
        enemyName = name;
        baseHealth = health;
        baseSpeed = speed;
        maxExp = exp;

        curHealth = health;
        curSpeed = speed;
        curExp = 0f;
    }

    public EnemyGameData(EnemyGameData data)
    {
        enemyName = data.enemyName;
        baseHealth = data.baseHealth;
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
        baseSpeed = data.speed;
        maxExp = data.exp;

        curHealth = this.baseHealth;
        curSpeed = this.baseSpeed;
        curExp = 0f;
    }
}
