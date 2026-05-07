using System;
using UnityEngine;
[CreateAssetMenu(fileName = "VisualNovelScriptable", menuName = "Scriptable Objects/VisualNovelScriptable")]
public class VisualNovelScriptable : ScriptableObject
{
    public UIPageName _UIPageName;
    public enum UIPageName
    {
        NONE = 0,
        RankExplanation = 1,
    }
    public enum SpeakingChar
    {
        NONE = 0,
        Nyx = 1,
        Syndra = 2,
        Cyra = 3,
        Simon = 4,
        DefaultPicoKnight = 5,
        PicoAdvisor = 6,
        Emilia = 7,
        Criminal = 8,
        Criminal2 = 9,
        SomeGirl = 10,
        Rosa = 11,
        Cameran = 12,
        Valerius = 13,
    }
    public enum Emotions
    {
        NONE = -1,
        Normal = 0,
        Happy = 1,
        Sad = 2,
        Angry = 3,
        Shocked = 4,
        Threatning = 5,
        Blushed = 6,
        Smug = 7,
        Thinking = 8,
        Exhausted = 9,
        Excited = 10,
        Disappointed = 11,
    }
    public LevelDataScriptable ChosenLevel;
    public float textspeed;
    public int fontSize;
    [TextArea(3, 10)]
    public string text;
    public VNCharacterSpriteScriptable[] speakingChars;
    public SpeakingChar[] chosenChar;
    public DisappearStatus[] ItisDisappearing;
    public enum DisappearStatus
    {
        NothingHappening,
        Appearing,
        Disappearing,
    }
    public Positions[] ObjPositions;
    public enum Positions
    {
        NONE = 0,
        Middle = 1,
        Left_0 = 2,
        Left_1 = 3,
        Left_2 = 4,
        Right_0 = 5,
        Right_1 = 6,
        Right_2 = 7,

    }
    public enum BackgroundSpriteId
    {
        PicoCity_Entrance = 0,
        PicoCity_Center = 1,
        PicoCity_WearShop = 2,
        PicoCity_Night = 3,
        PicoCity_ReallyNight = 4,

        Bar_Entrance = 10,
        Bar_Bar = 11,

        Castle_Entrance = 20,
        Castle_Floor = 21,
        

        Map = 22,

        Guild_News = 23,
        Guild_Contracts = 24,
        Guild_ContractsSlimed = 25,

        Guild_Entrance = 26,
        Guild_Night = 27,
        Guild_NightDaginik = 28,
        Guild_JokeQuest = 29,
        Guild_CyraLab = 30,
        Castle_ThreathingLookEmilia = 31,

    }
    public BackgroundSpriteId backgroundId;
}
