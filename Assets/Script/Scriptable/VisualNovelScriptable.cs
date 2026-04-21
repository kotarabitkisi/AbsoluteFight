using UnityEngine;
[CreateAssetMenu(fileName = "VisualNovelScriptable", menuName = "Scriptable Objects/VisualNovelScriptable")]
public class VisualNovelScriptable : ScriptableObject
{
    public enum SpeakingChar
    {
        NONE = 0,
        Nyx = 1,
        Syndra = 2,
        Cyra = 3,
        Simon = 4,
        DefaultPicoKnight = 5,
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

        Bar_Entrance = 10,
        Bar_Bar = 11,

        Castle_Entrance = 20,
        Castle_Floor = 21,

        Map=22,

    }
    public BackgroundSpriteId backgroundId;
}
