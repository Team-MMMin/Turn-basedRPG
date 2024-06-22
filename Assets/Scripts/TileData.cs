using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "SRPG/TileData")]
public class TileData : ScriptableObject
{
    public enum TileType { Normal, Blocked, Special }
    public TileType type;

    public int moveCost = 1;

    // Special tile properties
    public bool isHealingTile;
    public int healingAmount;

    public bool isManaTile;
    public int manaAmount;

    public bool isDefenseTile;
    public int defenseAmount;

    public bool isAttackTile;
    public int attackAmount;

    public bool isDamageTile;
    public float damagePercent;

    public bool singleUse; // Indicates if the effect is single-use
}
