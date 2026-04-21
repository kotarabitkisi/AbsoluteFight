using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillScriptable", menuName = "Scriptable Objects/SkillScriptable")]
public class SkillScriptable : ScriptableObject
{
    [System.Flags]
    public enum TargetType
    {
        NONE = 0,
        Ally = 1 << 0,
        Enemy = 1 << 1,
        EmptyTile = 1 << 2,
    }
    public CharacterScriptable TransformedEnemy;
    public TargetType AvailableTarget;
    public bool isTurnFinishes;
    public int cooldown = 0;
    public bool justTapToUse;
    public int useCount;
    public string skillName;
    public SkillIds skillId;
    public enum SkillIds
    {
        // Genel Yetenekler (0-9)
        Default_Melee = 0,
        Default_Ranged = 1,
        Default_DoubleAttack = 2,
        Default_AllAttack = 3,
        // Nyx (10-19)
        Nyx_AbsorbAllOfPain = 10,

        // Betty (20-29)
        Betty_Scratch = 20,
        Betty_FluffyPaw = 21,
        Betty_MakeAllCharacterCatSense = 22,
        Betty_Meow = 23,

        // Elara (30-39)
        Elara_RewardOrPunishment = 30,
        Elara_Moral = 31,
        Elara_FightingForHim = 32,
        Elara_TheSinnerMustDie = 33,

        // Tiffany (40-49)
        Tiffany_WindThrow = 40,
        Tiffany_TheWelcomingOfForest = 41,
        Tiffany_ThePowerOfNature = 42,

        // Shade (50-59)
        Shade_Push = 50,
        Shade_SwitchWithTeammate = 51,
        Shade_TheShadowLairAura = 52,

        // Samy (60-69)
        Samy_Shuriken = 60,
        Samy_DirtClone = 61,
        Samy_Revenge = 62,

        // Felix (70-79)
        Felix_Cut = 70,
        Felix_GetSpeed = 71,
        Felix_LookMyKatana = 72,

        // Cyra (80-89)
        Cyra_CpuCut = 80,
        Cyra_AIThinking = 81,
        Cyra_Reboot = 82,

        // Keeper (90-99)
        Keeper_Obey = 90,
        Keeper_Loading = 91,
        Keeper_MindCorrupt = 92,

        Syndra_Scythe = 101,
        Syndra_GetSoul = 102,
        Syndra_Execute = 103,

        Melodi_Note_Attack = 110,
        Melodi_Accent_Incident = 111,
        Melodi_Trying_Opera = 112,

        Sakura_BlinkStrike = 120,
        Sakura_PetalDance = 121,
        Sakura_SilentDeath = 122,

        Aurora_Punch = 130,
        Aurora_FrostBreath = 131,
        Aurora_TheIceDragonsWraith = 132,

        Damian_HeatSword = 140,
        Damian_HeatLazer = 141,
        Damian_OverheatBlow = 142,

        Rebecca_SpearSlash = 151,
        Rebecca_SpinningSlash = 152,
        Rebecca_BleedingDance = 153,

        Johnson_BigPunch = 160,
        Johnson_HitWithSledge = 161,
        Johnson_Balmond = 162,

        Emilia_GoAway = 170,
        Emilia_Healing = 171,
        Emilia_RevivingAura = 172,

        Slime_Blob = 180,
        Slime_Assimilate = 181,
        SlimeGirl_TentacleHit = 182,
        SlimeGirl_SlimeGlue = 183,
        SlimeGirl_BlowUp = 184,
    }
    public int range;
    public Sprite Icon;
    public float skillDamageBase;
    public float skillDamageMultiplier;
    public float skillHealMultiplier;
    public float skillHealBase;
    [Range(0, 1)]
    public float skillPenetration;
    [Range(0, 1)]
    public float skillVirusPercent;



}
