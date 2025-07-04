using System.Collections;
using System.Collections.Generic;
using Terresquall;
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

    [Header("Other Controler")]
    [SerializeField]
    private Map01_View mapView;

    private List<EnemyControl> enemiesSpawned = new List<EnemyControl>();

    int curPhaseId;
    bool endGame;
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

        mapView.Init();
        mapView.OnChangeWeapon = ChangePlayerWeapon;
        mapView.OnReturnToMenu = LoadMainMenuScene;

        if (enemyChaserPool == null || playerControl == null || spawnList.Count == 0)
        {
            Debug.LogError("Enemy pool, player transform or spawn list is not set!");
            return;
        }
        enemiesSpawned = new List<EnemyControl>();
        enemyChaserPool.Init();

        playerControl.Init(vfxSource);
        playerControl.OnPlayerDamageTaken += CheckPlayerHealth;

        ChangePlayerWeapon();

        curPhaseId = 0;
        levelTimer = levelModel.levelPhases[curPhaseId].phaseTime;
        endGame = false;        
    }

    private void SpawnEnemy(EnemyData enemyData, int spawnPosId)
    {
        GameObject enemyObject = enemyChaserPool.GetObject();
        Transform spawnPoint = spawnList[spawnPosId % (spawnList.Count)];
        enemyObject.transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);
        EnemyControl newEnemy = enemyObject.GetComponentInChildren<EnemyControl>();
        newEnemy.Init(enemyChaserPool, enemyData, playerControl.transform);
        enemiesSpawned.Add(newEnemy);
    }

    float spawnTimer = 0f;
    float levelTimer = 0f;
    // Update is called once per frame
    void Update()
    {
        if(!endGame)
        {
            playerControl.PlayerMove(mapView.GetJoystickInput());

            if (spawnTimer <= 0)
            {
                var curPhase = levelModel.levelPhases[curPhaseId];
                for (int i = 0; i < curPhase.levelEnemyData.Count; i++)
                {
                    SpawnEnemy(curPhase.levelEnemyData[i].enemyData, curPhase.levelEnemyData[i].spawnPosID);
                }

                spawnTimer = curPhase.spawnInterval;
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }

            if (curPhaseId >= levelModel.levelPhases.Count)
            {
                SetGameResult(true);
            }
            else
            {
                if (levelTimer > 0)
                {
                    levelTimer -= Time.deltaTime;
                }
                else
                {
                    curPhaseId++;
                    Debug.Log("New phase: " + curPhaseId);
                    levelTimer = levelModel.levelPhases[curPhaseId].phaseTime;
                }
            }
        }        
    }

    private void CheckPlayerHealth(int playerHealth)
    {
        if(!endGame)
        {
            Debug.Log("Cur Player Health " + playerHealth);
            mapView.SetPlayerHealth(playerHealth);
            if (playerHealth <= 0)
            {
                SetGameResult(false);
                playerControl.PlayerDeath();
            }
        }        
    }

    private void SetGameResult(bool result)
    {
        endGame = true;
        mapView.ShowGameOver(result);
        Debug.Log("Game Over! " + result);
    }

    private void LoadMainMenuScene()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void ChangePlayerWeapon()
    {
        string weaponName = playerControl.NextWeapon();
        mapView.SetChangeWeaponText(weaponName);
    }
}
