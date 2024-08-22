using UnityEngine;
using Unity.AI.Navigation;
using static BlockType;

/// <summary>
/// チャンク内にあるブロック、チャンクの大きさ、座標などの情報を持つclass。<br/>
/// ブロックの多次元配列に対する関数などを含んでいる。
/// </summary>
public class ChunkInfo
{
    [Tooltip("当チャンクのワールド座標")]
    public readonly Vector3Int worldCoord = Vector3Int.zero;

    // チャンク内のブロックの種類を保持する配列
    private BlockNameType[,,] _blocks = new BlockNameType[0,0,0];

    /// <summary>
    /// 書き換えても反映されないクローンのブロック配列
    /// </summary>
    public BlockNameType[,,] CloneBlocks => (BlockNameType[,,])_blocks.Clone();

    /// <summary>
    /// チャンク内のブロックの総数
    /// </summary>
    public int BlocksLength => _blocks.Length;

    /// <summary>
    /// チャンク内のブロックの x軸の長さ
    /// </summary>
    public int BlocksXLength => _blocks.GetLength(0);

    /// <summary>
    /// チャンク内のブロックの y軸の長さ
    /// </summary>
    public int BlocksYLength => _blocks.GetLength(1);

    /// <summary>
    /// チャンク内のブロックの z軸の長さ
    /// </summary>
    public int BlocksZLength => _blocks.GetLength(2);

    /// <summary>
    /// コンストラクタ。
    /// 新しいチャンク
    /// </summary>
    /// <param name="chunkSize">チャンクの広さ</param>
    /// <param name="chunkHeight">チャンクの高さ</param>
    /// <param name="world">当チャンクが属するボクセル世界</param>
    /// <param name="worldCoord">当チャンクのワールド座標</param>
    public ChunkInfo(int chunkSize, int chunkHeight, Vector3Int worldCoord)
    {
        // チャンクのワールド内での位置を設定
        this.worldCoord = worldCoord;

        // チャンク内のブロック配列を初期化
        _blocks = new BlockNameType[chunkSize, chunkHeight, chunkSize];
    }

    /// <summary>
    /// 全てのブロック入れ替え
    /// </summary>
    /// <param name="blocks">代入するブロックの配列</param>
    public void SetAllBlock(BlockNameType[,,] blocks)
    {
        bool isXLengthEqual = blocks.GetLength(0) == _blocks.GetLength(0);
        bool isYLengthEqual = blocks.GetLength(1) == _blocks.GetLength(1);
        bool isZLengthEqual = blocks.GetLength(2) == _blocks.GetLength(2);

        // 配列の大きさが違っていた
        if (!isXLengthEqual || !isYLengthEqual || !isZLengthEqual) { return; }

        _blocks = blocks;
    }

    /// <summary>
    /// ブロック入れ替え
    /// </summary>
    /// <param name="worldCoord">入れ替え先のワールド座標</param>
    /// <param name="block">入れ替えるブロック</param>
    public void SetBlock(Vector3Int worldCoord, BlockNameType block)
    {
        if (!ConvertLocalCoordBlock(worldCoord)) { return; }

        Vector3Int localCoord = worldCoord - this.worldCoord;

        _blocks[localCoord.x, localCoord.y, localCoord.z] = block;

        // ブロックへの変更をMeshに反映させる
        ChunkMeshData chunkMeshData = new ChunkMeshData(true);
        for (int x = 0; x < BlocksXLength; x++)
        {
            for (int y = 0; y < BlocksYLength; y++)
            {
                for (int z = 0; z < BlocksZLength; z++)
                {
                    Vector3Int currentWorldCoord = this.worldCoord + new Vector3Int(x, y, z);
                    chunkMeshData = VoxelWorldBehaviour.Instance.GetMeshData(this, currentWorldCoord, chunkMeshData, CloneBlocks[x, y, z]);
                }
            }
        }

        ChunkRenderer chunkRenderer = VoxelWorldBehaviour.Instance._drawingChunks[this.worldCoord];

        chunkRenderer.UpdateChunk(chunkMeshData);
    }

    /// <summary>
    /// ブロック取得
    /// </summary>
    /// <param name="worldCoord">取得先のワールド座標</param>
    public BlockNameType GetBlock(Vector3Int worldCoord)
    {
        if (!ConvertLocalCoordBlock(worldCoord))
        {
            // 範囲外だったら、適切なチャンクを取得して再度GetBlockを呼び出す
            //return VoxelWorldBehaviour.Instance.GetBlock(worldCoord);
            //Debug.Log("-1になったりしたので、とりあえず範囲外だったら 0を返すようにした");
            return 0;
        }

        Vector3Int localCoord = worldCoord - this.worldCoord;

        return _blocks[localCoord.x, localCoord.y, localCoord.z];
    }

    /// <summary>
    /// 指定のワールド座標がチャンク範囲内か判定
    /// </summary>
    /// <param name="worldCoord">範囲内か判定するワールド座標</param>
    /// <returns>指定のワールド座標がチャンク範囲内だったらtrue</returns>
    public bool ConvertLocalCoordBlock(Vector3Int worldCoord)
    {
        Vector3Int localCoord = worldCoord - this.worldCoord;

        // チャンクの範囲内か
        bool isXContains = localCoord.x >= 0 && localCoord.x < BlocksXLength;
        bool isYContains = localCoord.y >= 0 && localCoord.y < BlocksYLength;
        bool isZContains = localCoord.z >= 0 && localCoord.z < BlocksZLength;

        // worldCoordBlockがチャンクの範囲内か判定
        return isXContains && isYContains && isZContains;
    }
}