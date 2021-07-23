#if UNITY_EDITOR

using UnityEditor;
// using Rotorz.ReorderableList;


[CustomEditor(typeof(EnemyTypesManager))]
public class EnemyTypesManagerEditor : Editor
{
    private SerializedProperty _enemyTypesProperty;

    public void OnEnable()
    {
        _enemyTypesProperty = serializedObject.FindProperty("enemyTypes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var myScript = (EnemyTypesManager)target;

        DrawDefaultInspector();

        // ReorderableListGUI.Title("Enemy Types");
        // ReorderableListGUI.ListField(_enemyTypesProperty);

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
