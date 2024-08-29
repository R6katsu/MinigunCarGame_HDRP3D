using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class TerrainChange
{
    static public void ModifyTerrain(Terrain terrain, Vector3 position, float radius, float depth)
    {
        TerrainData terrainData = terrain.terrainData;

        // ���[���h���W��Terrain�̃��[�J�����W�ɕϊ�
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(position);

        // �e���C���̍����}�b�v�̉𑜓x
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // �e���C���̃T�C�Y
        Vector3 terrainSize = terrainData.size;

        // ���[���h���W�n��radius�����[�J�����W�n�ɕϊ�
        float radiusInHeightmapSpaceX = radius / terrainSize.x * heightmapWidth;
        float radiusInHeightmapSpaceY = radius / terrainSize.z * heightmapHeight;

        // ���W�������}�b�v�̃X�P�[���ɕϊ�
        int xStart = Mathf.RoundToInt((terrainPos.x - radiusInHeightmapSpaceX) / terrainSize.x * heightmapWidth);
        int xEnd = Mathf.RoundToInt((terrainPos.x + radiusInHeightmapSpaceX) / terrainSize.x * heightmapWidth);
        int yStart = Mathf.RoundToInt((terrainPos.z - radiusInHeightmapSpaceY) / terrainSize.z * heightmapHeight);
        int yEnd = Mathf.RoundToInt((terrainPos.z + radiusInHeightmapSpaceY) / terrainSize.z * heightmapHeight);

        // �͈̓`�F�b�N
        xStart = Mathf.Clamp(xStart, 0, heightmapWidth);
        xEnd = Mathf.Clamp(xEnd, 0, heightmapWidth);
        yStart = Mathf.Clamp(yStart, 0, heightmapHeight);
        yEnd = Mathf.Clamp(yEnd, 0, heightmapHeight);

        // ���݂̍������擾
        float[,] heights = terrainData.GetHeights(xStart, yStart, xEnd - xStart, yEnd - yStart);

        // ������ύX
        for (int y = 0; y < yEnd - yStart; y++)
        {
            for (int x = 0; x < xEnd - xStart; x++)
            {
                // �e���C�����̌��݈ʒu���璆�S�ւ̑��΍��W���v�Z
                float xPos = xStart + x - (terrainPos.x / terrainSize.x * heightmapWidth);
                float yPos = yStart + y - (terrainPos.z / terrainSize.z * heightmapHeight);

                // �������v�Z
                float distance = Mathf.Sqrt(xPos * xPos + yPos * yPos);

                // �K�E�X�֐����g�p���Ċ��炩�ɉ��݂𐶐�
                if (distance < radiusInHeightmapSpaceX)
                {
                    float gaussian = Mathf.Exp(-Mathf.Pow(distance / radiusInHeightmapSpaceX, 2)); // �K�E�X�֐�
                    heights[y, x] -= gaussian * depth;
                }
            }
        }

        // �ύX��K�p
        terrainData.SetHeights(xStart, yStart, heights);
    }
}
