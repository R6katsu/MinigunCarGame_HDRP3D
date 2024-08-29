#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class TestTextureAtlasGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Texture Atlas")]
    public static void GenerateTextureAtlas()
    {
        // テクスチャを取得するフォルダのパスを指定
        string folderPath = "Assets/Texture/Block";
        string[] texturePaths = Directory.GetFiles(folderPath, "*.png");

        // 読み込んだテクスチャをリストに格納
        Texture2D[] textures = new Texture2D[texturePaths.Length];
        for (int i = 0; i < texturePaths.Length; i++)
        {
            textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePaths[i]);
        }

        // テクスチャの数に基づいて正方形グリッドのサイズを計算
        int textureCount = textures.Length;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(textureCount));
        int textureSize = textures[0].width; // 全てのテクスチャが同じサイズであると仮定

        // アトラスのサイズを決定
        int atlasSize = gridSize * textureSize;
        Texture2D textureAtlas = new Texture2D(atlasSize, atlasSize);

        // 手動でテクスチャをグリッドに配置
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;
                if (index >= textureCount) break;

                // テクスチャを配置
                Texture2D texture = textures[index];
                textureAtlas.SetPixels(x * textureSize, (gridSize - y - 1) * textureSize, textureSize, textureSize, texture.GetPixels());
            }
        }

        textureAtlas.Apply();

        // テクスチャアトラスを保存
        byte[] atlasBytes = textureAtlas.EncodeToPNG();
        File.WriteAllBytes($"{folderPath}/TextureAtlas.png", atlasBytes);
        AssetDatabase.Refresh();
    }
}
#endif