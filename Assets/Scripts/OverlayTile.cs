
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    public Vector2Int gridLocation;
    public TileData tileData;
    public bool isBlocked;
    public bool hasCharacter;

    private SpriteRenderer spriteRenderer;
    private bool hasAppliedEffect; // Tracks if the effect has been applied

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(TileData data, Vector2Int location)
    {
        tileData = data;
        gridLocation = location;
        isBlocked = tileData.type == TileData.TileType.Blocked;

        // Visual representation (optional)
        if (isBlocked)
        {
            spriteRenderer.color = Color.gray; // or any other representation for blocked tiles
        }
    }

    public void ShowTile(Color color)
    {
        spriteRenderer.color = color;
    }

    public void HideTile()
    {
        spriteRenderer.color = Color.white;
    }

    public void ApplyTileEffect(Entity entity)
    {
        if (tileData.isHealingTile && !hasAppliedEffect)
        {
            entity.Hp += tileData.healingAmount;
            hasAppliedEffect = tileData.singleUse;
        }

        if (tileData.isManaTile && !hasAppliedEffect)
        {
            entity.Mp += tileData.manaAmount;
            hasAppliedEffect = tileData.singleUse;
        }

        if (tileData.isDefenseTile)
        {
            entity.Defense += tileData.defenseAmount;
        }

        if (tileData.isAttackTile)
        {
            entity.Atk += tileData.attackAmount;
        }

        if (tileData.isDamageTile && !hasAppliedEffect)
        {
            entity.Hp -= Mathf.FloorToInt(entity.Hp * tileData.damagePercent);
            hasAppliedEffect = tileData.singleUse;
        }
    }

    public void RemoveTileEffect(Entity entity)
    {
        if (tileData.isDefenseTile)
        {
            entity.Defense -= tileData.defenseAmount;
        }

        if (tileData.isAttackTile)
        {
            entity.Atk -= tileData.attackAmount;
        }
    }
}
