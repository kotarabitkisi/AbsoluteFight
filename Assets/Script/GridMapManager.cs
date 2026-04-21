using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMapManager : MonoBehaviour
{
    public SkillManager skillManager;
    public Tilemap tilemap;
    public static GridMapManager instance;
    void Awake()
    {
        instance = this;
    }

    public class Node
    {
        public CharacterScript unitOnTop;
        public TileScript tileScript;
        public int MovementCost;
        public Vector2Int parentPos;
    }
    public Dictionary<Vector2Int, Node> attackHighlightedGrids = new();
    public Dictionary<Vector2Int, Node> moveHighlightedGrids = new();
    public Dictionary<Vector2Int, Node> gridData = new();
    [ContextMenu("GetReachableTiles")]
    public Dictionary<Vector2Int, int> GetReachableTiles(Vector2Int startPos, int totalMovementPoints)
    {
        ClearHighlights();
        Dictionary<Vector2Int, int> reachableTiles = new();
        List<Vector2Int> frontier = new();

        reachableTiles[startPos] = 0;
        frontier.Add(startPos);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier[0];
            int bestCost = reachableTiles[current];

            foreach (var tile in frontier)
            {
                if (reachableTiles[tile] < bestCost)
                {
                    bestCost = reachableTiles[tile];
                    current = tile;
                }
            }

            frontier.Remove(current);

            foreach (Vector2Int next in GetNeighbors(current))
            {
                if (!gridData.ContainsKey(next)) continue;

                var tile = gridData[next].tileScript.tileScr;
                if (tile.isWall || gridData[next].unitOnTop != null) continue;

                int newCost = reachableTiles[current] + tile.movementCost;

                if (newCost > totalMovementPoints) continue;

                if (!reachableTiles.ContainsKey(next) || newCost < reachableTiles[next])
                {
                    reachableTiles[next] = newCost;
                    gridData[next].parentPos = current;
                    if (!frontier.Contains(next))
                        frontier.Add(next);
                }
            }
        }
        reachableTiles.Remove(startPos);
        foreach (Node tile in gridData.Values)
        {
            tile.tileScript.movementCosttxt.text = "";
        }
        foreach (var kvp in reachableTiles)
        {
            gridData[kvp.Key].tileScript.movementCosttxt.text = kvp.Value.ToString();
            gridData[kvp.Key].MovementCost = kvp.Value;
        }

        return reachableTiles;
    }

    public void InitializeGridData()
    {
        gridData = new Dictionary<Vector2Int, Node>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                GameObject tileGO = tilemap.GetInstantiatedObject(pos);

                if (tileGO != null)
                {
                    TileScript tScript = tileGO.GetComponent<TileScript>();

                    Node newNode = new Node
                    {
                        tileScript = tScript,
                        parentPos = new Vector2Int(pos.x, pos.y)
                    };
                    gridData.Add(new Vector2Int(pos.x, pos.y), newNode);
                }
                else
                {
                    Debug.LogWarning(pos + " pozisyonundaki Tile'ın prefab'ine ulaşılamadı!");
                }
            }
        }
    }
    public List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new();

        Vector2Int[] evenRowNeighbors = {
        new(1, 0),   // Sağ
        new(0, -1),  // Sağ Alt
        new(-1, -1), // Sol Alt
        new(-1, 0),  // Sol
        new(-1, 1),  // Sol Üst
        new(0, 1)    // Sağ Üst
    };

        Vector2Int[] oddRowNeighbors = {
        new(1, 0),   // Sağ
        new(1, -1),  // Sağ Alt
        new(0, -1),  // Sol Alt
        new(-1, 0),  // Sol
        new(0, 1),   // Sol Üst
        new(1, 1)    // Sağ Üst
    };

        bool isOddRow = Mathf.Abs(pos.y) % 2 != 0;
        Vector2Int[] directions = isOddRow ? oddRowNeighbors : evenRowNeighbors;

        foreach (Vector2Int dir in directions)
        {
            if (gridData.ContainsKey(pos + dir))
            {
                neighbors.Add(pos + dir);
            }
        }

        return neighbors;
    }
    public void HighlightMovementTiles(Dictionary<Vector2Int, int> reachableTiles)
    {
        foreach (var tilePos in reachableTiles.Keys)
        {
            Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
            tilemap.SetTileFlags(cellPos, TileFlags.None);
            tilemap.SetColor(cellPos, Color.lightCyan);
            Node node_ = new()
            {
                tileScript = gridData[tilePos].tileScript
            };
            moveHighlightedGrids.Add(tilePos, node_);
        }
    }
    public void ClearHighlights()
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector2Int gridPos = (Vector2Int)pos;
            if (gridData.ContainsKey(gridPos))
            {
                gridData[gridPos].tileScript.movementCosttxt.text = "";
            }
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, Color.white);
        }
        attackHighlightedGrids.Clear();
        moveHighlightedGrids.Clear();
    }
    public void GetUsableTiles(Vector2Int centerCell)
    {
        ClearHighlights();
        SkillScriptable selectedSkill_ = skillManager.selectedSkill;
        CharacterScript char_ = gridData[centerCell].unitOnTop;
        int range_ = selectedSkill_.range;

        for (int q = -range_; q <= range_; q++)
        {
            for (int r = -range_; r <= range_; r++)
            {
                Vector2Int cellPos = new(centerCell.x + q, centerCell.y + r);

                if (!gridData.ContainsKey(cellPos) || GetHexDistance(centerCell, cellPos) > range_) continue;

                var targetGrid = gridData[cellPos];
                SkillScriptable.TargetType currentTileType = SkillScriptable.TargetType.NONE;

                if (targetGrid.unitOnTop != null)
                {
                    bool isEnemy = targetGrid.unitOnTop.stats.TeamType != char_.stats.TeamType;
                    currentTileType = isEnemy ? SkillScriptable.TargetType.Enemy : SkillScriptable.TargetType.Ally;
                }
                else if (!targetGrid.tileScript.tileScr.isWall)
                {
                    currentTileType = SkillScriptable.TargetType.EmptyTile;
                }
                if (selectedSkill_.AvailableTarget.HasFlag(currentTileType) && currentTileType != SkillScriptable.TargetType.NONE)
                {
                    Color color = currentTileType switch
                    {
                        SkillScriptable.TargetType.Enemy => Color.red,
                        SkillScriptable.TargetType.Ally => Color.seaGreen,
                        SkillScriptable.TargetType.EmptyTile => Color.blue,
                        _ => Color.white
                    };

                    tilemap.SetColor((Vector3Int)cellPos, color);
                    attackHighlightedGrids.Add(cellPos, targetGrid);
                }
            }
        }
    }
    public int GetHexDistance(Vector2Int a, Vector2Int b)
    {
        int aq = a.x - (a.y - (a.y & 1)) / 2;
        int ar = a.y;

        int bq = b.x - (b.y - (b.y & 1)) / 2;
        int br = b.y;

        return (Mathf.Abs(aq - bq) + Mathf.Abs(aq + ar - bq - br) + Mathf.Abs(ar - br)) / 2;
    }
    public List<Vector3Int> GetHexTilesInRange(Vector3Int center, int range)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        for (int q = -range; q <= range; q++)
        {
            for (int r = Mathf.Max(-range, -q - range); r <= Mathf.Min(range, -q + range); r++)
            {
                int s = -q - r;
                tiles.Add(new Vector3Int(center.x + q, center.y + r, center.z + s));
            }
        }
        return tiles;
    }
    public IEnumerator ChooseTileForPutCharacter(Vector2Int chosenTilePos)
    {
        print("Spawned");
        yield return StartCoroutine(GameManager.instance.SpawnCharacter(chosenCharsToPut[id], 0, chosenTilePos));
        TilesForPutting.Remove(chosenTilePos);
        id++;
        ClearHighlights();
        HighlightTilesForPutChar();

    }
    public int id;
    public List<PlayerDataScriptable.CharacterData> chosenCharsToPut;
    public List<Vector2Int> TilesForPutting;
    public void HighlightTilesForPutChar()
    {
        print("Highlight");
        if (id >= chosenCharsToPut.Count || chosenCharsToPut.Count == 0)
        {
            print("Oh no");
            ClearHighlights();
            GameManager.instance.StartGame();
            return;
        }
        for (int i = 0; i < TilesForPutting.Count; i++)
        {
            print("yes yes");

            Vector3Int cellPos = new(TilesForPutting[i].x, TilesForPutting[i].y, 0);
            tilemap.SetTileFlags(cellPos, TileFlags.None);
            tilemap.SetColor(cellPos, Color.limeGreen);
            //gridData[TilesForPutting[i]].tileScript.GetComponent<SpriteRenderer>().color = Color.limeGreen;
            tilemap.RefreshTile(cellPos);
        }
    }
}