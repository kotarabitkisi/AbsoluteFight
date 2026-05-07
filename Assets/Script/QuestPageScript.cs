using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class QuestPageScript : MonoBehaviour
{
    [System.Serializable]
    public class QuestButtonElement
    {
        public GameObject obj;
        public Button btn;
        public Image rarityImage;
        public bool isActive;
    }


    public Sprite[] RarityImages;
    public QuestButtonElement[] questObjects;
    public List<QuestScriptable> allQuests;
    public List<QuestScriptable> allStoryQuests;
    public QuestScriptable chosenQuest;
    public PlayerDataScriptable playerData;


    public Image QuestImage;
    public Image QuestRank;
    public TextMeshProUGUI Title, Description;
    public Button[] charChooseButtons;
    public Image[] charImages;
    public Image charDescriptionPage;
    public TextMeshProUGUI charInfoTitle, CharInfoType, CharInfoEffect;
    public Button PutToQuestButton;
    
    public List<int> ChosenIds;
    public List<QuestVariables> chosenQuests;
    [System.Serializable]
    public class QuestVariables
    {
        public List<int> chosenChars;
        public int LeftDay;
        public QuestScriptable questScr;
    }
    public void InitializeAllCharactersOnQuestPage()
    {
        int a = 0;
        for (int i = 0; i < playerData.characters.Count && a < charImages.Length; i++)
        {
            if (!playerData.characters[i].InQuest)
            {

                charImages[a].sprite = playerData.characters[i].CharacterDefData.charIcon;
                charChooseButtons[a].onClick.RemoveAllListeners();
                charChooseButtons[a].onClick.AddListener(() => PutCharInfoToQuestPage(a));
            }
        }
    }
    public void PutQuests()
    {
        QuestScriptable.RarityEnum chosenRarity = 0;
        switch (Random.Range(0f, 200f))
        {
            case < 80:
                chosenRarity = QuestScriptable.RarityEnum.Common;
                break;
            case < 140:
                chosenRarity = QuestScriptable.RarityEnum.Uncommon;
                break;
            case < 180:
                chosenRarity = QuestScriptable.RarityEnum.Rare;
                break;
            case < 195:
                chosenRarity = QuestScriptable.RarityEnum.SuperRare;
                break;
            case <= 200:
                chosenRarity = QuestScriptable.RarityEnum.Legendary;
                break;
        }
        for (int i = 0; i < 10000; i++)
        {
            QuestScriptable _chosenquest = allQuests[Random.Range(0, allQuests.Count)];
            if (_chosenquest.Rarity == chosenRarity)
            {
                PutQuestInMap(_chosenquest);
                break;
            }
        }
    }
    public void PutQuestInMap(QuestScriptable Quest)
    {
        QuestButtonElement quest = null;
        foreach (QuestButtonElement questElement in questObjects)
        {
            if (!questElement.isActive)
            {
                questElement.isActive = true;
                break;
            }
        }
        if (quest == null) { return; }
        quest.obj.GetComponent<RectTransform>().anchoredPosition = Quest.QuestPosition;
        quest.rarityImage.sprite = RarityImages[(int)Quest.Rarity];
        quest.btn.onClick.RemoveAllListeners();
        quest.btn.onClick.AddListener(() => ToggleQuestPage(true, Quest));
    }
    public void ToggleQuestPage(bool a, QuestScriptable quest)
    {

    }
    public void PutCharToQuest(int index)
    {
        ChosenIds.Add(index);
        charImages[index].color = Color.green;
    }
    public void PutCharInfoToQuestPage(int index)
    {
        PutToQuestButton.onClick.RemoveAllListeners();
        PutToQuestButton.onClick.AddListener(() => PutCharToQuest(index));
    }
    public void StartQuest(int questId)
    {
        List<int> chosenChars = new();

        for (int i = 0; i < ChosenIds.Count; i++)
        {
            playerData.characters[ChosenIds[i]].InQuest = true;
            chosenChars.Add(ChosenIds[i]);
        }
        chosenQuests.Add(
            new QuestVariables
            {
                chosenChars = chosenChars,
                LeftDay = chosenQuests[questId].LeftDay,
                questScr = chosenQuest

            });
        if (chosenQuest.reqLevel != null)
        {
            VisualNovelManager.instance.StartGameWhenThisFinished(chosenQuest.reqLevel);
        }
    }
}
