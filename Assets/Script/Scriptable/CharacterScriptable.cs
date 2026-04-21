using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptable", menuName = "Scriptable Objects/CharacterScriptable")]
public class CharacterScriptable : ScriptableObject
{
        public Sprite charIcon;
        public Color VirusColor;

        public CHARNAME charName;
        public enum CHARNAME
        {
                NONE = 0,
                Nyx = 1,
                Scout = 2,
                Betty = 3,
                Betty_Transformed = 4,
                Betty_Infected = 5,
                Betty_Boss = 6,
                Tiffany = 7,
                Elara = 8,
                Shade = 9,
                Felix = 10,
                Samy = 11,
                Syndra = 12,
                Cyra = 13,
                Melodi = 14,
                Keeper = 15,
                Aurora = 16,
                Mona = 17,
                FienLing = 18,
                Sakura = 19,
                Knight = 20,
                Cyra_Infected = 21,
                Damian = 22,
                Rebecca = 23,
                Johnson = 24,
                Emilia = 25,
                DefaultEnemy_Deer = 10000,
                DefaultEnemy_Bull = 10001,
                DefaultEnemy_Wolf = 10002,
                DefaultEnemy_Slime = 10003,
                SlimeGirl = 26,
                Simon = 27,
                DefaultPicoKnight = 10004,
        }
        public float health;
        [Range(0, 1)]
        public float defense;
        public int MovementSpeed;
        public float playSpeed;
        public float damage;
        [Range(0, 1)]
        public float penetration;

        public float healForEachTurn;
        public float evadeProbability;

        public SkillScriptable[] Skills;

        [Header("Scalings")]
        public float healthScaling;
        [Range(0, 1)]
        public float defenseScaling;
        public int MovementSpeedScaling;
        public float playSpeedScaling;
        public float damageScaling;
        [Range(0, 1)]
        public float penetrationScaling;

        public float healForEachTurnScaling;
        public float evadeProbabilityScaling;


}
