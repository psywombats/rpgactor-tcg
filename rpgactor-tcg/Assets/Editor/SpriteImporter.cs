using System;
using UnityEditor;
using System.IO;
using System.Linq;
using Editor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using FilterMode = UnityEngine.FilterMode;

internal sealed class SpriteImporter : AssetPostprocessor 
{
    public void OnPreprocessTexture()
    {
        var formatData = GetFormatData(assetPath);
        var importer = (TextureImporter)assetImporter;
        if (formatData != null)
        {
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = formatData.PixelsPerUnit;
            
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(assetImporter);
            dataProvider.InitSpriteEditorDataProvider();
            formatData.ApplyToEditorData(dataProvider, EditorUtils.GetPreprocessedImageSize(importer), Path.GetFileNameWithoutExtension(assetPath));
            dataProvider.Apply();
        }
    }

    private static SpritesheetFormatData GetFormatData(string path)
    {
        var currentPath = path;

        SpritesheetFormatData formatData = null;
        while (formatData == null && currentPath.Contains("/"))
        {
            currentPath = EditorUtils.LocalDirectoryFromPath(currentPath);
            var formatDataPath = currentPath + "/" + SpritesheetFormatData.DefaultFilename + ".asset";
            formatData = AssetDatabase.LoadAssetAtPath<SpritesheetFormatData>(formatDataPath);
        }

        return formatData;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var path in importedAssets)
        {
            if (path.EndsWith(".png"))
            {
                PostprocessTexture(path);
            }
        }
    }

    private static void PostprocessTexture(string assetPath)
    {
        var format = GetFormatData(assetPath);
        if (format == null || format.IsSingleSprite) return;
        
        var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath)?.OfType<Sprite>().ToList();
        if (sprites == null) return;
        
        sprites.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        var spriteCount = sprites.Count / format.TotalFrames;
        var assetName = Path.GetFileNameWithoutExtension(assetPath);
        for (var i = 0; i < spriteCount; i++)
        {
            var sheetName = assetName + (spriteCount > 1 ? i.ToString("D2") : "");
            var spritesheetPath = $"{EditorUtils.LocalDirectoryFromPath(assetPath)}/{sheetName}.asset";
            var spritesheetData = AssetDatabase.LoadAssetAtPath<SpritesheetData>(spritesheetPath);
            if (spritesheetData == null)
            {
                spritesheetData = ScriptableObject.CreateInstance<SpritesheetData>();
                AssetDatabase.CreateAsset(spritesheetData, spritesheetPath);
            }

            var relevantSprites = sprites.GetRange(i * format.TotalFrames, format.TotalFrames);
            spritesheetData.PopulateFromSerializedSprite(relevantSprites, format, sheetName);
            EditorUtility.SetDirty(spritesheetData);
            AssetDatabase.SaveAssetIfDirty(spritesheetData);
        }
    }
}
