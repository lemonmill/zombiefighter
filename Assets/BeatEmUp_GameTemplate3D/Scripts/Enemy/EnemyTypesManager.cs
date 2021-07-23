using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTypesManager", menuName = "Custom/EnemyTypesManager", order = -1)]
public class EnemyTypesManager : ScriptableObject
{
    [HideInInspector] public List<GameObject> enemyTypes;

    [Header("Level Scaling")]
    public float hpModifier = .05f;

    public float attackModifier = .05f;
    public float attackSpeedModifier = .05f;
    public float walkSpeedModifier = .05f;
    public float minEnemySize = .9f, maxEnemySize = 1.3f;
    public int maxLevel
    {
        get { return enemyTypes.Count; }
    }

    /// <summary>
    /// Instantiates enemy, scaling it according to level and sizeSettings
    /// </summary>
    /// <param name="level">Level of enemy (>0)</param>
    /// <returns></returns>
    public GameObject InstantiateEnemy(int level)
    {
        //Instantiating
        var enemy = Instantiate<GameObject>(GetEnemy(level));

        enemy.GetComponent<EnemyLevelScaler>().ScaleLevel(level);

        return enemy;
    }

    private GameObject GetEnemy(int level)
    {
        if (enemyTypes.Count < level) return null;
        return enemyTypes[level - 1];
    }

    public float GetSizeValue(int level)
    {
        return Mathf.LerpUnclamped(minEnemySize, maxEnemySize, Mathf.InverseLerp(0, maxLevel, level - 1));
    }
}