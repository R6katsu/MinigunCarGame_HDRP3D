using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using static BlockType;

/// <summary>
/// �{�N�Z�����E
/// </summary>
public class VoxelWorldBehaviour : SingletonMonoBehaviour<VoxelWorldBehaviour>
{
    /// <summary>
    /// VoxelWorld�̍��W�n
    /// </summary>
    public enum CoordinateSystem
    {
        [Tooltip("���[���h���W�n")] World,
        [Tooltip("���[�J�����W�n")] Local
    }

    #region Field
    private const int DEFAULT_CHUNK_SIZE = 16;
    private const int DEFAULT_CHUNK_HEIGHT = 100;

    [SerializeField, Header("�`�����N�̍L��")]
    private int _chunkSize = DEFAULT_CHUNK_SIZE;

    [SerializeField, Header("�`�����N�̍���")]
    private int _chunkHeight = DEFAULT_CHUNK_HEIGHT;

    [SerializeField, Header("��VoxelWorld����������W�n")]
    private CoordinateSystem _coordinateSystem = 0;

    [SerializeField, Min(0), Header("�`�悷��`�����N�̋���")]
    private int _chunkDrawingRange = 0;

    [SerializeField, Header("�`�����N�𐶐�����ׂɕK�v��Prefab")]
    private Transform _chunkPrefab = null;

    [SerializeField]
    private Transform _playerTransform = null;      // ��������Player��������

    private Dictionary<Vector3Int,ChunkInfo> _chunkInfos = new();           // �`�����N���̃��X�g
    public Dictionary<Vector3Int,ChunkRenderer> _drawingChunks = new();    // ���ݕ`�ʒ��̃`�����N�̃��X�g

    private Vector3Int previousPlayerWorldCoord = Vector3Int.zero;          // �O��Player�������`�����N
    private bool isInitialization = false;
    private bool isUpdateChunk = false;
    #endregion

    private IEnumerator Start()
    {
        isInitialization = true;

        // Player�𒆐S�Ƃ����`�ʔ͈͓��̃`�����N�̍��W�����X�g�Ŏ擾
        List<Vector3Int> chunkCoords = GetChunkPositionsAroundPlayer();

        // �`�ʔ͈͓��Ƀ`�����N���W�̑S�ĂɃ`�����N�𐶐�����
        foreach (var chunkCoord in chunkCoords)
        {
            // ���������`�����N���̑S�Ẵu���b�N��u��������
            yield return StartCoroutine(TestBlockArrayCreate(chunkCoord));
        }

        ChunkMeshData[] chunkMeshDatas = new ChunkMeshData[chunkCoords.Count];
        // �`�ʔ͈͓��Ƀ`�����N�S�Ẵ`�����N��T��
        for (int i = 0; i < chunkCoords.Count; i++)
        {
            // �`�����N���̃u���b�N��Mesh���쐬����
            ChunkInfo currentChunk = _chunkInfos[chunkCoords[i]];   // ���݂̃`�����N���擾

            // ���ɐ����ς̃`�����N��������R���e�j���[
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

            // �쐬�����`�����N��Mesh�����g���ă`�����N��Objcet�Ƃ��Đ�������
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

        // �O��ƍ����Player�̋���`�����N���ς���Ă���
        if (!isUpdateChunk && previousPlayerWorldCoord != curretPlayerWorldCoord)
        {
            previousPlayerWorldCoord = curretPlayerWorldCoord;

            // Player�𒆐S�Ƃ����`�ʔ͈͂ɕω�����������`�ʃ`�����N���X�V����
            // ���ɌĂ΂�Ă��鎞�ɂ��Ă΂��ƃ`�����N�����B����
            StartCoroutine(UpdateChunk());
        }
    }

    private IEnumerator UpdateChunk()
    {
        isUpdateChunk = true;

        // Player�𒆐S�Ƃ����`�ʔ͈͓��̃`�����N�̍��W�����X�g�Ŏ擾
        List<Vector3Int> chunkCoords = GetChunkPositionsAroundPlayer();

        yield return null;

        Dictionary<Vector3Int, ChunkRenderer> drawingChunks = new();    // �`�ʔ͈͓��̃`�����N
        List<Vector3Int> newDrawingCoords = new();                      // �V�����`�ʔ͈͓��ɂȂ������W
        List<ChunkRenderer> outRangeChunks = new();                     // �͈͊O�ɂȂ����`�����N

        // 
        foreach (var chunkCoord in chunkCoords)
        {
            // ���Ɋ܂�ŋ����牽�������R���e�j���[
            if (_drawingChunks.ContainsKey(chunkCoord))
            {
                // ��������������`�ʂ���`�����N�������ɒǉ�
                drawingChunks.Add(chunkCoord, _drawingChunks[chunkCoord]);

                // ��x�폜���A�`�ʔ͈͊O�ɂȂ����`�����N�������c��悤�ɂ���
                _drawingChunks.Remove(chunkCoord);
            }
            else
            {
                if (newDrawingCoords.Contains(chunkCoord)) { continue; }

                // �܂������ɂȂ����W���`�ʔ͈͓��ɂȂ����̂ŕێ����Ă���
                newDrawingCoords.Add(chunkCoord);
            }
        }

        outRangeChunks = _drawingChunks.Values.ToList();    // �͈͊O�ɂȂ����`�����N����
        _drawingChunks = drawingChunks;                     // ���������͈͓��������`�����N�����̎����ɖ߂�

        // �`�ʔ͈͊O�ɂȂ����`�����N������������
        for (int i = 0; i< newDrawingCoords.Count; i++)
        {
            Vector3Int currentNewDrawingCoord = newDrawingCoords[i];

            // ���������`�����N���̑S�Ẵu���b�N��u��������
            // ���Ɏ����Ɋ܂܂�Ă�����W�������牽�������ɐ؂�グ����
            yield return StartCoroutine(TestBlockArrayCreate(currentNewDrawingCoord));

            // �`�����N���̃u���b�N��Mesh���쐬����
            ChunkInfo currentChunk = _chunkInfos[currentNewDrawingCoord];   // ���݂̃`�����N���擾
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

            // ���ݑ��݂��Ă���ChunkRenderer�����ł͑���Ȃ��ꍇ�A�V���ɐ�������
            if (outRangeChunks.Count <= i) { continue; }

            ChunkRenderer currentOutRangeChunks = outRangeChunks[i];

            // �V�����`�ʔ͈͓��ɂȂ������W��ChunkInfo��n��
            currentOutRangeChunks.InitializeChunk(currentChunk);

            // ���ۂ̍��W���ύX����
            currentOutRangeChunks.transform.position = currentChunk.worldCoord;

            currentOutRangeChunks.UpdateChunk(chunkMeshData);

            // ���ɐ����ς̃`�����N������
            if (_drawingChunks.ContainsKey(currentNewDrawingCoord))
            {
                _drawingChunks[currentNewDrawingCoord] = currentOutRangeChunks;
            }
            else
            {
                // ����������ChunkRenderer��`�ʔ͈͓��̃`�����N�Ƃ��Ď����ɒǉ�����
                _drawingChunks.Add(currentNewDrawingCoord, currentOutRangeChunks);
            }
        }

        isUpdateChunk = false;
    }

    private IEnumerator TestBlockArrayCreate(Vector3Int chunkCoord)
    {
        // ���Ɏ����Ɋ܂܂�Ă�����W�Ȃ珈����؂�グ��
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

        // �`�����N���̑S�Ẵu���b�N��u��������
        currentChunk.SetAllBlock(blocks);
    }

    /// <summary>
    /// �w�肵�����W���܂ރ`�����N���擾����
    /// </summary>
    /// <param name="blockWorldCoord">�擾���������W���w��</param>
    /// <returns>�w�肵�����W���܂ރ`�����N</returns>
    public ChunkInfo GetChunk(Vector3Int blockWorldCoord)
    {
        // ���[���h���W���`�����N�T�C�Y�Ŋ���A�`�����N���W���v�Z
        int chunkX = Mathf.FloorToInt((float)blockWorldCoord.x / _chunkSize) * _chunkSize;
        int chunkZ = Mathf.FloorToInt((float)blockWorldCoord.z / _chunkSize) * _chunkSize;

        // �`�����N���W��Vector3Int�ŕ\��
        Vector3Int chunkCoord = new Vector3Int(chunkX, 0, chunkZ);

        // �w�肵�����W�̃`�����N�������Ɋ܂܂�Ă��Ȃ�����
        if (!_chunkInfos.ContainsKey(chunkCoord))
        {
            // �Y������`�����N�����݂��Ȃ��ꍇ�͐V���ɐ�������
            //_chunkInfos[chunkCoord] = new ChunkInfo(_chunkSize, _chunkHeight, this, chunkCoord);
            //Debug.Log("�Y������`�����N�����݂��Ȃ�");
            return null;
        }

        return _chunkInfos[chunkCoord];
    }

    /// <summary>
    /// �w�肵�����W���܂ރ`�����N�̍��W���擾����
    /// </summary>
    /// <param name="worldCoord">�擾���������W���w��</param>
    /// <returns>�w�肵�����W���܂ރ`�����N�̍��W</returns>
    public Vector3Int GetChunkCoord(Vector3Int worldCoord)
    {
        // ���[���h���W���`�����N�T�C�Y�Ŋ���A�`�����N���W���v�Z
        int chunkX = Mathf.FloorToInt((float)worldCoord.x / _chunkSize) * _chunkSize;
        int chunkZ = Mathf.FloorToInt((float)worldCoord.z / _chunkSize) * _chunkSize;

        // �`�����N���W��Vector3Int�ŕ\��
        Vector3Int chunkCoord = new Vector3Int(chunkX, 0, chunkZ);

        return chunkCoord;
    }

    /// <summary>
    /// �w�肳�ꂽ���W�̃u���b�N���擾
    /// </summary>
    /// <param name="worldCoord">�擾���������W���w��</param>
    /// <returns>�w�肳�ꂽ���W�̃u���b�N</returns>
    public BlockNameType GetBlock(Vector3Int worldCoord)
    {
        return GetChunk(worldCoord).GetBlock(worldCoord);
    }

    /// <summary>
    /// �w�肳�ꂽ���W�̃u���b�N��u��������
    /// </summary>
    /// <param name="worldCoord">�擾���������W���w��</param>
    /// <param name="block">�u��������u���b�N</param>
    public void SetBlock(Vector3Int worldCoord, BlockNameType block)
    {
        // �ύX���������ꍇ�AMesh�ɔ��f������

        GetChunk(worldCoord).SetBlock(worldCoord, block);
    }

    /// <summary>
    /// �v���C���[�𒆐S�Ƃ����`�ʔ͈͓��̃`�����N�̍��W�����X�g�����Ď擾
    /// </summary>
    /// <returns>�v���C���[�𒆐S�Ƃ����`�ʔ͈͓��̃`�����N�̍��W�̃��X�g</returns>
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
                // �u���b�N����������`�����N�����߂�
                Vector3Int chunkPos = GetChunkCoord(new Vector3Int(x, 0, z));

                // ���Ɋ܂܂�Ă�����W�Ȃ�R���e�j���[
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

                        // ���Ɋ܂܂�Ă�����W�Ȃ�R���e�j���[
                        if (chunkPositionsToCreate.Contains(chunkPos)) { continue; }

                        chunkPositionsToCreate.Add(chunkPos);
                    }
                }
            }
        }

        return chunkPositionsToCreate;
    }

    /// <summary>
    /// �w�肳�ꂽ�ʒu�̃u���b�N�Ɋ�Â��ă��b�V���f�[�^���擾����B
    /// </summary>
    /// <param name="chunk">���݂̃`�����N�f�[�^�B</param>
    /// <param name="worldCoord">�u���b�N�̃��[���h���W�B</param>
    /// <param name="meshData">��������郁�b�V���f�[�^�B</param>
    /// <param name="blockType">��������u���b�N�̎�ށB</param>
    /// <returns>�X�V���ꂽ���b�V���f�[�^�B</returns>
    public ChunkMeshData GetMeshData(ChunkInfo chunk, Vector3Int worldCoord, ChunkMeshData meshData, BlockNameType blockType)
    {
        // ��C�܂��͖����ȃu���b�N�^�C�v�̏ꍇ�͂��̂܂ܕԂ�
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

        // �e�����ɑ΂��ď������s��
        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = worldCoord + direction.GetVector();     // ���[���h���W�ɗאڍ��W�𑫂�

            if (GetChunk(neighbourBlockCoordinates) == null) { continue; }

            var neighbourBlockType = GetBlock(neighbourBlockCoordinates);           // BlockType�擾      // ����������ƒ[�̗אځH

            // �אڃu���b�N���C��/�t��/���ߌő̂̂����ꂩ�ł����Mesh�ɒǉ�
            switch (GetBlockBehaviourType(neighbourBlockType))
            {
                case BlockBehaviourType.Gas:
                case BlockBehaviourType.Liquid:
                case BlockBehaviourType.TransparentSolid:
                    //Mesh�ǉ��Ώۂ��t�̂�����
                    if (GetBlockBehaviourType(blockType) == BlockBehaviourType.Liquid)
                    {
                        // �אڃu���b�N���C��/���ߌő̂̂����ꂩ�ł���Ήt��Mesh��ǉ�
                        switch (GetBlockBehaviourType(neighbourBlockType))
                        {
                            case BlockBehaviourType.Gas:
                            case BlockBehaviourType.TransparentSolid:
                                // ���[�J�����W
                                Vector3Int localCoord = worldCoord - chunk.worldCoord;

                                // �ʂ̒��_���擾
                                ChunkMeshData.GetFaceVertices(direction, localCoord.x, localCoord.y, localCoord.z, meshData.waterMesh, blockType);

                                bool generatesCollider;
                                // �R���C�_�[�𐶐�����ꍇ�̎O�p�`��ǉ�
                                switch (GetBlockBehaviourType(blockType))
                                {
                                    case BlockBehaviourType.Gas:
                                    case BlockBehaviourType.Liquid:
                                        generatesCollider = false;      // �C��/�t�̂̏ꍇ��Collider��ǉ����Ȃ�
                                        break;

                                    default:
                                        generatesCollider = true;       // �C��/�t�̈ȊO�̏ꍇ��Collider��ǉ�����
                                        break;
                                }
                                meshData.waterMesh.AddQuadTriangles(generatesCollider);

                                // UV���W��ǉ�
                                meshData.waterMesh.uv.AddRange(FaceUVs(direction, blockType));
                                Debug.Log("UV���W�ǉ��̓R�����g�A�E�g��");
                                break;
                        }
                    }
                    else
                    {
                        // ���̑��̃u���b�N�^�C�v�̏ꍇ�A���b�V���f�[�^�ɒǉ�

                        // ���[�J�����W
                        Vector3Int localCoord = worldCoord - chunk.worldCoord;

                        // �ʂ̒��_���擾
                        ChunkMeshData.GetFaceVertices(direction, localCoord.x, localCoord.y, localCoord.z, meshData, blockType);

                        bool generatesCollider;
                        // �R���C�_�[�𐶐�����ꍇ�̎O�p�`��ǉ�
                        switch (GetBlockBehaviourType(blockType))
                        {
                            case BlockBehaviourType.Gas:
                            case BlockBehaviourType.Liquid:
                                generatesCollider = false;      // �C��/�t�̂̏ꍇ��Collider��ǉ����Ȃ�
                                break;

                            default:
                                generatesCollider = true;       // �C��/�t�̈ȊO�̏ꍇ��Collider��ǉ�����
                                break;
                        }
                        meshData.AddQuadTriangles(generatesCollider);

                        // UV���W��ǉ�
                        meshData.uv.AddRange(FaceUVs(direction, blockType));
                    }
                    break;
            }
        }

        return meshData;
    }

    /// <summary>
    /// �w�肳�ꂽ�����ƃu���b�N�^�C�v�Ɋ�Â��� UV ���W���擾����B
    /// </summary>
    /// <param name="direction">�ʂ̕���</param>
    /// <param name="blockType">��������u���b�N�̎��</param>
    /// <returns>UV ���W�̔z��</returns>
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
