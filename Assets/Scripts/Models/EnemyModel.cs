using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public enum EnemyState
    {
        Idle = 0,
        Chase = 1,
        Hit = 2,
        Dead = 3,
    }

    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private ChaserSystem chaserSystem;
    [SerializeField]
    private Color32 isHitTintColor;
    [SerializeField]
    private Color32 isNormalTintColor;

    [SerializeField]
    private VAT_Animator vatAnimator;

    private EnemyState enemyState;

    public void Init(Transform target = null)
    {
        if (chaserSystem == null)
        {
            chaserSystem = GetComponent<ChaserSystem>();
        }

        if (target != null)
        {
            chaserSystem.Init();
            chaserSystem.SetTarget(target);
        }

        if (vatAnimator != null)
        {
            vatAnimator.Init();
            vatAnimator.OnAnimEnd += OnEndAnimTrigger;
        }

        SetAnimState(EnemyState.Idle);
    }

    public void SetAnimState(EnemyState newState)
    {
        enemyState = newState;
        vatAnimator.Play((int)enemyState);
        switch (newState)
        {
            case EnemyState.Idle:
                vatAnimator.Play((int)EnemyState.Idle);
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
            case EnemyState.Chase:
                vatAnimator.Play((int)EnemyState.Chase);
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
            case EnemyState.Hit:
                vatAnimator.Play((int)EnemyState.Hit);
                vatAnimator.SetTintColor(isHitTintColor);
                break;
            case EnemyState.Dead:
                vatAnimator.Play((int)EnemyState.Dead);
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
        }
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
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
            case EnemyState.Dead:
                break;
        }
    }

    private void OnDestroy()
    {
        if (vatAnimator != null)
        {
            vatAnimator.OnAnimEnd -= OnEndAnimTrigger;
        }
    }
}
