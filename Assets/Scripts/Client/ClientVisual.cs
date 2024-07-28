using System.Collections.Generic;
using UnityEngine;

public class ClientVisual : MonoBehaviour
{
    [SerializeField] SpriteRenderer _base;
    [SerializeField] List<SpriteRenderer> _accessories;

    public Vector3 Offset { get; private set; }

    public void Setup(List<Sprite> sprites) 
    {
        _base.sprite = sprites[0];
        for (int i = 0; i < _accessories.Count; i++)
        {
            _accessories[i].sprite = (i + 1 < sprites.Count) ? sprites[i + 1] : null;
        }

        Offset = CalculateOffset();
        transform.localPosition += Offset;
    }

    private Vector3 CalculateOffset()
    {
        return new Vector3(
            _base.sprite.pivot.x - _base.sprite.rect.width / 2, 
            _base.sprite.pivot.y)
            / _base.sprite.pixelsPerUnit;
    }
}
