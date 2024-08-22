using UnityEngine;
using Unity.AI.Navigation;
using static BlockType;

/// <summary>
/// �`�����N���ɂ���u���b�N�A�`�����N�̑傫���A���W�Ȃǂ̏�������class�B<br/>
/// �u���b�N�̑������z��ɑ΂���֐��Ȃǂ��܂�ł���B
/// </summary>
public class ChunkInfo
{
    [Tooltip("���`�����N�̃��[���h���W")]
    public readonly Vector3Int worldCoord = Vector3Int.zero;

    // �`�����N���̃u���b�N�̎�ނ�ێ�����z��
    private BlockNameType[,,] _blocks = new BlockNameType[0,0,0];

    /// <summary>
    /// ���������Ă����f����Ȃ��N���[���̃u���b�N�z��
    /// </summary>
    public BlockNameType[,,] CloneBlocks => (BlockNameType[,,])_blocks.Clone();

    /// <summary>
    /// �`�����N���̃u���b�N�̑���
    /// </summary>
    public int BlocksLength => _blocks.Length;

    /// <summary>
    /// �`�����N���̃u���b�N�� x���̒���
    /// </summary>
    public int BlocksXLength => _blocks.GetLength(0);

    /// <summary>
    /// �`�����N���̃u���b�N�� y���̒���
    /// </summary>
    public int BlocksYLength => _blocks.GetLength(1);

    /// <summary>
    /// �`�����N���̃u���b�N�� z���̒���
    /// </summary>
    public int BlocksZLength => _blocks.GetLength(2);

    /// <summary>
    /// �R���X�g���N�^�B
    /// �V�����`�����N
    /// </summary>
    /// <param name="chunkSize">�`�����N�̍L��</param>
    /// <param name="chunkHeight">�`�����N�̍���</param>
    /// <param name="world">���`�����N��������{�N�Z�����E</param>
    /// <param name="worldCoord">���`�����N�̃��[���h���W</param>
    public ChunkInfo(int chunkSize, int chunkHeight, Vector3Int worldCoord)
    {
        // �`�����N�̃��[���h���ł̈ʒu��ݒ�
        this.worldCoord = worldCoord;

        // �`�����N���̃u���b�N�z���������
        _blocks = new BlockNameType[chunkSize, chunkHeight, chunkSize];
    }

    /// <summary>
    /// �S�Ẵu���b�N����ւ�
    /// </summary>
    /// <param name="blocks">�������u���b�N�̔z��</param>
    public void SetAllBlock(BlockNameType[,,] blocks)
    {
        bool isXLengthEqual = blocks.GetLength(0) == _blocks.GetLength(0);
        bool isYLengthEqual = blocks.GetLength(1) == _blocks.GetLength(1);
        bool isZLengthEqual = blocks.GetLength(2) == _blocks.GetLength(2);

        // �z��̑傫��������Ă���
        if (!isXLengthEqual || !isYLengthEqual || !isZLengthEqual) { return; }

        _blocks = blocks;
    }

    /// <summary>
    /// �u���b�N����ւ�
    /// </summary>
    /// <param name="worldCoord">����ւ���̃��[���h���W</param>
    /// <param name="block">����ւ���u���b�N</param>
    public void SetBlock(Vector3Int worldCoord, BlockNameType block)
    {
        if (!ConvertLocalCoordBlock(worldCoord)) { return; }

        Vector3Int localCoord = worldCoord - this.worldCoord;

        _blocks[localCoord.x, localCoord.y, localCoord.z] = block;

        // �u���b�N�ւ̕ύX��Mesh�ɔ��f������
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
    /// �u���b�N�擾
    /// </summary>
    /// <param name="worldCoord">�擾��̃��[���h���W</param>
    public BlockNameType GetBlock(Vector3Int worldCoord)
    {
        if (!ConvertLocalCoordBlock(worldCoord))
        {
            // �͈͊O��������A�K�؂ȃ`�����N���擾���čēxGetBlock���Ăяo��
            //return VoxelWorldBehaviour.Instance.GetBlock(worldCoord);
            //Debug.Log("-1�ɂȂ����肵���̂ŁA�Ƃ肠�����͈͊O�������� 0��Ԃ��悤�ɂ���");
            return 0;
        }

        Vector3Int localCoord = worldCoord - this.worldCoord;

        return _blocks[localCoord.x, localCoord.y, localCoord.z];
    }

    /// <summary>
    /// �w��̃��[���h���W���`�����N�͈͓�������
    /// </summary>
    /// <param name="worldCoord">�͈͓������肷�郏�[���h���W</param>
    /// <returns>�w��̃��[���h���W���`�����N�͈͓���������true</returns>
    public bool ConvertLocalCoordBlock(Vector3Int worldCoord)
    {
        Vector3Int localCoord = worldCoord - this.worldCoord;

        // �`�����N�͈͓̔���
        bool isXContains = localCoord.x >= 0 && localCoord.x < BlocksXLength;
        bool isYContains = localCoord.y >= 0 && localCoord.y < BlocksYLength;
        bool isZContains = localCoord.z >= 0 && localCoord.z < BlocksZLength;

        // worldCoordBlock���`�����N�͈͓̔�������
        return isXContains && isYContains && isZContains;
    }
}