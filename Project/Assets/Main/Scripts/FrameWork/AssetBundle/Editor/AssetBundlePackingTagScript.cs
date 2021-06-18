using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameBerry
{
    public class AssetBundlePackingTagScript : ScriptableObject
    {
        private Sprite[] sprites;

        static public string assetDirPath = Application.dataPath + "AssetBundle";

        public static void AssetBundlePackingTag()
        {
#if UNITY_EDITOR
            if (Selection.assetGUIDs.Length == 0)
                return;

            var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            SetAssetBundlePackingTag(path);
#endif

        }

        public static void SetAssetBundlePackingTag(string path)
        {
            string[] spritenames = AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets/AssetBundle" });

            foreach (string name in spritenames)
            {
                string convertPath = AssetDatabase.GUIDToAssetPath(name);

                string[] pathblock = convertPath.Split('/');

                string parentpath = pathblock[pathblock.Length - 2];

                TextureImporter importer = TextureImporter.GetAtPath(convertPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureCompression = TextureImporterCompression.CompressedHQ;
                    importer.spritePackingTag = parentpath;
                }
            }
        }
    }
}