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

    [SerializeField] private string formatName = DefaultFilename;
    [Space]
    [SerializeField] private bool isSingleSprite;
    [SerializeField] private int pixelsPerUnit;
    [SerializeField] private Vector2Int frameSize;
    [SerializeField, ConditionalField(ConditionType.NOR, nameof(isSingleSprite))] private List<OrthoDir> facings;
    [SerializeField, ConditionalField(ConditionType.NOR, nameof(isSingleSprite))] private List<int> walkCycle;
    [SerializeField, ConditionalField(ConditionType.NOR, nameof(isSingleSprite))] private bool supportsMultipleSheetsPerFile;
    
    public bool IsSingleSprite => isSingleSprite;
    public List<int> WalkCycle => walkCycle;
    public int PixelsPerUnit => pixelsPerUnit;

    private int framesPerDir;
    public int FramesPerDir => framesPerDir > 0 ? framesPerDir : framesPerDir = WalkCycle.Max() + 1;
    public int TotalFrames => FramesPerDir * facings.Count;

    public List<Sprite> Split(Texture2D texture)
    {
        if (!ValidateSheetSize(new Vector2Int(texture.width, texture.height), texture.name)) return null;

        if (isSingleSprite)
        {
            return new List<Sprite> 
            {
                Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(.5f, .5f), pixelsPerUnit)
            };
        }
        
        var sprites = new List<Sprite>();
        var indexInSheet = 0;
        for (var baseX = 0; baseX < texture.width; baseX += frameSize.x * FramesPerDir)
        {
            for (var baseY = 0; baseY < texture.height; baseY += frameSize.y * facings.Count)
            {
                for (var x = 0; x < FramesPerDir; x++)
                {
                    for (var y = 0; y < facings.Count; y++)
                    {
                        var sprite = Sprite.Create(texture, new Rect(
                                baseX + x * frameSize.x, baseY + y * frameSize.y,
                                frameSize.x, frameSize.y),
                            new Vector2(0.5f, 0.5f), pixelsPerUnit);
                        sprite.name = NameForFrame(texture.name, facings[facings.Count - y - 1], x, indexInSheet);
                        sprites.Add(sprite);
                    }
                }
                indexInSheet += 1;
            }
        }

        return sprites;
    }

    public void ApplyToEditorData(ISpriteEditorDataProvider dataProvider, Vector2Int textureSize, string sheetName)
    {
        if (!ValidateSheetSize(textureSize, sheetName)) return;
        
        var spriteRects = new List<SpriteRect>();
        var spriteIdNamePairs = new List<SpriteNameFileIdPair>();
        var indexInSheet = 0;
        for (var baseX = 0; baseX < textureSize.x; baseX += frameSize.x * FramesPerDir)
        {
            for (var baseY = 0; baseY < textureSize.y; baseY += frameSize.y * facings.Count)
            {
                for (var x = 0; x < FramesPerDir; x++)
                {
                    for (var y = 0; y < facings.Count; y++)
                    {
                        var guid = GUID.Generate();
                        var spriteName = NameForFrame(sheetName, facings[facings.Count - y - 1], x, indexInSheet);
                        spriteRects.Add(new SpriteRect
                        {
                            name = spriteName,
                            spriteID = guid,
                            rect = new Rect(baseX + x * frameSize.x, baseY + y * frameSize.y, frameSize.x, frameSize.y)
                        });
                        spriteIdNamePairs.Add(new SpriteNameFileIdPair(spriteName, guid));
                    }
                }
                indexInSheet += 1;
            }
        }

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        
        var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        spriteNameFileIdDataProvider.SetNameFileIdPairs(spriteIdNamePairs);
    }
    
    public string NameForFrame(string sheetName, OrthoDir dir, int step, int indexInSheet = 0)
    {
        return isSingleSprite ? $"{sheetName}{indexInSheet:D2}" : $"{sheetName}{indexInSheet:D2}_{dir}{step:D2}";
    }

    private bool ValidateSheetSize(Vector2Int sheetSize, string sheetName)
    {
        if (isSingleSprite) return true;
        if (supportsMultipleSheetsPerFile)
        {
            if (sheetSize.x / frameSize.x % (walkCycle.Max() + 1) != 0)
            {
                Debug.LogError($"Importing [{sheetName}] as [{formatName}] failed: expected width to be multiple of " +
                               $"{walkCycle.Max() + 1} but tex has width of {sheetSize.x}");
                return false;
            }
            if (sheetSize.y / frameSize.y % facings.Count != 0)
            {
                Debug.LogError($"Importing [{sheetName}] as [{formatName}] failed: expected height to be multiple of " +
                               $"{facings.Count} but tex has height of {sheetSize.y}");
                return false;
            }
        }
        else
        {
            if (sheetSize.x / frameSize.x != walkCycle.Max() + 1)
            {
                Debug.LogError($"Importing [{sheetName}] as [{formatName}] failed: expected a width of " +
                               $"{(walkCycle.Max() + 1) * frameSize.x} but tex has width of {sheetSize.x}");
                return false;
            }
            if (sheetSize.y / frameSize.y != facings.Count)
            {
                Debug.LogError($"Importing [{sheetName}] as [{formatName}] failed: expected a height of " +
                               $"{facings.Count * frameSize.y} but tex has height of {sheetSize.y}");
                return false;
            }
        }
        return true;
    }
}

