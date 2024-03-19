using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    public enum SpawnState { INACTIVE, SPAWNING, ACTIVE, COUNTING };

    bool isArcade;
    Wave currentWave;
    public WaveSequence[] waveSequences;
    public Wave[] waves;
    public List<EnemySpawner> enemySpawners;
    public List<EnemySpawner> selectedSpawners;
    public int waveIndex = 0;
    public int totalWavesInDay = 0;
    private int nextWaveIndex = 1;

    public float waveCountdown;
    public float waveDuration;
    public float spawnCountdown;

    public SpawnState state = SpawnState.INACTIVE;

    public int totalEnemies;
    public int enemiesRemaining;
    public int enemiesToSpawn;
    public int spawnIndex;

    public BedStore bedStore;
    public GameObject photoSpawnPosition;
    public PhotoInteractable photoSpawned;

    bool hasPlayedWaveSound;
    private void Start()
    {
        bedStore.gameObject.SetActive(false);
        
    }

    public void SetWave(int index)
    {
        spawnIndex = 0;
        state = SpawnState.INACTIVE;

        if (waveSequences.Length <= DataManager.Instance.publicData.day - 1)
        {
            Debug.LogError("Current Day does not correspond to a sequence");
            //Alt to endless here
        }

        if (GameManager.Instance.devMode && GameManager.Instance.overrideWaveDay)
        {
            waves = waveSequences[GameManager.Instance.ovr_sequence].waves;
            index = GameManager.Instance.ovr_wave;
        }
        else
        {
            waves = waveSequences[DataManager.Instance.publicData.day-1].waves;
        }

        PlayerController.Instance.hasTakenDamageThisWave = false;
        
        if (waves.Length <= index)
        {
            Debug.LogError("WaveHandler, SetWave: Invalid index");
            waveIndex = 0;
            nextWaveIndex = waveIndex + 1;
        }
        else
        {
            waveIndex = index;
            nextWaveIndex = index + 1;
        }

        currentWave = waves[waveIndex];
        totalWavesInDay = waves.Length;
        waveCountdown = currentWave.waveCountdown;
        waveDuration = currentWave.waveDuration;
        selectedSpawners = new List<EnemySpawner>();
        List<EnemySpawner> spawnersList = new List<EnemySpawner>(enemySpawners);
        int foundSpawners = 0;
        for (int i = 0; i < 2; i++)
        {
            EnemySpawner randomSpawner = spawnersList[(int)Random.Range(0, spawnersList.Count-1)];
            
            if (!selectedSpawners.Contains(randomSpawner))
            {
                selectedSpawners.Add(randomSpawner);
                spawnersList.Remove(randomSpawner);
                foundSpawners++;
                if (foundSpawners >= 2)
                {
                    break;
                }
            }
        }

        totalEnemies = 0;
        enemiesToSpawn = 0;

        for (int i = 0; i < selectedSpawners.Count; i++)
        {
            totalEnemies += EnemiesOnSpawner(i);
        }

        enemiesRemaining += totalEnemies;
        enemiesToSpawn = totalEnemies;
     
        state = SpawnState.COUNTING;
    }

    public int EnemiesOnSpawner(int index)
    {
        switch (index)
        {
            case 0:
                return currentWave.spawner1Enemies.Count;
            case 1:
                return currentWave.spawner2Enemies.Count;
        }

        Debug.LogError("Invalid Index - Spawners cases are handled from 0 to 3");
        return 0;
    }

    public EnemyType GetSpawnerItem(int spanwerIndex, int enemyIndex)
    {
        switch (spanwerIndex)
        {
            case 0:
                return currentWave.spawner1Enemies[enemyIndex];
            case 1:
                return currentWave.spawner2Enemies[enemyIndex];
        }

        Debug.LogError("Invalid Index - Spawners cases are handled from 0 to 3");
        return EnemyType.Anger;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case SpawnState.INACTIVE:
                return;
            case SpawnState.COUNTING:
                waveCountdown -= Time.deltaTime;
                if (waveCountdown <= 0)
                {
                    if (waveIndex != 0)
                    {
                        AudioManager.Instance.Play("spawnerOpen");
                    }
                    else
                    {
                        AudioManager.Instance.Play("waveStart");
                    }
                  
                    state = SpawnState.SPAWNING;
                    SetSpawnersActive();
                    spawnCountdown = currentWave.spawnCountdown;
                }
                break;
            case SpawnState.SPAWNING:
                if (bedStore.gameObject.activeSelf)
                {
                    bedStore.StoreDisappear();
                }
                spawnCountdown -= Time.deltaTime;
                if (spawnCountdown <= 0)
                {
                   
                    for (int i = 0; i < selectedSpawners.Count; i++)
                    {
                        selectedSpawners[i].Spawn(GetSpawnerItem(i, spawnIndex));
                        enemiesToSpawn--;
                    }
                    if (enemiesToSpawn <= 0)
                    {
                        SetSpawnersDisabled();
                        state = SpawnState.ACTIVE;
                        return;
                    }
                    else
                    {
                        spawnIndex++;
                        spawnCountdown = currentWave.spawnCountdown;
                    }
                   
                }
                break;
            case SpawnState.ACTIVE:
                waveDuration -= Time.deltaTime;

                if (enemiesRemaining <= 0)
                {
                    WaveCompleted();
                }

                break;
            
        }
    }

    void SetSpawnersActive()
    {
        foreach(EnemySpawner spawner in selectedSpawners)
        {
            spawner.SetAsActive();
        }
    }

    void SetSpawnersDisabled()
    {
        foreach (EnemySpawner spawner in selectedSpawners)
        {
            spawner.SetAsDisabled();
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave completed");
        if (waves.Length <= nextWaveIndex)
        {
            SequeneceCompleted();
        }
        else
        {
            if (!bedStore.gameObject.activeSelf)
            {
                bedStore.gameObject.SetActive(true);
                bedStore.StoreAppear(PlayerController.Instance.hasTakenDamageThisWave);
            }

            waveIndex = nextWaveIndex;
            nextWaveIndex++;
            SetWave(waveIndex);
        }

       
    }

    void SequeneceCompleted()
    {
        state = SpawnState.INACTIVE;
        AudioManager.Instance.Play("win");
        GameObject newObj = Instantiate(GameManager.Instance.photoPrefab, photoSpawnPosition.transform.position, Quaternion.identity);
        photoSpawned = newObj.GetComponent<PhotoInteractable>();
    }
}
