using System.Collections;
using System.Collections.Generic;
using BeatEmUp_GameTemplate3D.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UILevelRewarding : MonoBehaviour
{
    [FormerlySerializedAs("textUI")] 
    public Text rewardTextUi;

    [Header("Sections")] 
    public GameObject section1;
    public GameObject section2;
    
    private void OnEnable()
    {
        if (GlobalGameSettings.shouldGiveReward)
        {
            GlobalGameSettings.shouldGiveReward = false;
            
            //var reward = (GlobalGameSettings.currentLevelId * 3 + 1) * 3 + 3;
            var reward = MoneyBag.MoveToSaved();
            
            rewardTextUi.text = "Reward: " + reward + "$";
            
            
            section1.SetActive(true);
            section2.SetActive(false);
        }
    }
}
