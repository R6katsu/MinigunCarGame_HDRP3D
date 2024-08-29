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
        // �e�N�X�`�����擾����t�H���_�̃p�X���w��
        string folderPath = "Assets/Texture/Block";
        string[] texturePaths = Directory.GetFiles(folderPath, "*.png");

        // �ǂݍ��񂾃e�N�X�`�������X�g�Ɋi�[
        Texture2D[] textures = new Texture2D[texturePaths.Length];
        for (int i = 0; i < texturePaths.Length; i++)
        {
            textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePaths[i]);
        }

        // �e�N�X�`���̐��Ɋ�Â��Đ����`�O���b�h�̃T�C�Y���v�Z
        int textureCount = textures.Length;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(textureCount));
        int textureSize = textures[0].width; // �S�Ẵe�N�X�`���������T�C�Y�ł���Ɖ���

        // �A�g���X�̃T�C�Y������
        int atlasSize = gridSize * textureSize;
        Texture2D textureAtlas = new Texture2D(atlasSize, atlasSize);

        // �蓮�Ńe�N�X�`�����O���b�h�ɔz�u
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;
                if (index >= textureCount) break;

                // �e�N�X�`����z�u
                Texture2D texture = textures[index];
                textureAtlas.SetPixels(x * textureSize, (gridSize - y - 1) * textureSize, textureSize, textureSize, texture.GetPixels());
            }
        }

        textureAtlas.Apply();

        // �e�N�X�`���A�g���X��ۑ�
        byte[] atlasBytes = textureAtlas.EncodeToPNG();
        File.WriteAllBytes($"{folderPath}/TextureAtlas.png", atlasBytes);
        AssetDatabase.Refresh();
    }
}
#endif