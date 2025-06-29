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

    private ObjectsPooling enemyChaserPool;
    private EnemyGameData curData;

    public void Init(ObjectsPooling objectsPooling, Transform target = null)
    {
        enemyModel.Init(target);

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
        enemyModel.SetAnimState(EnemyState.Dead);
        col.enabled = false;
        enemyModel.SetChaseState(false);

        float deathTime = enemyModel.GetVATAnimator().GetAnimationTime((int)EnemyState.Dead);
        yield return new WaitForSeconds(deathTime);

        Destroy();
    }

    private IEnumerator SetHitState()
    {
        enemyModel.SetAnimState(EnemyState.Hit);
        enemyModel.SetChaseState(false);
        float hitTime = enemyModel.GetVATAnimator().GetAnimationTime((int)EnemyState.Hit);

        yield return new WaitForSeconds(hitTime);

        enemyModel.SetAnimState(EnemyState.Idle);
        col.enabled = true;
        enemyModel.SetChaseState(true);
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
