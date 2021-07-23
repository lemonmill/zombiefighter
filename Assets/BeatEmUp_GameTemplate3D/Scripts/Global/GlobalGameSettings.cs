using UnityEngine;
using System.Collections.Generic;

public static class GlobalGameSettings
{
    public static int Player1CharacterID
    {
        get => _player1CharacterId;
        set
        {
            _player1CharacterId = value;
        }
    }
    public static GameObject Player1Prefab;
    public static List<LevelData> LevelData = new List<global::LevelData>();
    public static int currentLevelId = 0;

    public static bool enableContinueButton = false;
    public static bool enableBackToRewardScreenButton = false;
    public static bool shouldGiveReward     = false;
    private static int _player1CharacterId;

    public static void Reset()
    {
        LevelData = new List<global::LevelData>();
        currentLevelId = 0;
        Player1CharacterID = 0;
    }
}