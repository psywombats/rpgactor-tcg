using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class EditorUtils
    {
        public static string LocalDirectoryFromPath(string path)
        {
            var components = new List<string>(path.Split(new[] { '/' }));
            components.RemoveAt(components.Count - 1);
            return string.Join("/", components);
        }

        public static Vector2Int GetPreprocessedImageSize(TextureImporter importer)
        {
            var args = new object[] { 0, 0 };
            var methodInfo =
                typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(importer, args);
            return new Vector2Int((int)args[0], (int)args[1]);
        }

        public static List<Tuple<string, T>> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            return new List<Tuple<string, T>>(guids.Select(guid =>
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return new Tuple<string, T>(path, AssetDatabase.LoadAssetAtPath<T>(path));
            }));
        }
    }
}