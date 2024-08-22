using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockType;

public class BlockAction
{
    /// <summary>
    /// ブロックの破壊
    /// </summary>
    /// <param name="hit">プレイヤーからブロックに向けてにRay</param>
    /// <param name="sourcePosition">Rayの発射位置</param>
    static public void BreakBlock(RaycastHit hit, Vector3 sourcePosition)
    {
        float rayLengthening = 0.1f;    // Rayをブロック方向に延長

        Vector3 pos_obj = hit.point - hit.normal * rayLengthening;
        Vector3Int hitWorldCoord = new Vector3Int
        (
            Mathf.RoundToInt(pos_obj.x),
            Mathf.RoundToInt(pos_obj.y),
            Mathf.RoundToInt(pos_obj.z)
        );

        VoxelWorldBehaviour.Instance.SetBlock(hitWorldCoord, (BlockNameType)0);
    }

    /// <summary>
    /// ブロックの設置
    /// </summary>
    /// <param name="hitPoint">プレイヤーからブロックに向けてのRay</param>
    static public void PlacementBlock(Vector3Int hitPoint, BlockNameType blockNameType)
    {
        VoxelWorldBehaviour.Instance.SetBlock(hitPoint, blockNameType);
    }
}
