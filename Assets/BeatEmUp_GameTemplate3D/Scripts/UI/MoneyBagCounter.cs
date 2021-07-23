using System;
using BeatEmUp_GameTemplate3D.Scripts.Player;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUp_GameTemplate3D.Scripts.UI
{
    public class MoneyBagCounter : MonoBehaviour
    {
        public string format = "{0}$";
        public Text text;
        private IDisposable subscribe;

        private void OnEnable()
        {
            text.text = String.Format(format, 0);
            subscribe = MoneyBag.collectedMoney.Subscribe(MoneyBagUpdate);
        }

        private void MoneyBagUpdate(int newMoneyCount)
        {
            text.text = String.Format(format, newMoneyCount);
        }

        private void OnDisable()
        {
            subscribe?.Dispose();
        }
    }
}