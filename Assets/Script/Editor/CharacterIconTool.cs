using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterIconTool : EditorWindow
{
    [MenuItem("Tools/Kotara/Update All Character Icons")]
    public static void UpdateAllIcons()
    {
        // 1. Projedeki tüm senin scriptinin tipindeki assetleri bul (Örn: DialogueSO)
        // Buradaki "t:DialogueSO" kısmını kendi script isminle değiştir
        string[] guids = AssetDatabase.FindAssets("t:VisualNovelScriptable"); 
        int updateCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // Kendi script tipine cast et
            var script = AssetDatabase.LoadAssetAtPath<VisualNovelScriptable>(assetPath);

            if (script.chosenChar.Length >0)
            {
                string charName = script.chosenChar[0].ToString();
                string iconPath = "Assets/Sprites/CharImages/" + charName + ".png";
                
                Sprite foundIcon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);

                if (foundIcon != null)
                {
                    EditorGUIUtility.SetIconForObject(script, foundIcon.texture);
                    updateCount++;
                }
            }
        }

        // Değişiklikleri kaydet ve editörü tazele
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("İşlem Tamam", 
            $"{updateCount} adet karakter ikonu başarıyla güncellendi!", "Tamam");
    }
}
