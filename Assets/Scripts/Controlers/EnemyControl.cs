using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        enemyModel.Init();
        enemyModel.onHitEnd += SetHitState;
        enemyModel.onDeathEnd += Destroy;

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
                enemyModel.SetAnimState(EnemyModel.EnemyState.Hit);
                col.enabled = false;
                enemyModel.SetChaseState(false);

                if (curData.curHealth <= 0)
                {
                    SetDeath();
                }
            }
        }
    }

    private void SetDeath()
    {
        enemyModel.SetAnimState(EnemyModel.EnemyState.Dead);
        col.enabled = false;
        enemyModel.SetChaseState(false);
    }

    private void SetHitState()
    {
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
