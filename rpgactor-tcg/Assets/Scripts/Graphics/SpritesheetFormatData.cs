using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

[CreateAssetMenu(fileName = DefaultFilename, menuName = "SpriteFormat")]
public class SpritesheetFormatData : ScriptableObject
{
    public const string DefaultFilename = "SpriteFormat";

    [SerializeField] private bool isSingleSprite;
    [SerializeField] private int pixelsPerUnit;
    [SerializeField] private Vector2Int frameSize;
    [SerializeField, ConditionalField(ConditionType.NOR, nameof(isSingleSprite))] private List<OrthoDir> facings;
    [SerializeField, ConditionalField(ConditionType.NOR, nameof(isSingleSprite))] private List<int> walkCycle;
    
    public bool IsSingleSprite => isSingleSprite;
    public List<int> WalkCycle => walkCycle;

    public List<Sprite> Split(Texture2D texture)
    {
        if (isSingleSprite)
        {
            throw new ArgumentException("No need to split single sprites");
        }

        if (texture.width / frameSize.x != walkCycle.Max() + 1)
        {
            Debug.LogError($"Importing [{texture.name}] as [{name}] failed: expected a width of " +
                           $"{(walkCycle.Max() + 1) * frameSize.x} but tex has width of {texture.width}");
            return null;
        }
        if (texture.height / frameSize.y != facings.Count)
        {
            Debug.LogError($"Importing [{texture.name}] as [{name}] failed: expected a height of " +
                           $"{facings.Count * frameSize.y} but tex has height of {texture.height}");
            return null;
        }
        
        var sprites = new List<Sprite>();
        for (var y = 0; y < frameSize.y / frameSize.y; y++)
        {
            for (var x = 0; x < texture.width / frameSize.x; x++)
            {
                var sprite = Sprite.Create(texture, new Rect(
                        x * frameSize.x, y * frameSize.y,
                        frameSize.x, frameSize.y),
                    new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.name = NameForFrame(texture.name, facings[facings.Count - y - 1], x);
                sprites.Add(sprite);
            }
        }
        return sprites;
    }
    
    public static string NameForFrame(string sheetName, OrthoDir dir, int step) 
    {
        return sheetName + "_" + dir + "_" + step;
    }

    public void ApplyToEditorData(ISpriteEditorDataProvider dataProvider, Vector2Int textureSize, string sheetName)
    {
        if (textureSize.x / frameSize.x != walkCycle.Max() + 1)
        {
            Debug.LogError($"Importing [{sheetName}] as [{name}] failed: expected a width of " +
                           $"{(walkCycle.Max() + 1) * frameSize.x} but tex has width of {textureSize.y}");
            return;
        }
        if (textureSize.y / frameSize.y != facings.Count)
        {
            Debug.LogError($"Importing [{sheetName}] as [{name}] failed: expected a height of " +
                           $"{facings.Count * frameSize.y} but tex has height of {textureSize.y}");
            return;
        }
        
        var spriteRects = new List<SpriteRect>();
        var spriteIdNamePairs = new List<SpriteNameFileIdPair>();
        for (var y = 0; y < textureSize.y / frameSize.y; y++)
        {
            for (var x = 0; x < textureSize.x / frameSize.x; x++)
            {
                var guid = GUID.Generate();
                var spriteName = NameForFrame(sheetName, facings[facings.Count - y - 1], x);
                spriteRects.Add(new SpriteRect
                {
                    name = spriteName,
                    spriteID = guid,
                    rect = new Rect(x * frameSize.x, y * frameSize.y, frameSize.x, frameSize.y)
                });
                spriteIdNamePairs.Add(new SpriteNameFileIdPair(spriteName, guid));
            }
        }

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        
        var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        spriteNameFileIdDataProvider.SetNameFileIdPairs(spriteIdNamePairs);
    }
}

