using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    TextMeshPro _damageText;

    public void SetInfo(Vector2 pos, float damage, Transform parent)
    {
        _damageText = GetComponent<TextMeshPro>();
        _damageText.sortingOrder = SortingLayers.DAMAGE_FONT;
        transform.position = pos;

        _damageText.text = $"{Mathf.Abs(damage)}";
        _damageText.alpha = 1;

        Invoke("DestroyDamageFont", 0.5f);
    }

    void DestroyDamageFont()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
