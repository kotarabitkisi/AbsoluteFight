using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestScriptable", menuName = "Scriptable Objects/QuestScriptable")]
public class QuestScriptable : ScriptableObject
{
    public Vector2 QuestPosition;
    public LevelDataScriptable reqLevel;
    public float questTime;//it depends on day
    public int placableCharCount;
    public int reqPower;
    public Reward reward;
    public string title;
    public string description;
    public Image QuestImage;
    public RarityEnum Rarity;
    public enum RarityEnum
    {
        Common,
        Uncommon,
        Rare,
        SuperRare,
        Legendary,
    }
    public class Reward
    {
        public float exp;
        public float charExp;
        public float money;
        public PlayerDataScriptable newCharData;
    }
}
