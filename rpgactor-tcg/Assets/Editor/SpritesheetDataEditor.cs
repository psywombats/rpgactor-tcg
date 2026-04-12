using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(SpritesheetData))]
public class SpritesheetDataEditor : UnityEditor.Editor 
{
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        var data = (SpritesheetData)target;
        if (data == null) return null;
        var sprite = data.GetPreviewSprite();
        if (sprite == null) return null;
        
        var type = ReflectionUtils.FindReflectedType("UnityEditor.SpriteUtility");
        if (type == null)
        {
            // reflection failed -- w/e, use backup
            return AssetPreview.GetAssetPreview(sprite);
        }
        
        var method = type.GetMethod("RenderStaticPreview",new[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
        if (method == null)
        {
            return AssetPreview.GetAssetPreview(sprite);
        }

        var ret = method.Invoke("RenderStaticPreview", new object[] { sprite, Color.white, width, height });
        return ret as Texture2D;
    }
}
