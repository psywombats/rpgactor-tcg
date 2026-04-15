using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageCharaRenderer : CharaRenderer
{
    private Image image;
    private Image Image => image ??= GetComponent<Image>();
    
    protected override void SetSprite(Sprite sprite)
    {
        Image.sprite = sprite;
    }
}