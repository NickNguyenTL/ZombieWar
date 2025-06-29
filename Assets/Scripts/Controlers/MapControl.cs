using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private FXSource vfxSource; // Reference to the VFX source, if needed for visual effects.
    [SerializeField]
    private ObjectsPooling enemyChaserPool;
    [SerializeField]
    private LevelModel levelModel; // Reference to the level model, if needed for level-specific data
    [SerializeField]
    private PlayerControl playerControl;
    [SerializeField]
    private List<Transform> spawnList;
    [SerializeField]
    private float spawnInterval = 2f; // Time in seconds between spawns

    private List<EnemyControl> enemiesSpawned = new List<EnemyControl>();
    private List<Matrix4x4> enemyMatrices = new List<Matrix4x4>();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (vfxSource != null)
        {
            vfxSource.Init(3);
        }

        if (enemyChaserPool == null || playerControl == null || spawnList.Count == 0)
        {
            Debug.LogError("Enemy pool, player transform or spawn list is not set!");
            return;
        }
        enemiesSpawned = new List<EnemyControl>();
        enemyChaserPool.Init();

        // Initialize the spawn timer
        levelTimer = levelModel.levelTime;

        playerControl.Init(vfxSource);
        playerControl.OnPlayerDamageTaken += CheckPlayerHealth;
    }

    private void SpawnEnemy()
    {
        GameObject enemyObject = enemyChaserPool.GetObject();
        Transform spawnPoint = spawnList[Random.Range(0, spawnList.Count)];
        enemyObject.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);
        EnemyControl newEnemy = enemyObject.GetComponentInChildren<EnemyControl>();
        newEnemy.Init(enemyChaserPool, playerControl.transform);
        enemiesSpawned.Add(newEnemy);
    }

    float spawnTimer = 0f;
    float levelTimer = 0f;
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

        if(levelTimer <= 0)
        {
           SetGameResult(true);
        }
        else
        {
            levelTimer -= Time.deltaTime;
        }
    }

    private void CheckPlayerHealth(int playerHealth)
    {
        if(playerHealth <= 0)
        {
            SetGameResult(false);
            playerControl.PlayerDeath();
        }
    }

    private void SetGameResult(bool result)
    {
        
    }
}
