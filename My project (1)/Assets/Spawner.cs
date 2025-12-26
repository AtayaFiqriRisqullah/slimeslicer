using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using TMPro;

public class InfiniteWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyTier
    {
        public string name;
        public GameObject enemyPrefab;
        public int minWaveToSpawn; 
    }

    public enum SpawnState { mengsummon, nunggu, ngitung };

    [Header("Settings")]
    public EnemyTier[] enemyTypes;      
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;

    [Header("Difficulty")]
    public int baseEnemies = 3;
    public float difficultyMultiplier = 1.2f; 

    [Header("Status")]
    public SpawnState state = SpawnState.ngitung;
    public int currentWave = 1;
    private float waveCountdown;
    private float searchCountdown = 1f;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public playerHPSystem playerscript;

    private void Start()
    {
        waveCountdown = timeBetweenWaves;
        UpdateWaveUI();
    }

    private void Update()
    {
        playerscript.currentWave = currentWave;
        if (state == SpawnState.nunggu)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.mengsummon)
            {
                StartCoroutine(SpawnWave());
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        state = SpawnState.ngitung;
        waveCountdown = timeBetweenWaves;
        currentWave++;
        playerscript.currentHP = 100;
        UpdateWaveUI();
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
                return false;
        }
        return true;
    }

    IEnumerator SpawnWave()
    {
        state = SpawnState.mengsummon;

        int enemyCount = Mathf.RoundToInt(baseEnemies + (currentWave * difficultyMultiplier));

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyToSpawn = GetRandomEnemyBasedOnWave();

            if (enemyToSpawn != null)
            {
                SpawnEnemy(enemyToSpawn);
            }

            yield return new WaitForSeconds(0.5f);
        }

        state = SpawnState.nunggu;
        yield break;
    }

    GameObject GetRandomEnemyBasedOnWave()
    {
        List<GameObject> validEnemies = new List<GameObject>();

        foreach (EnemyTier tier in enemyTypes)
        {
            if (currentWave >= tier.minWaveToSpawn)
            {
                validEnemies.Add(tier.enemyPrefab);
            }
        }

        if (validEnemies.Count > 0)
        {
            return validEnemies[Random.Range(0, validEnemies.Count)];
        }
        else
        {
            return enemyTypes[0].enemyPrefab;
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, sp.position, sp.rotation);
    }

    void UpdateWaveUI()
    {
        if (waveText != null) waveText.text = "Wave: " + currentWave;
    }
}