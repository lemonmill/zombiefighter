using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ItemsManager", menuName = "Create ItemsManager", order = -1)]
public class ItemsManager : ScriptableObject
{
    public ItemWithChance[] items;

    private float chanceSum
    {
        get
        {
            float result = 0;

            foreach (var item in items)
            {
                result += item.chance;
            }

            return result;
        }
    }

    public GameObject GetRandomItemPrefab()
    {
        float randomValue = Random.Range(0, chanceSum);

        foreach (var item in items)
        {
            randomValue -= item.chance;
            if (randomValue <= 0) return item.itemPrefab;
        }
        
        throw new Exception("Programmer is stupid");
    }
}

[Serializable]
public struct ItemWithChance
{
    public float chance;
    public GameObject itemPrefab;
}