using UnityEditor;
using UnityEngine;

public class MissingScriptCleaner : Editor
{
    [MenuItem("Tools/Clean Missing Scripts")]
    [System.Obsolete]
    public static void Clean()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;
        foreach (GameObject go in allObjects)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removed > 0) count += removed;
        }
        Debug.Log($"{count} adet bozuk script referansı temizlendi.");
    }
}
