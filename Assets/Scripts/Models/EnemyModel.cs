using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public enum EnemyState
    {
        Idle = 0,
        Hit = 1,
        Chase = 2,        
        Dead = 3,
    }

    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private RewardData rewardData;
    [SerializeField]
    private ChaserSystem chaserSystem;
    [SerializeField]
    private Color32 isHitTintColor;
    [SerializeField]
    private Color32 isNormalTintColor;

    [Header("Animator")]
    [SerializeField]
    private VAT_Animator vatAnimator;
    [SerializeField]
    private bool InitOnStart;
    [SerializeField]
    private EnemyState InitState;

    private EnemyState enemyState;

    private void Start()
    {
        if (InitOnStart)
        {
            Init();
        }
    }

    public void Init(Transform target = null, EnemyData newData = null)
    {
        enemyData = newData != null ? newData : enemyData;
        transform.localScale = Vector3.one * enemyData.size;

        if (chaserSystem == null)
        {
            chaserSystem = GetComponent<ChaserSystem>();
        }

        if (target != null)
        {
            chaserSystem.Init();
            chaserSystem.SetTarget(target);
            chaserSystem.OnChaseStateChanged += SetChaseStateAnim;
            chaserSystem.SetSpeed(enemyData.speed);
        }

        if (vatAnimator != null)
        {
            vatAnimator.Init();
            vatAnimator.OnAnimEnd += OnEndAnimTrigger;
        }

        SetAnimState(InitState);
    }

    #region Get Properties
    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    public RewardData GetRewardData()
    {
        return rewardData;
    }

    public ChaserSystem GetChaserSystem()
    {
        return chaserSystem;
    }
    public EnemyState GetAnimState()
    {
        return enemyState;
    }

    public VAT_Animator GetVATAnimator()
    {
        return vatAnimator;
    }
    #endregion

    public void SetChaseState(bool isChasing)
    {
        if (chaserSystem != null)
        {
            if(isChasing)
            {
                chaserSystem.StartChasing();
                SetChaseStateAnim(true);
            }
            else
            {
                chaserSystem.StopChasing();                
            }            
        }
    }

    private void SetChaseStateAnim(bool isChasing)
    {
        if (isChasing)
        {
            SetAnimState(EnemyState.Chase);
        }
        else
        {
            SetAnimState(EnemyState.Idle);
        }
    }

    public void SetAnimState(EnemyState newState)
    {
        enemyState = newState;
        vatAnimator.Play((int)enemyState);
        switch (newState)
        {
            case EnemyState.Idle:
                vatAnimator.Play((int)EnemyState.Idle, true, false);
                vatAnimator.SetTintColor(isNormalTintColor);
                vatAnimator.SetDisolve(false);
                break;
            case EnemyState.Chase:
                vatAnimator.Play((int)EnemyState.Chase, true, false);
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
            case EnemyState.Hit:
                vatAnimator.Play((int)EnemyState.Hit, false, true);
                vatAnimator.SetTintColor(isHitTintColor);
                break;
            case EnemyState.Dead:
                vatAnimator.Play((int)EnemyState.Dead, false, true);
                vatAnimator.SetDisolve(true, 1.5f);
                break;
        }

        Debug.Log(gameObject.name + " set Anim State: " + newState.ToString());
    }

    private void OnEndAnimTrigger(int animId)
    {
        switch ((EnemyState)animId)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }
    }

    public void ReturnToPool()
    {
        if (vatAnimator != null)
        {
            vatAnimator.OnAnimEnd -= OnEndAnimTrigger;
        }
        chaserSystem.SetTarget(null);
        chaserSystem.OnChaseStateChanged -= SetChaseStateAnim;
    }
}
