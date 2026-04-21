using TMPro;
using UnityEngine;

public class TileScript : MonoBehaviour
{
   public Vector2Int gridPos;
   public CharacterScript whoIsHere;
   public TileScriptable tileScr;
   public TextMeshProUGUI movementCosttxt;
   public void InitializePos()
   {
      Vector3Int cell = GridMapManager.instance.tilemap.WorldToCell(transform.position);
      gridPos = new Vector2Int(cell.x, cell.y);
   }
}
