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
            importer.spriteImportMode = formatData.IsSingleSprite ? SpriteImportMode.Single : SpriteImportMode.Multiple;
            
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
        var formatData = GetFormatData(assetPath);
        if (formatData == null || formatData.IsSingleSprite)
        {
            return;
        }
        
        var spritesheetName = Path.GetFileNameWithoutExtension(assetPath);
        var spritesheetPath = EditorUtils.LocalDirectoryFromPath(assetPath) + "/" + spritesheetName + ".asset";
        var spritesheetData = AssetDatabase.LoadAssetAtPath<SpritesheetData>(spritesheetPath);
        if (spritesheetData == null)
        {
            spritesheetData = ScriptableObject.CreateInstance<SpritesheetData>();
            AssetDatabase.CreateAsset(spritesheetData, spritesheetPath);
        }

        var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath)?.OfType<Sprite>();
        spritesheetData.PopulateFromSerializedSprite(sprites, formatData);
        EditorUtility.SetDirty(spritesheetData);
        AssetDatabase.SaveAssetIfDirty(spritesheetData);
    }
}
