
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
public class DebugScript : MonoBehaviour
{
    public int addMoneyOnStart = 0;
    public static bool moneyAdded = false;

    public int setLastOpenedLevelOnStart = 0;

    private void Start()
    {
        if (!moneyAdded)
        {
            SavableSettings.instance.AddMoney(addMoneyOnStart);
            moneyAdded = true;
        }

        if (setLastOpenedLevelOnStart > 0)
        {
            PlayerPrefs.SetInt("LastOpenedLevel", setLastOpenedLevelOnStart);
            PlayerPrefs.Save();
        }
    }

    public void DeleteSaveFile()
    {
        SavableSettings.CheckSaveFilePath();
        if (!string.IsNullOrEmpty(SavableSettings.saveFilePath))
            File.Delete(SavableSettings.saveFilePath);
    }
}

#if UNITY_EDITOR
[CustomEditor(inspectedType: typeof(DebugScript))]
public class DebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (DebugScript) target;
        if (GUILayout.Button("Delete Save File"))
            myScript.DeleteSaveFile();
    }
}
#endif