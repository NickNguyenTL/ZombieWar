using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapModel", menuName = "Game/MapModel", order = 3)]
public class LevelModel : ScriptableObject
{
    public string mapName;
    public List<LevelPhase> levelPhases;
}

[Serializable]
public class LevelPhase
{
    public float phaseTime;
    public float spawnInterval;
    public List<LevelEnemyData> levelEnemyData;
}

[Serializable]
public class  LevelEnemyData
{
    public EnemyData enemyData;
    public int spawnPosID;
}
