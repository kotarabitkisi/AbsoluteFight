using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Page;
    public TextMeshProUGUI TitleText;

    public Button Btn;
    public LevelDataScriptable levelDataScriptable;
    public VisualNovelManager novelManager;

    public InputSystem_Actions inputActions;
    readonly Color[] TeamColors = new Color[]
    {
        Color.blue,
        Color.red,
        Color.green,
        Color.yellow,
    };
    [SerializeField] GameObject CharPrefab;

    [System.Serializable]
    public class SkillUIClass
    {
        public Image skillIcon;
        public Button skillButton;
        public GameObject skillObj;
        public SkillScriptable skill;
        public TextMeshProUGUI skillCooldown;
    }
    public SkillUIClass[] skills;
    public Image charImage;
    bool isMoving;
    public SkillManager skillManager;
    public GridMapManager gridMapManager;
    public static GameManager instance;
    public CharacterScript ChosenChar;
    bool gameStarted;
    public List<CharacterScript> allCharacterScripts;
    public List<CharacterScriptable> allCharacterScriptables;
    void Awake()
    {
        instance = this;
        inputActions = new InputSystem_Actions();
    }
    public void StartGame()
    {
        allCharacterScripts = FindObjectsByType<CharacterScript>(FindObjectsSortMode.None).ToList();
        gameStarted = true;
        DetectWhereIsAllCharacters();
        TakeTurn();
    }
    void DetectWhereIsAllCharacters()
    {
        foreach (CharacterScript char_ in allCharacterScripts)
        {
            Vector3Int cellPos = gridMapManager.tilemap.WorldToCell(char_.transform.position);
            Vector2Int gridpos = new(cellPos.x, cellPos.y);
            gridMapManager.gridData[gridpos].unitOnTop = char_;
            char_.characterMovement.GetCellPosition();
        }
    }
    public void TakeTurn()
    {
        CloseUI();

        InitializeCharacterList();
        ChosenChar = allCharacterScripts[0];
        foreach (CharacterScript characterScript in allCharacterScripts)
        {
            if (ChosenChar.playTurn / ChosenChar.stats.playSpeed > characterScript.playTurn / characterScript.stats.playSpeed)
            {
                ChosenChar = characterScript;
            }
        }
        float TurnMovementAmount = ChosenChar.playTurn / ChosenChar.stats.playSpeed;
        print(TurnMovementAmount);
        foreach (CharacterScript characterScript in allCharacterScripts)
        {
            characterScript.playTurn -= TurnMovementAmount * characterScript.stats.playSpeed;
            characterScript.TurnSpeedBar.DOFillAmount((100f - characterScript.playTurn) / 100f, 0.25f);
        }
        charImage.sprite = ChosenChar.CharScriptable.charIcon;
        ChosenChar.TurnSpeedBar.DOFillAmount(1, 0.25f);
        if (IsGameEnded())
        {
            return;
        }
        StartCoroutine(ChosenChar.StartTurn());
    }
    public bool IsGameEnded()
    {
        bool iswin = true;
        bool islose = true;
        for (int i = 0; i < allCharacterScripts.Count; i++)
        {
            if (allCharacterScripts[i].stats.TeamType != 0)
            {
                iswin = false;
            }
            else
            {
                islose = false;
            }
        }
        if (iswin)
        {
            Win();
        }
        else if (islose)
        {
            Lose();
        }
        return iswin || islose;
    }
    public void Win()
    {
        Page.SetActive(true);
        TitleText.text = "You Win";
        Btn.onClick.AddListener(() => novelManager.FinishGameAndOpenNovel(levelDataScriptable));
        Page.transform.DOScale(Vector3.one, 1);
    }
    public void Lose()
    {
        Page.SetActive(true);
        TitleText.text = "YouLose";
        if (levelDataScriptable.isPlayingWhenLose)
        {
            Btn.onClick.AddListener(() => novelManager.FinishGameAndOpenNovel(levelDataScriptable));
        }
        else { Btn.onClick.AddListener(() => novelManager.GameOver()); }
        Page.transform.DOScale(Vector3.one, 1);
    }
    void Update()
    {
        if (!gameStarted) { return; }

    }
    public void OpenUIOfThisCharacter(CharacterScript characterScript)
    {
        print("UI opened");
        for (int i = 0; i < characterScript.CharScriptable.Skills.Length; i++)
        {
            int index = i;
            var currentSkill = characterScript.CharScriptable.Skills[index];

            skills[index].skillObj.SetActive(true);
            skills[index].skillIcon.sprite = currentSkill.Icon;
            skills[index].skill = currentSkill;
            if (characterScript.currentCooldown[i] > 0)
            {
                skills[index].skillButton.interactable = false;
                skills[index].skillCooldown.text = characterScript.currentCooldown[i].ToString();
            }
            else
            {
                skills[index].skillButton.interactable = true;
                skills[index].skillCooldown.text = "";
                skills[index].skillButton.onClick.AddListener(() =>
                {
                    skillManager.chosenCharToUseSkill = characterScript;
                    skillManager.SelectSkillVoid(currentSkill);
                });
            }

        }
    }
    public void CloseUI()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].skillObj.SetActive(false);
            skills[i].skillIcon.sprite = null;
            skills[i].skill = null;
            skills[i].skillButton.onClick.RemoveAllListeners();
        }
    }
    public void HandleMovementClick(InputAction.CallbackContext context)
    {
        print("clicked");
        print(context.phase);
        if (isMoving || context.phase != InputActionPhase.Performed) return;
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        print("raycast");
        Vector2 mousePos2D = new(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {

            TileScript targetTile = hit.collider.GetComponent<TileScript>();
            print(targetTile.gridPos);
            if (targetTile != null)
            {
                if (gridMapManager.moveHighlightedGrids.ContainsKey(targetTile.gridPos))
                {
                    ChosenChar.stats.MovementSpeed -= gridMapManager.gridData[targetTile.gridPos].MovementCost;
                    gridMapManager.gridData[ChosenChar.characterMovement.gridpos].unitOnTop = null;
                    gridMapManager.gridData[targetTile.gridPos].unitOnTop = ChosenChar;
                    ChosenChar.characterMovement.MoveCharacter(targetTile.gridPos);
                }
                else if (gridMapManager.attackHighlightedGrids.ContainsKey(targetTile.gridPos))
                {
                    skillManager.SelectUsableChar(targetTile.gridPos);
                }
                else if (gridMapManager.TilesForPutting.Contains(targetTile.gridPos))
                {
                    StartCoroutine(gridMapManager.ChooseTileForPutCharacter(targetTile.gridPos));
                }
            }

        }
    }
    void OnEnable()
    {
        // C# Wrapper üzerinden bağlama (En güvenli yol budur)
        inputActions.Player.WalkButton.performed += HandleMovementClick;
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.WalkButton.performed -= HandleMovementClick;
        inputActions.Player.Disable();
    }
    public void InitializeCharacterList()
    {
        allCharacterScripts.RemoveAll(char_ => char_ == null || char_.gameObject == null);
    }
    [ContextMenu("RenameAllCharacters")]
    public void RenameAllCharacterNames()
    {
        allCharacterScripts = FindObjectsByType<CharacterScript>(FindObjectsSortMode.None).ToList();
        foreach (CharacterScript char_ in allCharacterScripts)
        {
            InitializeCharacter(char_);
        }
    }
    public IEnumerator SpawnCharacter(PlayerDataScriptable.CharacterData charScr, int Team, Vector2Int gridPos)
    {
        GameObject SpawnedChar = Instantiate(
            CharPrefab,
         gridMapManager.tilemap.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0))
            , quaternion.identity);
        CharacterScript _script = SpawnedChar.GetComponent<CharacterScript>();
        _script.CharScriptable = charScr.CharacterDefData;
        _script.stats.TeamType = Team;
        _script.stats.level = charScr.Level;
        InitializeCharacter(_script);
        yield return _script.spriteRenderer.DOColor(Color.white, 1).OnComplete(RenameAllCharacterNames).WaitForCompletion();
    }
    public void InitializeCharacter(CharacterScript char_)
    {
        char_.defStatsAfterLevelThings.health = (1 + (char_.stats.level - 1) * char_.CharScriptable.healthScaling) * char_.CharScriptable.health;
        char_.defStatsAfterLevelThings.maxHealth = (1 + (char_.stats.level - 1) * char_.CharScriptable.healthScaling) * char_.CharScriptable.health;
        char_.characterMovement.GetCellPosition();
        char_.defStatsAfterLevelThings.defense = (1 + (char_.stats.level - 1) * char_.CharScriptable.defenseScaling) * char_.CharScriptable.defense;
        char_.defStatsAfterLevelThings.MovementSpeed = char_.CharScriptable.MovementSpeed;
        char_.defStatsAfterLevelThings.playSpeed = (1 + (char_.stats.level - 1) * char_.CharScriptable.playSpeedScaling) * char_.CharScriptable.playSpeed;
        char_.defStatsAfterLevelThings.damage = (1 + (char_.stats.level - 1) * char_.CharScriptable.damageScaling) * char_.CharScriptable.damage;
        char_.defStatsAfterLevelThings.penetration = (1 + (char_.stats.level - 1) * char_.CharScriptable.penetrationScaling) * char_.CharScriptable.penetration;
        char_.defStatsAfterLevelThings.virusDamage = (1 + (char_.stats.level - 1) * char_.CharScriptable.damageScaling) * char_.CharScriptable.damage;
        char_.defStatsAfterLevelThings.healForEachTurn = (1 + (char_.stats.level - 1) * char_.CharScriptable.healForEachTurnScaling) * char_.CharScriptable.healForEachTurn;
        char_.defStatsAfterLevelThings.evadeProbability = (1 + (char_.stats.level - 1) * char_.CharScriptable.evadeProbabilityScaling) * char_.CharScriptable.evadeProbability;
        char_.spriteRenderer.sprite = char_.CharScriptable.charIcon;





        char_.TeamColor.color = new Color(
        TeamColors[char_.stats.TeamType].r,
        TeamColors[char_.stats.TeamType].g,
        TeamColors[char_.stats.TeamType].b,
        0.01f);



        char_.gameObject.name = char_.CharScriptable.charName.ToString();
        char_.stats.health = char_.defStatsAfterLevelThings.health;
        char_.stats.maxHealth = char_.defStatsAfterLevelThings.health;
        char_.characterMovement.GetCellPosition();
        char_.stats.defense = char_.defStatsAfterLevelThings.defense;
        char_.stats.MovementSpeed = char_.defStatsAfterLevelThings.MovementSpeed;
        char_.stats.playSpeed = char_.defStatsAfterLevelThings.playSpeed;
        char_.stats.damage = char_.defStatsAfterLevelThings.damage;
        char_.stats.penetration = char_.defStatsAfterLevelThings.penetration;
        char_.stats.virusDamage = char_.defStatsAfterLevelThings.damage;
        char_.stats.healForEachTurn = char_.defStatsAfterLevelThings.healForEachTurn;
        char_.stats.evadeProbability = char_.defStatsAfterLevelThings.evadeProbability;
        char_.spriteRenderer.sprite = char_.CharScriptable.charIcon;
    }

}
