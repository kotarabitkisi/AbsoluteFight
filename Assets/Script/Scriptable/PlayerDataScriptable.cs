using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataScriptable", menuName = "Scriptable Objects/PlayerDataScriptable")]
public class PlayerDataScriptable : ScriptableObject
{
    public List<CharacterData> characters;
    public int HowMuchQuestFinished;
    [System.Serializable]
    public class CharacterData
    {
        public CharacterScriptable CharacterDefData;
        [Range(0,1)]
        public float Health;
        public int Level;
        public float Exp, ReqExp;
        public bool InQuest;
    }
}
