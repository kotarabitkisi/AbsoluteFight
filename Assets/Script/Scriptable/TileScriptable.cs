using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Objects/Tile")]
public class TileScriptable : ScriptableObject
{
    public int movementCost;
    [Range(0,1)]
    public float HealAmount,CleanseAmount;
    public bool isWall;
    public float damageMultiplier,damageAmount;
    public int virusId;//if virusId=-1 it doesnt spread virus
    [Range(0,1)]
    public int virusDamageMultiplier;
    [Range(0,1)]
    public float defenseMultiplier;
    [Range(0,1)]
    public float attackBuffMultiplier;

}
