using UnityEngine;

namespace BeatEmUp_GameTemplate3D.Scripts.Other
{
    public class PlayerProgressHelper : MonoBehaviour
    {
        public void RemoveAllPlayerPrefsButton() => RemoveAllPlayerPrefs();
        public static void RemoveAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            SavableSettings.instance.ClearAll();
            SavableSettings.instance.Save();
        }
    }
}