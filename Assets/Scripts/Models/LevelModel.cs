using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapModel", menuName = "Game/MapModel", order = 3)]
public class LevelModel : ScriptableObject
{
    public string mapName;
    public float levelTime;
}
