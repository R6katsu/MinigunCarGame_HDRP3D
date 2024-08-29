#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestSetTextureNonCompressed : MonoBehaviour
{
    [MenuItem("Tools/Set Textures Non-Compressed")]
    public static void SetTexturesNonCompressed()
    {
        string folderPath = "Assets/Texture"; // テクスチャが保存されているフォルダのパス
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);

            if (textureImporter != null)
            {
                // テクスチャの形式を非圧縮形式に設定
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.SaveAndReimport();
            }
        }
    }
}
#endif