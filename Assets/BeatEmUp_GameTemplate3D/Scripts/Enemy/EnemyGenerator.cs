using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Header("Settings")]
    public EnemyTypesManager enemyTypesManager;

    public EnemyWaveSystem enemyWaveSystem;
    public Transform enemiesParent;

    [Header("Spawn Settings")]
    public Transform[] SpawnPoints;

    public Vector2 randomOffset;

    public const int pointsPerGameLevel = 3;

    public void SetWaveEnemies(int waveID)
    {
        EnemyWave currentWave = enemyWaveSystem.enemyWaves[waveID];

        const int shiftPoint = 4; // balance factor
        int currentLevelPoints = GlobalGameSettings.currentLevelId * 3 + shiftPoint + waveID;
        
        currentWave.EnemyList = GenerateEnemies(SpawnPoints[waveID].position, currentLevelPoints);
    }

    private List<GameObject> GenerateEnemies(Vector3 spawnPointPosition, int levelPoints)
    {
        List<GameObject> generatedEnemies = new List<GameObject>();

        List<int> levelList = GetLevelList(levelPoints, 10);

        foreach (int enemyLevel in levelList)
        {
            GameObject currentEnemy = enemyTypesManager.InstantiateEnemy(enemyLevel);

            //disabling by default
            currentEnemy.SetActive(false);

            //randomizing position
            currentEnemy.transform.position = spawnPointPosition + new Vector3(Random.Range(-randomOffset.x, randomOffset.x), 0, Random.Range(-randomOffset.y, randomOffset.y));

            generatedEnemies.Add(currentEnemy);
        }

        //      Returning list of generated enemyes
        return generatedEnemies;
    }

    private List<int> GetLevelList(int levelPoints, int maxCount)
    {
        List<int> levelList = new List<int>();

        //      Trying
        while ((levelList.Count == 0) || (levelList.Count > maxCount))
        {
            levelList.Clear();
            int _levelPoints = levelPoints;

            //      Filling list
            while (_levelPoints > 0)
            {
                int maxPossibleLevel = _levelPoints;
                if (maxPossibleLevel > enemyTypesManager.maxLevel)
                    maxPossibleLevel = enemyTypesManager.maxLevel;

                int enemyLevel = Random.Range(1, maxPossibleLevel + 1);

                levelList.Add(enemyLevel);

                _levelPoints -= enemyLevel;
            }
        }

        return levelList;
    }
}