using UnityEngine;

[CreateAssetMenu(fileName = "VNCharacterSpriteScriptable", menuName = "Scriptable Objects/VNCharacterSpriteScriptable")]
public class VNCharacterSpriteScriptable : ScriptableObject
{
    public VisualNovelScriptable.SpeakingChar speaking;
    public VisualNovelScriptable.Emotions emotion;
    public Sprite sprite;
#if UNITY_EDITOR
[ContextMenu("evet")]
    private void OnValidate()
    {
        string charName = speaking.ToString();
        string path = "Assets/Sprites/CharImages/" + charName + ".png";

        Sprite foundIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (foundIcon != null)
        {
            UnityEditor.EditorGUIUtility.SetIconForObject(this, foundIcon.texture);
        }
    }

#endif
}
