using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUp_GameTemplate3D.Scripts.UI
{
    public class MoneyCounter : MonoBehaviour
    {
        private Text _counterText;

        private void OnEnable()
        {
            _counterText = GetComponent<Text>();
            
            SavableSettings.instance.moneyChanged += UpdateCounter;
            
            UpdateCounter(SavableSettings.instance.money);
        }

        private void OnDisable()
        {
            SavableSettings.instance.moneyChanged -= UpdateCounter;
        }

        private void UpdateCounter(int value)
        {
            _counterText.text = value + " $";
        }
    }
}