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
        string folderPath = "Assets/Texture"; // �e�N�X�`�����ۑ�����Ă���t�H���_�̃p�X
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);

            if (textureImporter != null)
            {
                // �e�N�X�`���̌`����񈳏k�`���ɐݒ�
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.SaveAndReimport();
            }
        }
    }
}
#endif