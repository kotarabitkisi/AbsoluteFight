using UnityEngine;

[CreateAssetMenu(fileName = "VNCharacterSpriteScriptable", menuName = "Scriptable Objects/VNCharacterSpriteScriptable")]
public class VNCharacterSpriteScriptable : ScriptableObject
{
    public VisualNovelScriptable.SpeakingChar speaking;
    public VisualNovelScriptable.Emotions emotion;
    public Sprite sprite;
#if UNITY_EDITOR
    // Inspector'da bir değeri değiştirdiğinde bu fonksiyon tetiklenir
    private void OnValidate()
    {
        string newName = $"{speaking} - {emotion}";

        if (name != newName && !string.IsNullOrEmpty(newName))
        {
            UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), newName);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
#endif
}
