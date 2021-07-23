using System;
using UnityEngine;

namespace BeatEmUp_GameTemplate3D.Scripts.Level
{
    public class UIUpgradeShop : UISceneLoader
    {
        public GameObject continueButton;
        [Space]
        public GameObject normalBackButton;
        public GameObject backToLevelRewardButton;
        
        public void StartLastLevel()
        {
            GlobalGameSettings.currentLevelId = PlayerPrefs.GetInt("LastOpenedLevel", 1) - 1;
            LoadScene("01_Game");
        }

        public void EnableContinueButton() => 
            GlobalGameSettings.enableContinueButton = true;

        public void EnableBackToRewardScreenButton() =>
            GlobalGameSettings.enableBackToRewardScreenButton = true;
        
        private void OnEnable()
        {
            continueButton
                .SetActive(GlobalGameSettings.enableContinueButton);
            GlobalGameSettings.enableContinueButton = false;    

            normalBackButton
                .SetActive(!GlobalGameSettings.enableBackToRewardScreenButton);
            backToLevelRewardButton
                .SetActive(GlobalGameSettings.enableBackToRewardScreenButton);
            GlobalGameSettings.enableBackToRewardScreenButton = false;
        }
    }
}