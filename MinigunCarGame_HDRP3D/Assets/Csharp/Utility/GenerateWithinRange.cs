using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GenerateWithinRange
{
    /// <summary>
    /// ランダムな方向にnM離れた座標にランダムに生成する
    /// </summary>
    /// <param name="center">中心</param>
    /// <param name="distance">生成する距離</param>
    /// <param name="generateObject">生成するオブジェクト</param>
    /// <returns></returns>
    static public GameObject GenerateAtRandomDistance(GameObject generateObject, Vector3 center, float distance)
    {
        // ランダムな方向にランダムな距離だけ離れた場所を計算
        Vector3 spawnPosition = GetSpawnPosition(center, distance, distance);

        GameObject result = GameObject.Instantiate(generateObject, spawnPosition, Quaternion.identity);
        return result;
    }

    /// <summary>
    /// nMからmM以内の座標にランダムに生成する。
    /// または、nM以内の座標にランダムに生成する。
    /// </summary>
    /// <param name="generateObject">生成するオブジェクト</param>
    /// <param name="center">中心</param>
    /// <param name="maxRange">生成する最大距離</param>
    /// <param name="minRange">生成する最小距離</param>
    /// <returns></returns>
    static public GameObject GenerateAtRandomDistance(GameObject generateObject, Vector3 center, float maxRange, float minRange = 0.0f)
    {
        // ランダムな方向にランダムな距離だけ離れた場所を計算
        Vector3 spawnPosition = GetSpawnPosition(center, minRange, maxRange);

        GameObject result = GameObject.Instantiate(generateObject, spawnPosition, Quaternion.identity);
        return result;
    }

    /// <summary>
    /// ランダムな方向にランダムな距離だけ離れた場所を計算
    /// </summary>
    /// <param name="center">中心</param>
    /// <param name="minRange">最小距離</param>
    /// <param name="maxRange">最大距離</param>
    /// <returns></returns>
    static public Vector3 GetSpawnPosition(Vector3 center, float minRange, float maxRange)
    {
        // 円周のランダムな位置を計算
        float theta = Random.Range(0f, Mathf.PI * 2);   // 0度から360度までのランダムな角度
        float x = Mathf.Cos(theta);                     // Cos値を計算（余弦）
        float y = 0.0f;                                 // 高さを定義
        float z = Mathf.Sin(theta);                     // Sin値を計算（正弦）
        Vector3 randomDirection = new Vector3(x, y, z);

        // min, maxRangeが同等の場合、ランダムな距離を取得せずに結果を出す
        if (minRange == maxRange) { return center + randomDirection * minRange; }

        float randomDistance = Random.Range(minRange, maxRange);    // ランダムな距離を取得

        // ランダムな方向にランダムな距離だけ離れた場所を計算
        return center + randomDirection * randomDistance;
    }
}
