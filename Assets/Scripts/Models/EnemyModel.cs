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
    private ChaserSystem chaserSystem;
    [SerializeField]
    private Color32 isHitTintColor;
    [SerializeField]
    private Color32 isNormalTintColor;

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
            chaserSystem.OnChaseStateChanged += OnChaseStateChange;
        }

        if (vatAnimator != null)
        {
            vatAnimator.Init();
            vatAnimator.OnAnimEnd += OnEndAnimTrigger;
        }

        SetAnimState(InitState);
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
                break;
            case EnemyState.Chase:
                vatAnimator.Play((int)EnemyState.Chase, true, false);
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
            case EnemyState.Hit:
                vatAnimator.Play((int)EnemyState.Hit, true);
                vatAnimator.SetTintColor(isHitTintColor);
                break;
            case EnemyState.Dead:
                vatAnimator.Play((int)EnemyState.Dead, true, false);
                vatAnimator.SetTintColor(isNormalTintColor);
                break;
        }

        Debug.Log("Set Anim State: " + newState.ToString());
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

    private void OnChaseStateChange(bool isChasing)
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

    private void OnDestroy()
    {
        if (vatAnimator != null)
        {
            vatAnimator.OnAnimEnd -= OnEndAnimTrigger;
        }

        if(chaserSystem != null)
        {
            chaserSystem.OnChaseStateChanged -= OnChaseStateChange;
        }
    }
}
