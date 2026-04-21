using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataScriptable", menuName = "Scriptable Objects/LevelDataScriptable")]
[System.Serializable]
public class LevelDataScriptable : ScriptableObject
{
    public SpawningCharStats[] Chars;
    public List<Vector2Int> PlaceableTiles;
    public GameObject Map;
    [System.Serializable]
    public class SpawningCharStats
    {
        public PlayerDataScriptable.CharacterData SpawningCharData;
        public int TeamType;
        public Vector2Int SpawnTilePos;
    }
    public int DialogId;
    public bool isPlayingWhenLose;




}
