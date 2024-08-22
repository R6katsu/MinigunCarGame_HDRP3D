using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブロックの種類
/// </summary>
static public class BlockType
{
    /// <summary>
    /// ブロックの名前の種類
    /// </summary>
    public enum BlockNameType
    {
        [Tooltip("空気")] Air,
        [Tooltip("草の生えた土")] GrassDirt,
        [Tooltip("土")] Dirt,
        [Tooltip("ガラス")] Glass,
    }

    /// <summary>
    /// ブロックの振る舞いの種類
    /// </summary>
    public enum BlockBehaviourType
    {
        [Tooltip("なにもない")] None,
        [Tooltip("気体")] Gas,
        [Tooltip("固体")] Solid,
        [Tooltip("液体")] Liquid,
        [Tooltip("透過固体")] TransparentSolid,
    }

    // ブロックと振る舞いの辞書
    static private readonly Dictionary<BlockNameType, BlockBehaviourType> _blockBehaviourTypes = new()
    {
        { BlockNameType.Air, BlockBehaviourType.Gas },
        { BlockNameType.GrassDirt, BlockBehaviourType.Solid },
        { BlockNameType.Dirt, BlockBehaviourType.Solid },
        { BlockNameType.Glass, BlockBehaviourType.TransparentSolid },
    };

    // ブロックとタイル位置の辞書
    static private readonly Dictionary<BlockNameType, Vector2> _blockTilePositions = new()
    {
        { BlockNameType.GrassDirt, new Vector2(4, 0) },
        { BlockNameType.Dirt, new Vector2(4, 1) },
        { BlockNameType.Glass, new Vector2(4, 2) },
    };

    /// <summary>
    /// ブロックの振る舞いを取得する
    /// </summary>
    /// <param name="blockNameType">振る舞いを取得するブロック</param>
    /// <returns>ブロックの振る舞い</returns>
    static public BlockBehaviourType GetBlockBehaviourType(BlockNameType blockNameType)
    {
        // 辞書に含まれていないブロックだったら切り上げる
        if (!_blockBehaviourTypes.ContainsKey(blockNameType)) { return 0; }

        return _blockBehaviourTypes[blockNameType];
    }

    /// <summary>
    /// ブロックのタイル位置を取得する
    /// </summary>
    /// <param name="blockNameType">タイル位置を取得するブロック</param>
    /// <returns>ブロックのタイル位置</returns>
    static public Vector2 GetBlockTilePosition(BlockNameType blockNameType)
    {
        // 辞書に含まれていないブロックだったら切り上げる
        if (!_blockTilePositions.ContainsKey(blockNameType)) { return -Vector2.one; }

        return _blockTilePositions[blockNameType];
    }
}
