using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvironmentLoader : MonoBehaviour
{
    public GameObject[] envPresets;

    private void Awake()
    {
        LoadEnvironment(GlobalGameSettings.currentLevelId);
    }

    public void LoadEnvironment(int levelId)
    {
        while (levelId >= envPresets.Length)
        {
            levelId -= envPresets.Length;
        }
        Debug.Log("Loading " + (levelId+1) + " envPreset");
        Instantiate<GameObject>(envPresets[levelId]);
    }
}
