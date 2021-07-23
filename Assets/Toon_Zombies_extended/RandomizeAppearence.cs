#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RandomizeAppearence : MonoBehaviour
{
    public bool randomizeOnStart = true;
    public bool destroyAfterRandomize = false;
    public GameObjectArray[] groups;

    private void Start()
    {
        if (randomizeOnStart)
        {
            Randomize();
        }
    }

    public void Randomize()
    {
        foreach (var group in groups)
        {
            // Selecting one favorite
            int selected = Random.Range(0, group.gameObjects.Length);

            // Enabling selected and disabling/destroying all other
            for (int i = 0; i < group.gameObjects.Length; i++)
            {
                if (i == selected)
                    group.gameObjects[i].SetActive(true);
                else if (!destroyAfterRandomize)
                    group.gameObjects[i].SetActive(false);
                else
                    Destroy(group.gameObjects[i]);
            }
        }

        if (destroyAfterRandomize)
            Destroy(this);
    }
}

[System.Serializable]
public struct GameObjectArray
{
    public GameObject[] gameObjects;
}

#if UNITY_EDITOR
[CustomEditor(typeof(RandomizeAppearence))]
public class RandomizeAppearenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (RandomizeAppearence)target;
        if (GUILayout.Button("Randomize"))
        {
            myScript.Randomize();
        }
    }
}
#endif