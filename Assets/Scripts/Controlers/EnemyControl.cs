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
        enemyModel.SetChaseState(false);

        yield return null; // Small delay before death animation
        enemyModel.SetAnimState(EnemyState.Dead);
        col.enabled = false;        

        float deathTime = enemyModel.GetVATAnimator().GetAnimationTime((int)EnemyState.Dead);
        yield return new WaitForSeconds(deathTime);

        Destroy();
    }

    private IEnumerator SetHitState()
    {
        enemyModel.SetChaseState(false);
        yield return null;

        enemyModel.SetAnimState(EnemyState.Hit);        
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
