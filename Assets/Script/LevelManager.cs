using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public void StartGameWhenThisFinished(LevelDataScriptable Level)
    {
        print("StartGameWhenThisFinished");
        VisualNovelManager.instance.DialogPlaying = false;
        VisualNovelManager.instance.MainGameMapCanvasGroup.interactable = false;
        VisualNovelManager.instance.MainGameMapCanvasGroup.DOFade(0, 1.25f);
        VisualNovelManager.instance.VisualNovelCanvasGroup.interactable = false;
        VisualNovelManager.instance.VisualNovelCanvasGroup.DOFade(0, 1.25f).OnComplete(() =>
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

            Destroy(VisualNovelManager.instance.Map);
            VisualNovelManager.instance.LevelObject.SetActive(false);
            VisualNovelManager.instance.chosenChapter = VisualNovelManager.instance.chapters[level.DialogId];
            VisualNovelManager.instance.DialogOrder = 0;
            VisualNovelManager.instance.VisualNovelCanvasGroup.DOFade(1, 1.25f).OnComplete(() =>
            {
                VisualNovelManager.instance.DialogPlaying = true;
                VisualNovelManager.instance.coroutine = StartCoroutine(VisualNovelManager.instance.PlayDialogScriptable(VisualNovelManager.instance.chosenChapter.scriptables[VisualNovelManager.instance.DialogOrder]));
            });


        });
    }
    public void GameOver()
    {
        print("GameOver");
    }
    IEnumerator SetupLevelCoroutine(LevelDataScriptable Level)
    {
        VisualNovelManager.instance.LevelObject.SetActive(true);
        VisualNovelManager.instance.Map = Instantiate(Level.Map, Vector3.zero, Quaternion.identity);
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
        gridMapManager.tilemap = VisualNovelManager.instance.Map.transform.GetChild(0).GetComponent<Tilemap>();
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
        VisualNovelManager.instance.cameraMovementScript.enabled = true;
        GameManager.instance.levelDataScriptable = Level;
        gridMapManager.HighlightTilesForPutChar();
    }
}
