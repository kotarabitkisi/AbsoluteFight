using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public GridMapManager gridMapManager;
    public CharacterScript characterScript;
    public TileScript WhereIsHere;
    public Vector2Int gridpos;

    void Awake()
    {
        gridMapManager=GridMapManager.instance;
        GetCellPosition();
    }
    public void GetCellPosition()
    {
        if (gridMapManager.gridData.ContainsKey(gridpos))
        {
            if (gridMapManager.gridData[gridpos].unitOnTop == characterScript)
            {
                gridMapManager.gridData[gridpos].unitOnTop = null;
            }
        }

        Vector3Int cellPos = gridMapManager.tilemap.WorldToCell(transform.position);
        gridpos = new Vector2Int(cellPos.x, cellPos.y);

        if (gridMapManager.gridData.ContainsKey(gridpos))
        {
            gridMapManager.gridData[gridpos].unitOnTop = characterScript;
        }
    }
    public List<Vector3> GetPathWorldPositions(Vector2Int targetPos, Vector2Int startPos)
    {
        List<Vector3> worldPath = new List<Vector3>();
        Vector2Int current = targetPos;
        int safetyCounter = 0;
        while (current != startPos && safetyCounter < 100)
        {
            Vector3 worldPos = gridMapManager.tilemap.GetCellCenterWorld(new Vector3Int(current.x, current.y, 0));
            worldPath.Add(worldPos);

            current = gridMapManager.gridData[current].parentPos;
            safetyCounter++;
        }

        worldPath.Reverse();
        return worldPath;
    }
    public void MoveCharacter(Vector2Int targetPos)
    {
        List<Vector3> path = GetPathWorldPositions(targetPos, gridpos);
        int lastCost = 0;
        Sequence s = DOTween.Sequence();
        foreach (Vector3 p in path)
        {
            Vector3Int cell = gridMapManager.tilemap.WorldToCell(p);
            int cost = gridMapManager.gridData[(Vector2Int)cell].MovementCost - lastCost;
            s.Append(transform.DOMove(p, 0.2f * cost).SetEase(Ease.Linear));
            lastCost = cost;
        }
        s.OnComplete(() =>
        {
            GetCellPosition();
            if (characterScript.stats.MovementSpeed == 0)
            {
                GameManager.instance.TakeTurn();
            }
            else
            {
                ControlAllReachableTiles();
            }
        });

    }
    [ContextMenu("Highlight")]

    public void ControlAllReachableTiles()
    {
        GetCellPosition();

        var reachableTiles = gridMapManager.GetReachableTiles(gridpos, characterScript.stats.MovementSpeed);

        gridMapManager.HighlightMovementTiles(reachableTiles);
    }
}
