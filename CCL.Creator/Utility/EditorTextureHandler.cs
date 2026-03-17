using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Utility
{
    internal class EditorTextureHandler
    {
        private static readonly Dictionary<string, Texture2D?> s_textures = new Dictionary<string, Texture2D?>();

        private static bool DarkMode => EditorGUIUtility.isProSkin;

        public static bool TryGetTexture(string name, out Texture2D texture)
        {
            // Check for a dark mode texture first.
            if (DarkMode && TryGetValidTexture($"D_{name}", out texture)) return true;

            // Fallback to light mode.
            return TryGetValidTexture(name, out texture);
        }

        public static bool TryGetTexture(string path, string name, out Texture2D texture)
        {
            // Add the path before the darkmode identifier.
            if (DarkMode && TryGetValidTexture($"{path}/D_{name}", out texture)) return true;

            // Fallback to light mode.
            return TryGetValidTexture($"{path}/{name}", out texture);
        }

        public static Texture2D GetTexture(string path, string name)
        {
            if (!TryGetTexture(path, name, out var texture))
            {
                Debug.LogError($"Could not find texture Assets/CarCreator/{path}/{name}.png");
            }
            return texture;
        }

        private static bool TryGetValidTexture(string name, out Texture2D texture)
        {
            if (s_textures.TryGetValue(name, out texture!))
            {
                return texture;
            }

            texture = LoadTexture(name)!;
            return texture;
        }

        private static Texture2D? LoadTexture(string name)
        {
            string assetPath = $"Assets/CarCreator/{name}.png";

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture)
            {
                s_textures.Add(name, texture);
                return texture;
            }
            return null;
        }
    }
}
