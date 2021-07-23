using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UniRx;
using UnityEngine;

[Serializable]
public class SavableSettings
{
    #region Singletone

    public static SavableSettings instance
    {
        get
        {
            CheckSaveFilePath();
            return _instance ?? (_instance = Load());
        }
    }

    private static SavableSettings _instance;

    #endregion

    #region Money

    public int money
    {
        get { return _money; }
        private set
        {
            _money = value;
            if (moneyChanged != null)
                moneyChanged.Invoke(_money);
        }
    }
    private int _money;

    public delegate void MoneyChanged(int newValue);

    [NonSerialized]
    public MoneyChanged moneyChanged;

    public void AddMoney(int count)
    {
        _money += count;
        if (moneyChanged != null)
            moneyChanged.Invoke(_money);

        Save();
    }

    public bool BuyWithoutSave(int cost)
    {
        if (_money >= cost)
        {
            _money -= cost;

            if (moneyChanged != null)
                moneyChanged.Invoke(_money);

            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Markers

    public bool? appodealConsent = null;

    public List<string> markers = new List<string>();

    #endregion

    #region PlayerStats

    [Serializable]
    public enum UpgradeType
    {
        Health,
        PunchDamage,
        KickDamage,
        CriticalChance,

        JumpKick,
        SuperPunch,
        SuperKick,
        PunchCombo,
        KickCombo
    }

    [Serializable]
    public class CharacterUpgrades
    {
        public bool isCharacterBuyed = false;
        
        public Dictionary<UpgradeType, int> infiniteUpgrades = new Dictionary<UpgradeType, int>()
        {
            {UpgradeType.Health, 0},
            {UpgradeType.PunchDamage, 0},
            {UpgradeType.KickDamage, 0},
            {UpgradeType.CriticalChance, 0}
        };

        public Dictionary<UpgradeType, bool> boolUpgrades = new Dictionary<UpgradeType, bool>()
        {
            {UpgradeType.JumpKick, true},
            {UpgradeType.SuperPunch, true},
            {UpgradeType.SuperKick, true},
            {UpgradeType.PunchCombo, true},
            {UpgradeType.KickCombo, true}
        };
    }

    public CharacterUpgrades[] charactersUpgrades = new CharacterUpgrades[3]
    {
        new CharacterUpgrades(){isCharacterBuyed = true}, new CharacterUpgrades() {isCharacterBuyed = true}, new CharacterUpgrades()
    };
    
    public CharacterUpgrades currentCharacter
    {
        get
        {
            return charactersUpgrades[GlobalGameSettings.Player1CharacterID];
        }
    }

    #endregion

    #region Saving / Loading

    private const string SAVE_FILE_NAME = "playerStats.sav";

    public static string saveFilePath;

    public static void CheckSaveFilePath()
    {
        saveFilePath = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
    }

    public void Save()
    {
        var formatter = new BinaryFormatter();

        using (var fileStream = File.Open(saveFilePath, FileMode.OpenOrCreate))
        {
            formatter.Serialize(fileStream, this);
        }
    }

    private static SavableSettings Load()
    {
        if (string.IsNullOrEmpty(saveFilePath))
        {
            CheckSaveFilePath();
        }

        if (!File.Exists(saveFilePath))
        {
            return new SavableSettings();
        }


        var formatter = new BinaryFormatter();

        try
        {
            using (var fileStream = File.OpenRead(saveFilePath))
            {
                var deserialized = (SavableSettings) formatter.Deserialize(fileStream);
                
                FixSave(deserialized);
                
                return deserialized;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            int backUpID = 0;
            while (File.Exists(Application.persistentDataPath + "/playerStatsBackUp" + backUpID + ".sav"))
            {
                backUpID++;
            }
            
            File.Move(saveFilePath, Application.persistentDataPath + "/playerStatsBackUp" + backUpID + ".sav");
            return new SavableSettings();
        }
        finally
        {
            
        }
    }

    private static void FixSave(SavableSettings save)
    {
        if (save.coinsPerLevelHighscore == null)
        {
            Debug.LogWarning("Restoring coinsPerLevelHighscore");
            save.coinsPerLevelHighscore = new IntReactiveProperty(0);
        }
                
        save.charactersUpgrades[1].isCharacterBuyed = true;

        // ntk: reset save for buy punch
        foreach (var item in save.charactersUpgrades)
        {
            item.boolUpgrades[UpgradeType.JumpKick] = true;
            item.boolUpgrades[UpgradeType.SuperPunch] = true;
            item.boolUpgrades[UpgradeType.SuperKick] = true;
            item.boolUpgrades[UpgradeType.PunchCombo] = true;
            item.boolUpgrades[UpgradeType.KickCombo] = true;
        }

    }
    
    #endregion

    #region Leaderboard

    public IntReactiveProperty coinsPerLevelHighscore = new IntReactiveProperty(0);

    #endregion

    #region Methods

    public void ClearAll()
    {
        money = 0;
        markers = new List<string>();
        charactersUpgrades = new CharacterUpgrades[3]
        {
            new CharacterUpgrades(){isCharacterBuyed = true},
            new CharacterUpgrades(),
            new CharacterUpgrades()
        };
        
        GlobalGameSettings.Reset();
    }

    #endregion
}