using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RpgActorTGC;

/// <summary>
/// An instance of SpritesheetData powers a single character sprite, supporting multiple directions and frames.
/// </summary>
/// <remarks>
/// Spritesheet data can either be serialized (in the case of spritesheets shipped with the game) or created dynamically
/// (in the case of custom or DL'd spritesheets). 
/// </remarks>
[DatabaseIndexed]
public class SpritesheetData : ScriptableObject, IDatabaseKeyable
{
    [SerializeField] private string tag;
    [Space]
    [SerializeField] private List<Sprite> serializedSprites;
    [SerializeField] private SpritesheetFormatData format;
    [SerializeField] private int indexInSheet;

    public int StepCount => format.WalkCycle.Count;

    public string Key => tag;

    private Dictionary<string, Sprite> spritesByName;
    private Dictionary<string, Sprite> SpritesByName
    {
        get 
        {
            if (spritesByName == null) 
            {
                if (serializedSprites == null || serializedSprites.Count == 0) 
                {
                    return null;
                }
                spritesByName = new Dictionary<string, Sprite>();
                foreach (var sprite in serializedSprites) 
                {
                    spritesByName[sprite.name] = sprite;
                }
            }
            return spritesByName;
        }
    }
    
    public void PopulateFromTexture(Texture2D texture, SpritesheetFormatData format, int indexInSheet)
    {
        if (!Application.isPlaying) throw new ArgumentException("Can only populate from a texture at runtime");
        this.format = format;
        this.indexInSheet = indexInSheet;
        serializedSprites = format.Split(texture);
    }

    public void PopulateFromSerializedSprite(IEnumerable<Sprite> sprites, SpritesheetFormatData format, 
        int indexInSheet, string tag)
    {
        if (Application.isPlaying) throw new ArgumentException("Can only populate from serialized sprites in editor");
        this.format = format;
        this.indexInSheet = indexInSheet;
        serializedSprites = sprites.ToList();
        if (string.IsNullOrEmpty(this.tag)) this.tag = tag;
    }

    public Sprite GetSprite(OrthoDir dir, int step) 
    {
        if (SpritesByName == null) return null;
        var walkCycle = GetWalkCycle(dir);
        if (walkCycle.Count <= step) throw new ArgumentException();
        var frameNumber = walkCycle[step];
        var frameName = format.NameForFrame(name, dir, frameNumber, indexInSheet);

        if (!SpritesByName.ContainsKey(frameName) && Application.isPlaying) 
        {
            Debug.LogError(this + " doesn't contain frame " + frameName);
            return null;
        }

        return SpritesByName[frameName];
    }

    public Sprite GetPreviewSprite() 
    {
        if (serializedSprites == null || serializedSprites.Count == 0) return null;
        var frameName = format.NameForFrame(name, OrthoDir.South, GetWalkCycle(OrthoDir.South)[0], indexInSheet);
        return SpritesByName.ContainsKey(frameName) ? SpritesByName[frameName] : serializedSprites.FirstOrDefault();
    }

    private List<int> GetWalkCycle(OrthoDir _) => format.WalkCycle;
}
