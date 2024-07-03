using UnityEngine;

public class Entity : MonoBehaviour
{
    public int Hp;
    public int Mp;
    public int Atk;
    public int Defense;
    public OverlayTile currentTile;

    public void MoveToTile(OverlayTile newTile)
    {
        if (currentTile != null)
        {
            currentTile.hasCharacter = false;
            currentTile.RemoveTileEffect(this);
        }

        currentTile = newTile;
        currentTile.hasCharacter = true;
        transform.position = currentTile.transform.position;

        ApplyTileEffects();
    }

    private void ApplyTileEffects()
    {
        currentTile.ApplyTileEffect(this);
    }
}
