using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpritesheetData : ScriptableObject 
{
    [SerializeField] private Texture2D spritesheet;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private List<int> walkCycleN, walkCycleE, walkCycleW, walkCycleS;

    public int StepCount => walkCycleN.Count;

    private Dictionary<string, Sprite> spritesByName;
    private Dictionary<string, Sprite> SpritesByName
    {
        get 
        {
            if (spritesByName == null) 
            {
                if (sprites == null || sprites.Count == 0) 
                {
                    return null;
                }
                spritesByName = new Dictionary<string, Sprite>();
                foreach (var sprite in sprites) 
                {
                    spritesByName[sprite.name] = sprite;
                }
            }
            return spritesByName;
        }
    }

    public Sprite GetSprite(OrthoDir dir, int step) 
    {
        if (SpritesByName == null) return null;
        var walkCycle = GetWalkCycle(dir);
        if (walkCycle.Count <= step) throw new ArgumentException();
        var frameNumber = walkCycle[step];
        var frameName = NameForFrame(name, dir, frameNumber);

        if (!SpritesByName.ContainsKey(frameName) && Application.isPlaying) 
        {
            Debug.LogError(this + " doesn't contain frame " + frameName);
            return null;
        }

        return SpritesByName[frameName];
    }

    public Sprite GetPreviewSprite() 
    {
        if (SpritesByName == null) return null;
        var frameName = NameForFrame(name, OrthoDir.South, 0);
        return SpritesByName.ContainsKey(frameName) ? SpritesByName[frameName] : spritesByName.Values.FirstOrDefault();
    }

    public static string NameForFrame(string sheetName, OrthoDir dir, int step) 
    {
        return sheetName + "_" + dir + "_" + step;
    }

    private List<int> GetWalkCycle(OrthoDir dir)
    {
        return dir switch
        {
            OrthoDir.North => walkCycleN,
            OrthoDir.East  => walkCycleE,
            OrthoDir.West  => walkCycleW,
            OrthoDir.South => walkCycleS,
            _              => throw new ArgumentException()
        };
    }
}
