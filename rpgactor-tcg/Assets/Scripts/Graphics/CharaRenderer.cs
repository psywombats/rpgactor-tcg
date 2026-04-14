using EditorAttributes;
using UnityEngine;

public abstract class CharaRenderer : MonoBehaviour
{
    [SerializeField, Required] private CharaModelView model;

    protected void Awake()
    {
        model.OnSpriteUpdated += ModelOnOnSpriteUpdated;
    }

    protected abstract void SetSprite(Sprite sprite);

    private void ModelOnOnSpriteUpdated(Sprite sprite)
    {
        SetSprite(sprite);
    }
}