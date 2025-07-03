using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyModel;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private EnemyModel enemyModel;
    [SerializeField]
    private Collider col;
    [SerializeField]
    private bool InitOnStart;

    private ObjectsPooling enemyChaserPool;
    private EnemyGameData curData;

    private void Start()
    {
        if (InitOnStart)
        {
            Init(null, null, null);
        }
    }

    public void Init(ObjectsPooling objectsPooling, EnemyData newData, Transform target = null)
    {
        enemyModel.Init(target, newData);

        curData = new EnemyGameData(enemyModel.GetEnemyData());
        enemyChaserPool = objectsPooling;

        col.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        if (curData != null)
        {
            if (curData.curHealth > 0)
            {
                curData.curHealth -= damage;
                col.enabled = false;
                if (curData.curHealth <= 0)
                {
                    StartCoroutine(SetDeath());
                }
                else
                {
                    StartCoroutine(SetHitState());
                }
            }
        }
    }

    private IEnumerator SetDeath()
    {
        enemyModel.SetChaseState(false, EnemyState.Dead);
        col.enabled = false;
        float deathTime = enemyModel.GetVATAnimator().GetAnimationTime((int)EnemyState.Dead);
        yield return new WaitForSeconds(deathTime);

        Destroy();
    }

    private IEnumerator SetHitState()
    {
        enemyModel.SetChaseState(false, EnemyState.Hit);
        float hitTime = enemyModel.GetVATAnimator().GetAnimationTime((int)EnemyState.Hit);
        
        yield return new WaitForSeconds(hitTime);

        col.enabled = true;
        enemyModel.SetChaseState(true, EnemyState.Idle);
    }

    public EnemyGameData GetEnemyData()
    {
        return curData;
    }

    private void Destroy()
    {
        if (enemyChaserPool != null)
        {
            enemyModel.ReturnToPool();
            enemyChaserPool.ReturnObjects(enemyModel.gameObject);
        }
        else
        {
            Destroy(enemyModel.gameObject);
        }
    }
}
