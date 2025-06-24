using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private ChaserSystem enemyChaserPrefab;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private List<Transform> spawnList;
    [SerializeField]
    private float spawnInterval = 2f; // Time in seconds between spawns

    private List<ChaserSystem> enemiesSpawned = new List<ChaserSystem>();
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (enemiesSpawned != null)
        {
            foreach (var enemy in enemiesSpawned)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
        }
        enemiesSpawned = new List<ChaserSystem>();
    }

    private void SpawnEnemy()
    {
        if (enemyChaserPrefab == null || playerTransform == null || spawnList.Count == 0)
        {
            Debug.LogError("Enemy prefab, player transform or spawn list is not set!");
            return;
        }
        Transform spawnPoint = spawnList[Random.Range(0, spawnList.Count)];
        ChaserSystem newEnemy = Instantiate(enemyChaserPrefab, spawnPoint.position, Quaternion.identity);
        newEnemy.Init();
        newEnemy.SetTarget(playerTransform);
        enemiesSpawned.Add(newEnemy);
    }

    float spawnTimer = 0f;
    // Update is called once per frame
    void Update()
    {
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }
}
