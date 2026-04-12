using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An instance of SpritesheetData powers a single character sprite, supporting multiple directions and frames.
/// </summary>
/// <remarks>
/// Spritesheet data can either be serialized (in the case of spritesheets shipped with the game) or created dynamically
/// (in the case of custom or DL'd spritesheets). 
/// </remarks>
public class SpritesheetData : ScriptableObject 
{
    [SerializeField] private List<Sprite> serializedSprites;
    [SerializeField] private SpritesheetFormatData format;

    public int StepCount => format.WalkCycle.Count;

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
    
    public void PopulateFromTexture(Texture2D texture, SpritesheetFormatData format)
    {
        if (!Application.isPlaying) throw new ArgumentException("Can only populate from a texture at runtime");
        this.format = format;
        serializedSprites = format.Split(texture);
    }

    public void PopulateFromSerializedSprite(IEnumerable<Sprite> sprites, SpritesheetFormatData format)
    {
        if (Application.isPlaying) throw new ArgumentException("Can only populate from serialized sprites in editor");
        this.format = format;
        serializedSprites = sprites.ToList();
    }

    public Sprite GetSprite(OrthoDir dir, int step) 
    {
        if (SpritesByName == null) return null;
        var walkCycle = GetWalkCycle(dir);
        if (walkCycle.Count <= step) throw new ArgumentException();
        var frameNumber = walkCycle[step];
        var frameName = SpritesheetFormatData.NameForFrame(name, dir, frameNumber);

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
        var frameName = SpritesheetFormatData.NameForFrame(name, OrthoDir.South, GetWalkCycle(OrthoDir.South)[0]);
        return SpritesByName.ContainsKey(frameName) ? SpritesByName[frameName] : serializedSprites.FirstOrDefault();
    }

    private List<int> GetWalkCycle(OrthoDir _) => format.WalkCycle;
}
