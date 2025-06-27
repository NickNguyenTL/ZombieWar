using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private EnemyModel enemyModel;

    private ObjectsPooling enemyChaserPool;

    public void Init(ObjectsPooling objectsPooling)
    {
        enemyModel.Init();

        enemyChaserPool = objectsPooling;
    }

    private void Destroy()
    {
        if (enemyChaserPool != null)
        {
            enemyChaserPool.ReturnObjects(enemyModel.gameObject);
        }
        else
        {
            Destroy(enemyModel.gameObject);
        }
    }
}
