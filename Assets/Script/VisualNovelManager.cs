using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class VisualNovelManager : MonoBehaviour
{
    public GameObject Map;
    public GameObject LevelObject;
    public CameraMovement cameraMovementScript;
    public ChapterLineScriptable[] chapters;
    public GameObject[] ImageObjects;
    public Color[] charColors;

    public Image BackGround;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Text;
    public ChapterLineScriptable chosenChapter;

    public Sprite[] BackgroundSprites;
    public Dictionary<VisualNovelScriptable.BackgroundSpriteId, Sprite> BackgroundDictionary = new();

    public Sprite[] CharSprites;
    public Dictionary<VisualNovelScriptable.Positions, float> PositionDictionary = new();

    private Coroutine coroutine;
    public int DialogOrder = 0;

    private InputSystem_Actions inputActions;

    public CanvasGroup VisualNovelCanvasGroup;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        InitializePositions();
        InitializeBackgroundImages();
    }

    void OnEnable()
    {
        inputActions.Player.NextDialogue.performed += OnNextPressed;
        inputActions.Player.PreviousDialogue.performed += OnPreviousPressed;
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.NextDialogue.performed -= OnNextPressed;
        inputActions.Player.PreviousDialogue.performed -= OnPreviousPressed;
    }

    private void OnNextPressed(InputAction.CallbackContext context)
    {
        if (DialogOrder < chosenChapter.scriptables.Length - 1)
        {
            PlayAnimAfter(1);
        }
    }

    private void OnPreviousPressed(InputAction.CallbackContext context)
    {
        if (DialogOrder > 0)
        {
            PlayAnimAfter(-1);
        }
    }

    public void InitializeBackgroundImages()
    {
        var values = System.Enum.GetValues(typeof(VisualNovelScriptable.BackgroundSpriteId));
        for (int i = 0; i < BackgroundSprites.Length && i < values.Length; i++)
        {
            BackgroundDictionary[(VisualNovelScriptable.BackgroundSpriteId)values.GetValue(i)] = BackgroundSprites[i];
        }
    }

    public void InitializePositions()
    {
        float w = Screen.width;
        PositionDictionary[VisualNovelScriptable.Positions.Left_0] = -w * 0.4f;
        PositionDictionary[VisualNovelScriptable.Positions.Left_1] = -w * 0.25f;
        PositionDictionary[VisualNovelScriptable.Positions.Left_2] = -w * 0.1f;
        PositionDictionary[VisualNovelScriptable.Positions.Middle] = 0f;
        PositionDictionary[VisualNovelScriptable.Positions.Right_0] = w * 0.4f;
        PositionDictionary[VisualNovelScriptable.Positions.Right_1] = w * 0.25f;
        PositionDictionary[VisualNovelScriptable.Positions.Right_2] = w * 0.1f;
    }

    public void PlayAnimAfter(int i)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        DOTween.KillAll();

        DialogOrder += i;
        if (DialogOrder >= 0 && DialogOrder < chosenChapter.scriptables.Length)
        {
            coroutine = StartCoroutine(PlayDialogScriptable(chosenChapter.scriptables[DialogOrder]));
        }
    }

    public IEnumerator PlayDialogScriptable(VisualNovelScriptable chosenDialog)
    {
        if (chosenDialog.ChosenLevel != null) { StartGameWhenThisFinished(chosenDialog.ChosenLevel); yield return 0; }
        if (chosenDialog.chosenChar.Length == 0)
        {
            Title.text = "ERROR:THE SPEAKING CHARACTER IS NOT CHOOSED!!!";
            Title.color = Color.red;
        }
        else
        {
            if (chosenDialog.chosenChar.Length > 1)
            {
                Title.color = Color.black;
            }
            else if (chosenDialog.chosenChar.Length == 1)
            {
                Title.color = charColors[(int)chosenDialog.chosenChar[0]];
            }
            string TitleString = "";
            for (int i = 0; i < chosenDialog.chosenChar.Length; i++)
            {
                VisualNovelScriptable.SpeakingChar firstChar = chosenDialog.chosenChar[i];

                string rawName = firstChar.ToString();
                if (chosenDialog.chosenChar.Length != 1 && i != 0)
                {
                    if (i == chosenDialog.chosenChar.Length - 1)
                    {
                        TitleString += " And ";
                    }
                    else
                    {
                        TitleString += ", ";
                    }
                }
                TitleString += rawName.Contains("_") ? rawName.Split('_')[0] : rawName;
            }
            Title.text = TitleString;
        }

        Text.maxVisibleCharacters = 0;
        Text.text = chosenDialog.text;
        Text.fontSize = chosenDialog.fontSize;

        if (BackgroundDictionary.ContainsKey(chosenDialog.backgroundId))
        {
            if (BackGround.sprite != BackgroundDictionary[chosenDialog.backgroundId])
            {
                BackGround.DOFade(0, 0.3f).OnComplete(() =>
                {
                    BackGround.sprite = BackgroundDictionary[chosenDialog.backgroundId];
                    BackGround.DOFade(1, 0.3f);
                });
            }
        }

        for (int i = 0; i < ImageObjects.Length; i++)
        {
            if (chosenDialog.ObjPositions[i] != VisualNovelScriptable.Positions.NONE)
            {
                ImageObjects[i].GetComponent<RectTransform>().DOAnchorPosX(PositionDictionary[chosenDialog.ObjPositions[i]], 0.5f);
                if (PositionDictionary[chosenDialog.ObjPositions[i]] > 0)
                {
                    ImageObjects[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    ImageObjects[i].transform.rotation = Quaternion.identity;
                }
            }

            if (i < chosenDialog.speakingChars.Length)
            {
                ImageObjects[i].GetComponent<Image>().sprite = chosenDialog.speakingChars[i].sprite;
            }

            if (chosenDialog.ItisDisappearing[i] == VisualNovelScriptable.DisappearStatus.Appearing)
            {
                ImageObjects[i].GetComponent<Image>().DOFade(1, 0.5f);
            }
            else if (chosenDialog.ItisDisappearing[i] == VisualNovelScriptable.DisappearStatus.Disappearing)
            {
                ImageObjects[i].GetComponent<Image>().DOFade(0, 0.5f);
            }
        }

        int totalVisibleCharacters = chosenDialog.text.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            Text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(1f / chosenDialog.textspeed);
        }
    }

    public void ChooseChapter(int partId)
    {
        chosenChapter = chapters[partId];
        DialogOrder = 0;
    }
    public void StartGameWhenThisFinished(LevelDataScriptable Level)
    {
        print("StartGameWhenThisFinished");
        VisualNovelCanvasGroup.DOFade(0, 1.25f).OnComplete(() =>
        {
            StartCoroutine(SetupLevelCoroutine(Level));
        });
    }
    public void FinishGameAndOpenNovel(LevelDataScriptable level)
    {
        GameManager.instance.Page.transform.DOScale(Vector3.zero, 1).OnComplete(() =>
        {
            foreach (CharacterScript char_ in GameManager.instance.allCharacterScripts)
            {
                Destroy(char_.gameObject);
            }
            GameManager.instance.InitializeCharacterList();

            Destroy(Map);
            LevelObject.SetActive(false);
            chosenChapter = chapters[level.DialogId];
            DialogOrder = 0;
            VisualNovelCanvasGroup.DOFade(1, 1.25f).OnComplete(() =>
            {
                coroutine = StartCoroutine(PlayDialogScriptable(chosenChapter.scriptables[DialogOrder]));
            });


        });

    }
    public void GameOver()
    {
        print("GameOver");
    }
    IEnumerator SetupLevelCoroutine(LevelDataScriptable Level)
    {
        LevelObject.SetActive(true);
        Map = Instantiate(Level.Map, Vector3.zero, Quaternion.identity);
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance hala null! Sahne hiyerarşisini kontrol et.");
            yield break;
        }
        GridMapManager gridMapManager = GameManager.instance.gridMapManager;
        if (gridMapManager == null)
        {
            Debug.LogError("GameManager üzerinde GridMapManager scripti bulunamadı!");
            yield break;
        }
        gridMapManager.tilemap = Map.transform.GetChild(0).GetComponent<Tilemap>();
        gridMapManager.InitializeGridData();
        foreach (GridMapManager.Node tile in gridMapManager.gridData.Values)
        {
            tile.tileScript.InitializePos();
        }
        foreach (Vector2Int pos in Level.PlaceableTiles)
        {
            gridMapManager.TilesForPutting.Add(pos);
        }
        foreach (PlayerDataScriptable.CharacterData char_ in MainGameDesigner.instance.data.characters)
        {
            gridMapManager.chosenCharsToPut.Add(char_);
        }
        for (int i = 0; i < Level.Chars.Length; i++)
        {
            StartCoroutine(GameManager.instance.SpawnCharacter(Level.Chars[i].SpawningCharData, Level.Chars[i].TeamType, Level.Chars[i].SpawnTilePos));
        }
        cameraMovementScript.enabled = true;
        GameManager.instance.levelDataScriptable = Level;
        gridMapManager.HighlightTilesForPutChar();
    }
}