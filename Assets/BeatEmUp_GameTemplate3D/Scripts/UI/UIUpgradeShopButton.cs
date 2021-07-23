using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUp_GameTemplate3D.Scripts.UI
{
    public class UIUpgradeShopButton : MonoBehaviour
    {
        private SavableSettings _save;
        
        public Text title;
        public Button buyButton;
        public Text buyButtonText;

        string _parameterName;
        public int cost;
        public SavableSettings.UpgradeType upgradeType = SavableSettings.UpgradeType.Health;

        private bool _isInfinite;

        private void Awake()
        {
            _parameterName = title.text;
        }

        private void OnEnable()
        {
            SavableSettings.CheckSaveFilePath();
            _save = SavableSettings.instance;
            _isInfinite = _save.currentCharacter.infiniteUpgrades.ContainsKey(upgradeType);
            
            UpdateVisual();
        }

        public void Buy()
        {
            if (_save.BuyWithoutSave(cost))
            {
                if (_isInfinite)
                {
                    _save.currentCharacter.infiniteUpgrades[upgradeType]++;
                }
                else
                {
                    _save.currentCharacter.boolUpgrades[upgradeType] = true;
                }
            }

            _save.Save();

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_isInfinite)
            {
                int upgradeLevel;
                _save.currentCharacter.infiniteUpgrades.TryGetValue(upgradeType, out upgradeLevel);

                if (upgradeLevel != 0)
                    title.text = _parameterName + " (" + upgradeLevel + ")";
                else
                    title.text = _parameterName;
            }
            else
            {
                bool isBuyed;
                _save.currentCharacter.boolUpgrades.TryGetValue(upgradeType, out isBuyed);

                if (isBuyed)
                {
                    buyButtonText.text = "Buyed!";
                    buyButton.interactable = false;
                }
                else
                {
                    buyButtonText.text = cost + "$";
                    buyButton.interactable = true;
                }
            }
        }
    }
}