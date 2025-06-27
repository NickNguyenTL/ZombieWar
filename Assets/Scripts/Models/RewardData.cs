using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "Game/RewardData", order = 2)]
public class RewardData : ScriptableObject
{
    public enum RewardType
    {
        None = 0,
        Health = 1,
        Score = 2,

        Weapon_1 = 10,
        Weapon_2 = 11,
        Weapon_3 = 12,//Bomb
    }

    [Serializable]
    public class Reward
    {
        public RewardType Type;
        public int Value;
        public int Rate;
        public Reward(RewardType type, int value, int rate)
        {
            Type = type;
            Value = value;
            Rate = rate;
        }
    }

    public List<Reward> rewards;
}
