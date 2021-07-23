using UniRx;
using UnityEngine;

namespace BeatEmUp_GameTemplate3D.Scripts.Player
{
    public static class MoneyBag
    {
        public static IReadOnlyReactiveProperty<int> collectedMoney => _collectedMoney;
        private static ReactiveProperty<int> _collectedMoney = new ReactiveProperty<int>();

        public static void AddMoney(int money)
        {
            _collectedMoney.Value += money;
        }

        public static int MoveToSaved()
        {
            var savableSettings = SavableSettings.instance; 
            
            if (savableSettings.coinsPerLevelHighscore.Value < collectedMoney.Value)
                savableSettings.coinsPerLevelHighscore.Value = collectedMoney.Value;
            
            savableSettings.AddMoney(collectedMoney.Value);
            savableSettings.Save();
            
            // Сообщить о том сколько денег собрано
            var collected = collectedMoney.Value;
            _collectedMoney.Value = 0;
            return collected;
        }
    }
}