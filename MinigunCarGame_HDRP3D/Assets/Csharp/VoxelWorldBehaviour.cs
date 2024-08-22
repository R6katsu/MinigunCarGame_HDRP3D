using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using static BlockType;

/// <summary>
/// ボクセル世界
/// </summary>
public class VoxelWorldBehaviour : SingletonMonoBehaviour<VoxelWorldBehaviour>
{
    /// <summary>
    /// VoxelWorldの座標系
    /// </summary>
    public enum CoordinateSystem
    {
        [Tooltip("ワールド座標系")] World,
        [Tooltip("ローカル座標系")] Local
    }

    #region Field
    private const int DEFAULT_CHUNK_SIZE = 16;
    private const int DEFAULT_CHUNK_HEIGHT = 100;

    [SerializeField, Header("チャンクの広さ")]
    private int _chunkSize = DEFAULT_CHUNK_SIZE;

    [SerializeField, Header("チャンクの高さ")]
    private int _chunkHeight = DEFAULT_CHUNK_HEIGHT;

    [SerializeField, Header("当VoxelWorldが属する座標系")]
    private CoordinateSystem _coordinateSystem = 0;

    [SerializeField, Min(0), Header("描画するチャンクの距離")]
    private int _chunkDrawingRange = 0;

    [SerializeField, Header("チャンクを生成する為に必要なPrefab")]
    private Transform _chunkPrefab = null;

    [SerializeField]
    private Transform _playerTransform = null;      // 生成したPlayerを代入する

    private Dictionary<Vector3Int,ChunkInfo> _chunkInfos = new();           // チャンク情報のリスト
    public Dictionary<Vector3Int,ChunkRenderer> _drawingChunks = new();    // 現在描写中のチャンクのリスト

    private Vector3Int previousPlayerWorldCoord = Vector3Int.zero;          // 前回Playerが居たチャンク
    private bool isInitialization = false;
    private bool isUpdateChunk = false;
    #endregion

    private IEnumerator Start()
    {
        isInitialization = true;

        // Playerを中心とした描写範囲内のチャンクの座標をリストで取得
        List<Vector3Int> chunkCoords = GetChunkPositionsAroundPlayer();

        // 描写範囲内にチャンク座標の全てにチャンクを生成する
        foreach (var chunkCoord in chunkCoords)
        {
            // 生成したチャンク内の全てのブロックを置き換える
            yield return StartCoroutine(TestBlockArrayCreate(chunkCoord));
        }

        ChunkMeshData[] chunkMeshDatas = new ChunkMeshData[chunkCoords.Count];
        // 描写範囲内にチャンク全てのチャンクを探索
        for (int i = 0; i < chunkCoords.Count; i++)
        {
            // チャンク内のブロックのMeshを作成する
            ChunkInfo currentChunk = _chunkInfos[chunkCoords[i]];   // 現在のチャンクを取得

            // 既に生成済のチャンクだったらコンテニュー
            if (_drawingChunks.ContainsKey(currentChunk.worldCoord)) { continue; }

            ChunkMeshData chunkMeshData = new ChunkMeshData(true);
            for (int x = 0; x < currentChunk.BlocksXLength; x++)
            {
                for (int y = 0; y < currentChunk.BlocksYLength; y++)
                {
                    for (int z = 0; z < currentChunk.BlocksZLength; z++)
                    {
                        Vector3Int worldCoord = currentChunk.worldCoord + new Vector3Int(x, y, z);
                        chunkMeshData = GetMeshData(currentChunk, worldCoord, chunkMeshData, currentChunk.CloneBlocks[x, y, z]);
                    }
                }
            }

            // 作成したチャンクのMesh情報を使ってチャンクをObjcetとして生成する
            GameObject chunkObject = Instantiate(_chunkPrefab).gameObject;
            chunkObject.transform.position = currentChunk.worldCoord;
            _drawingChunks.Add(currentChunk.worldCoord, chunkObject.GetComponent<ChunkRenderer>());

            ChunkRenderer currentDrawingChunk = _drawingChunks[currentChunk.worldCoord];

            currentDrawingChunk.InitializeChunk(currentChunk);
            currentDrawingChunk.UpdateChunk(chunkMeshData);

            yield return null;
        }

        isInitialization = false;
    }

    private void Update()
    {
        if (_playerTransform == null || isInitialization) { return; }

        Vector3Int playerPosition = Vector3Int.zero;
        playerPosition.x = Mathf.RoundToInt(_playerTransform.position.x);
        playerPosition.y = Mathf.RoundToInt(_playerTransform.position.y);
        playerPosition.z = Mathf.RoundToInt(_playerTransform.position.z);

        Vector3Int curretPlayerWorldCoord = GetChunkCoord(playerPosition);

        // 前回と今回でPlayerの居るチャンクが変わっていた
        if (!isUpdateChunk && previousPlayerWorldCoord != curretPlayerWorldCoord)
        {
            previousPlayerWorldCoord = curretPlayerWorldCoord;

            // Playerを中心とした描写範囲に変化があったら描写チャンクを更新する
            // 既に呼ばれている時にも呼ばれるとチャンクが増殖する
            StartCoroutine(UpdateChunk());
        }
    }

    private IEnumerator UpdateChunk()
    {
        isUpdateChunk = true;

        // Playerを中心とした描写範囲内のチャンクの座標をリストで取得
        List<Vector3Int> chunkCoords = GetChunkPositionsAroundPlayer();

        yield return null;

        Dictionary<Vector3Int, ChunkRenderer> drawingChunks = new();    // 描写範囲内のチャンク
        List<Vector3Int> newDrawingCoords = new();                      // 新しく描写範囲内になった座標
        List<ChunkRenderer> outRangeChunks = new();                     // 範囲外になったチャンク

        // 
        foreach (var chunkCoord in chunkCoords)
        {
            // 既に含んで居たら何もせずコンテニュー
            if (_drawingChunks.ContainsKey(chunkCoord))
            {
                // 次回も引き続き描写するチャンクを辞書に追加
                drawingChunks.Add(chunkCoord, _drawingChunks[chunkCoord]);

                // 一度削除し、描写範囲外になったチャンクだけが残るようにする
                _drawingChunks.Remove(chunkCoord);
            }
            else
            {
                if (newDrawingCoords.Contains(chunkCoord)) { continue; }

                // まだ辞書にない座標が描写範囲内になったので保持しておく
                newDrawingCoords.Add(chunkCoord);
            }
        }

        outRangeChunks = _drawingChunks.Values.ToList();    // 範囲外になったチャンクを代入
        _drawingChunks = drawingChunks;                     // 引き続き範囲内だったチャンクを元の辞書に戻す

        // 描写範囲外になったチャンクを初期化する
        for (int i = 0; i< newDrawingCoords.Count; i++)
        {
            Vector3Int currentNewDrawingCoord = newDrawingCoords[i];

            // 生成したチャンク内の全てのブロックを置き換える
            // 既に辞書に含まれている座標だったら何もせずに切り上げられる
            yield return StartCoroutine(TestBlockArrayCreate(currentNewDrawingCoord));

            // チャンク内のブロックのMeshを作成する
            ChunkInfo currentChunk = _chunkInfos[currentNewDrawingCoord];   // 現在のチャンクを取得
            ChunkMeshData chunkMeshData = new ChunkMeshData(true);
            for (int x = 0; x < currentChunk.BlocksXLength; x++)
            {
                for (int y = 0; y < currentChunk.BlocksYLength; y++)
                {
                    for (int z = 0; z < currentChunk.BlocksZLength; z++)
                    {
                        Vector3Int worldCoord = currentChunk.worldCoord + new Vector3Int(x, y, z);
                        chunkMeshData = GetMeshData(currentChunk, worldCoord, chunkMeshData, currentChunk.CloneBlocks[x, y, z]);
                    }
                }
            }

            yield return null;

            // 現在存在しているChunkRendererだけでは足りない場合、新たに生成する
            if (outRangeChunks.Count <= i) { continue; }

            ChunkRenderer currentOutRangeChunks = outRangeChunks[i];

            // 新しく描写範囲内になった座標のChunkInfoを渡す
            currentOutRangeChunks.InitializeChunk(currentChunk);

            // 実際の座標も変更する
            currentOutRangeChunks.transform.position = currentChunk.worldCoord;

            currentOutRangeChunks.UpdateChunk(chunkMeshData);

            // 既に生成済のチャンクだった
            if (_drawingChunks.ContainsKey(currentNewDrawingCoord))
            {
                _drawingChunks[currentNewDrawingCoord] = currentOutRangeChunks;
            }
            else
            {
                // 初期化したChunkRendererを描写範囲内のチャンクとして辞書に追加する
                _drawingChunks.Add(currentNewDrawingCoord, currentOutRangeChunks);
            }
        }

        isUpdateChunk = false;
    }

    private IEnumerator TestBlockArrayCreate(Vector3Int chunkCoord)
    {
        // 既に辞書に含まれている座標なら処理を切り上げる
        if (_chunkInfos.ContainsKey(chunkCoord)) { yield break; }

        var chunkInfo = new ChunkInfo(_chunkSize, _chunkHeight, chunkCoord);
        _chunkInfos.Add(chunkCoord, chunkInfo);

        var currentChunk = _chunkInfos[chunkCoord];
        var blocks = new BlockNameType[currentChunk.BlocksXLength, currentChunk.BlocksYLength, currentChunk.BlocksZLength];

        for (int x = 0; x < currentChunk.BlocksXLength; x++)
        {
            for (int z = 0; z < currentChunk.BlocksZLength; z++)
            {
                blocks[x, 0, z] = BlockNameType.GrassDirt;
            }
        }

        yield return null;

        // チャンク内の全てのブロックを置き換える
        currentChunk.SetAllBlock(blocks);
    }

    /// <summary>
    /// 指定した座標を含むチャンクを取得する
    /// </summary>
    /// <param name="blockWorldCoord">取得したい座標を指定</param>
    /// <returns>指定した座標を含むチャンク</returns>
    public ChunkInfo GetChunk(Vector3Int blockWorldCoord)
    {
        // ワールド座標をチャンクサイズで割り、チャンク座標を計算
        int chunkX = Mathf.FloorToInt((float)blockWorldCoord.x / _chunkSize) * _chunkSize;
        int chunkZ = Mathf.FloorToInt((float)blockWorldCoord.z / _chunkSize) * _chunkSize;

        // チャンク座標をVector3Intで表現
        Vector3Int chunkCoord = new Vector3Int(chunkX, 0, chunkZ);

        // 指定した座標のチャンクが辞書に含まれていなかった
        if (!_chunkInfos.ContainsKey(chunkCoord))
        {
            // 該当するチャンクが存在しない場合は新たに生成する
            //_chunkInfos[chunkCoord] = new ChunkInfo(_chunkSize, _chunkHeight, this, chunkCoord);
            //Debug.Log("該当するチャンクが存在しない");
            return null;
        }

        return _chunkInfos[chunkCoord];
    }

    /// <summary>
    /// 指定した座標を含むチャンクの座標を取得する
    /// </summary>
    /// <param name="worldCoord">取得したい座標を指定</param>
    /// <returns>指定した座標を含むチャンクの座標</returns>
    public Vector3Int GetChunkCoord(Vector3Int worldCoord)
    {
        // ワールド座標をチャンクサイズで割り、チャンク座標を計算
        int chunkX = Mathf.FloorToInt((float)worldCoord.x / _chunkSize) * _chunkSize;
        int chunkZ = Mathf.FloorToInt((float)worldCoord.z / _chunkSize) * _chunkSize;

        // チャンク座標をVector3Intで表現
        Vector3Int chunkCoord = new Vector3Int(chunkX, 0, chunkZ);

        return chunkCoord;
    }

    /// <summary>
    /// 指定された座標のブロックを取得
    /// </summary>
    /// <param name="worldCoord">取得したい座標を指定</param>
    /// <returns>指定された座標のブロック</returns>
    public BlockNameType GetBlock(Vector3Int worldCoord)
    {
        return GetChunk(worldCoord).GetBlock(worldCoord);
    }

    /// <summary>
    /// 指定された座標のブロックを置き換える
    /// </summary>
    /// <param name="worldCoord">取得したい座標を指定</param>
    /// <param name="block">置き換えるブロック</param>
    public void SetBlock(Vector3Int worldCoord, BlockNameType block)
    {
        // 変更があった場合、Meshに反映させる

        GetChunk(worldCoord).SetBlock(worldCoord, block);
    }

    /// <summary>
    /// プレイヤーを中心とした描写範囲内のチャンクの座標をリスト化して取得
    /// </summary>
    /// <returns>プレイヤーを中心とした描写範囲内のチャンクの座標のリスト</returns>
    private List<Vector3Int> GetChunkPositionsAroundPlayer()
    {
        Vector3Int playerPosition = Vector3Int.zero;
        playerPosition.x = Mathf.RoundToInt(_playerTransform.position.x);
        playerPosition.y = Mathf.RoundToInt(_playerTransform.position.y);
        playerPosition.z = Mathf.RoundToInt(_playerTransform.position.z);

        int startX = playerPosition.x - (_chunkDrawingRange) * _chunkSize;
        int startZ = playerPosition.z - (_chunkDrawingRange) * _chunkSize;
        int endX = playerPosition.x + (_chunkDrawingRange) * _chunkSize;
        int endZ = playerPosition.z + (_chunkDrawingRange) * _chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += _chunkSize)
        {
            for (int z = startZ; z <= endZ; z += _chunkSize)
            {
                // ブロックが所属するチャンクを求める
                Vector3Int chunkPos = GetChunkCoord(new Vector3Int(x, 0, z));

                // 既に含まれている座標ならコンテニュー
                if (chunkPositionsToCreate.Contains(chunkPos)) { continue; }

                chunkPositionsToCreate.Add(chunkPos);
                if (x >= playerPosition.x - _chunkSize
                    && x <= playerPosition.x + _chunkSize
                    && z >= playerPosition.z - _chunkSize
                    && z <= playerPosition.z + _chunkSize)
                {
                    for (int y = -_chunkHeight; y >= playerPosition.y - _chunkHeight * 2; y -= _chunkHeight)
                    {
                        chunkPos = GetChunkCoord(new Vector3Int(x, y, z));

                        // 既に含まれている座標ならコンテニュー
                        if (chunkPositionsToCreate.Contains(chunkPos)) { continue; }

                        chunkPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }

        return chunkPositionsToCreate;
    }

    /// <summary>
    /// 指定された位置のブロックに基づいてメッシュデータを取得する。
    /// </summary>
    /// <param name="chunk">現在のチャンクデータ。</param>
    /// <param name="worldCoord">ブロックのワールド座標。</param>
    /// <param name="meshData">生成されるメッシュデータ。</param>
    /// <param name="blockType">処理するブロックの種類。</param>
    /// <returns>更新されたメッシュデータ。</returns>
    public ChunkMeshData GetMeshData(ChunkInfo chunk, Vector3Int worldCoord, ChunkMeshData meshData, BlockNameType blockType)
    {
        // 空気または無効なブロックタイプの場合はそのまま返す
        if (BlockBehaviourType.Gas == GetBlockBehaviourType(blockType)) { return meshData; }

        Direction[] directions =
        {
            Direction.backwards,
            Direction.down,
            Direction.foreward,
            Direction.left,
            Direction.right,
            Direction.up
        };

        // 各方向に対して処理を行う
        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = worldCoord + direction.GetVector();     // ワールド座標に隣接座標を足す

            if (GetChunk(neighbourBlockCoordinates) == null) { continue; }

            var neighbourBlockType = GetBlock(neighbourBlockCoordinates);           // BlockType取得      // もしかすると端の隣接？

            // 隣接ブロックが気体/液体/透過固体のいずれかであればMeshに追加
            switch (GetBlockBehaviourType(neighbourBlockType))
            {
                case BlockBehaviourType.Gas:
                case BlockBehaviourType.Liquid:
                case BlockBehaviourType.TransparentSolid:
                    //Mesh追加対象が液体だった
                    if (GetBlockBehaviourType(blockType) == BlockBehaviourType.Liquid)
                    {
                        // 隣接ブロックが気体/透過固体のいずれかであれば液体Meshを追加
                        switch (GetBlockBehaviourType(neighbourBlockType))
                        {
                            case BlockBehaviourType.Gas:
                            case BlockBehaviourType.TransparentSolid:
                                // ローカル座標
                                Vector3Int localCoord = worldCoord - chunk.worldCoord;

                                // 面の頂点を取得
                                ChunkMeshData.GetFaceVertices(direction, localCoord.x, localCoord.y, localCoord.z, meshData.waterMesh, blockType);

                                bool generatesCollider;
                                // コライダーを生成する場合の三角形を追加
                                switch (GetBlockBehaviourType(blockType))
                                {
                                    case BlockBehaviourType.Gas:
                                    case BlockBehaviourType.Liquid:
                                        generatesCollider = false;      // 気体/液体の場合はColliderを追加しない
                                        break;

                                    default:
                                        generatesCollider = true;       // 気体/液体以外の場合はColliderを追加する
                                        break;
                                }
                                meshData.waterMesh.AddQuadTriangles(generatesCollider);

                                // UV座標を追加
                                meshData.waterMesh.uv.AddRange(FaceUVs(direction, blockType));
                                Debug.Log("UV座標追加はコメントアウト中");
                                break;
                        }
                    }
                    else
                    {
                        // その他のブロックタイプの場合、メッシュデータに追加

                        // ローカル座標
                        Vector3Int localCoord = worldCoord - chunk.worldCoord;

                        // 面の頂点を取得
                        ChunkMeshData.GetFaceVertices(direction, localCoord.x, localCoord.y, localCoord.z, meshData, blockType);

                        bool generatesCollider;
                        // コライダーを生成する場合の三角形を追加
                        switch (GetBlockBehaviourType(blockType))
                        {
                            case BlockBehaviourType.Gas:
                            case BlockBehaviourType.Liquid:
                                generatesCollider = false;      // 気体/液体の場合はColliderを追加しない
                                break;

                            default:
                                generatesCollider = true;       // 気体/液体以外の場合はColliderを追加する
                                break;
                        }
                        meshData.AddQuadTriangles(generatesCollider);

                        // UV座標を追加
                        meshData.uv.AddRange(FaceUVs(direction, blockType));
                    }
                    break;
            }
        }

        return meshData;
    }

    /// <summary>
    /// 指定された方向とブロックタイプに基づいて UV 座標を取得する。
    /// </summary>
    /// <param name="direction">面の方向</param>
    /// <param name="blockType">処理するブロックの種類</param>
    /// <returns>UV 座標の配列</returns>
    public Vector2[] FaceUVs(Direction direction, BlockNameType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = BlockType.GetBlockTilePosition(blockType);
        float tileSizeX = 1.0f/5.0f;
        float tileSizeY = 1.0f/5.0f;
        float textureOffset = 0.015f;

        UVs[0] = new Vector2(tileSizeX * tilePos.x + tileSizeX - textureOffset,
            tileSizeY * tilePos.y + textureOffset);

        UVs[1] = new Vector2(tileSizeX * tilePos.x + tileSizeX - textureOffset,
            tileSizeY * tilePos.y + tileSizeY - textureOffset);

        UVs[2] = new Vector2(tileSizeX * tilePos.x + textureOffset,
            tileSizeY * tilePos.y + tileSizeY - textureOffset);

        UVs[3] = new Vector2(tileSizeX * tilePos.x + textureOffset,
            tileSizeY * tilePos.y + textureOffset);

        return UVs;
    }
}
