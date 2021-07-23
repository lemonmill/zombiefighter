using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Client.Editor
{
    public class GameWindow : EditorWindow
    {
        [MenuItem("Window/Client/Game")]
        static public void OpenWindow()
        {
            GetWindow<GameWindow>();
        }
        
        void OnGUI()
        {
            GUILayout.Label($"current level {GlobalGameSettings.currentLevelId}");
        }
    }
}