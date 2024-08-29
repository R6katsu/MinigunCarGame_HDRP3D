using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class TerrainChange
{
    static public void ModifyTerrain(Terrain terrain, Vector3 position, float radius, float depth)
    {
        TerrainData terrainData = terrain.terrainData;

        // ワールド座標をTerrainのローカル座標に変換
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(position);

        // テレインの高さマップの解像度
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // テレインのサイズ
        Vector3 terrainSize = terrainData.size;

        // ワールド座標系のradiusをローカル座標系に変換
        float radiusInHeightmapSpaceX = radius / terrainSize.x * heightmapWidth;
        float radiusInHeightmapSpaceY = radius / terrainSize.z * heightmapHeight;

        // 座標を高さマップのスケールに変換
        int xStart = Mathf.RoundToInt((terrainPos.x - radiusInHeightmapSpaceX) / terrainSize.x * heightmapWidth);
        int xEnd = Mathf.RoundToInt((terrainPos.x + radiusInHeightmapSpaceX) / terrainSize.x * heightmapWidth);
        int yStart = Mathf.RoundToInt((terrainPos.z - radiusInHeightmapSpaceY) / terrainSize.z * heightmapHeight);
        int yEnd = Mathf.RoundToInt((terrainPos.z + radiusInHeightmapSpaceY) / terrainSize.z * heightmapHeight);

        // 範囲チェック
        xStart = Mathf.Clamp(xStart, 0, heightmapWidth);
        xEnd = Mathf.Clamp(xEnd, 0, heightmapWidth);
        yStart = Mathf.Clamp(yStart, 0, heightmapHeight);
        yEnd = Mathf.Clamp(yEnd, 0, heightmapHeight);

        // 現在の高さを取得
        float[,] heights = terrainData.GetHeights(xStart, yStart, xEnd - xStart, yEnd - yStart);

        // 高さを変更
        for (int y = 0; y < yEnd - yStart; y++)
        {
            for (int x = 0; x < xEnd - xStart; x++)
            {
                // テレイン内の現在位置から中心への相対座標を計算
                float xPos = xStart + x - (terrainPos.x / terrainSize.x * heightmapWidth);
                float yPos = yStart + y - (terrainPos.z / terrainSize.z * heightmapHeight);

                // 距離を計算
                float distance = Mathf.Sqrt(xPos * xPos + yPos * yPos);

                // ガウス関数を使用して滑らかに凹みを生成
                if (distance < radiusInHeightmapSpaceX)
                {
                    float gaussian = Mathf.Exp(-Mathf.Pow(distance / radiusInHeightmapSpaceX, 2)); // ガウス関数
                    heights[y, x] -= gaussian * depth;
                }
            }
        }

        // 変更を適用
        terrainData.SetHeights(xStart, yStart, heights);
    }
}
