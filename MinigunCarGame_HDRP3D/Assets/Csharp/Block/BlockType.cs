using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �u���b�N�̎��
/// </summary>
static public class BlockType
{
    /// <summary>
    /// �u���b�N�̖��O�̎��
    /// </summary>
    public enum BlockNameType
    {
        [Tooltip("��C")] Air,
        [Tooltip("���̐������y")] GrassDirt,
        [Tooltip("�y")] Dirt,
        [Tooltip("�K���X")] Glass,
    }

    /// <summary>
    /// �u���b�N�̐U�镑���̎��
    /// </summary>
    public enum BlockBehaviourType
    {
        [Tooltip("�Ȃɂ��Ȃ�")] None,
        [Tooltip("�C��")] Gas,
        [Tooltip("�ő�")] Solid,
        [Tooltip("�t��")] Liquid,
        [Tooltip("���ߌő�")] TransparentSolid,
    }

    // �u���b�N�ƐU�镑���̎���
    static private readonly Dictionary<BlockNameType, BlockBehaviourType> _blockBehaviourTypes = new()
    {
        { BlockNameType.Air, BlockBehaviourType.Gas },
        { BlockNameType.GrassDirt, BlockBehaviourType.Solid },
        { BlockNameType.Dirt, BlockBehaviourType.Solid },
        { BlockNameType.Glass, BlockBehaviourType.TransparentSolid },
    };

    // �u���b�N�ƃ^�C���ʒu�̎���
    static private readonly Dictionary<BlockNameType, Vector2> _blockTilePositions = new()
    {
        { BlockNameType.GrassDirt, new Vector2(4, 0) },
        { BlockNameType.Dirt, new Vector2(4, 1) },
        { BlockNameType.Glass, new Vector2(4, 2) },
    };

    /// <summary>
    /// �u���b�N�̐U�镑�����擾����
    /// </summary>
    /// <param name="blockNameType">�U�镑�����擾����u���b�N</param>
    /// <returns>�u���b�N�̐U�镑��</returns>
    static public BlockBehaviourType GetBlockBehaviourType(BlockNameType blockNameType)
    {
        // �����Ɋ܂܂�Ă��Ȃ��u���b�N��������؂�グ��
        if (!_blockBehaviourTypes.ContainsKey(blockNameType)) { return 0; }

        return _blockBehaviourTypes[blockNameType];
    }

    /// <summary>
    /// �u���b�N�̃^�C���ʒu���擾����
    /// </summary>
    /// <param name="blockNameType">�^�C���ʒu���擾����u���b�N</param>
    /// <returns>�u���b�N�̃^�C���ʒu</returns>
    static public Vector2 GetBlockTilePosition(BlockNameType blockNameType)
    {
        // �����Ɋ܂܂�Ă��Ȃ��u���b�N��������؂�グ��
        if (!_blockTilePositions.ContainsKey(blockNameType)) { return -Vector2.one; }

        return _blockTilePositions[blockNameType];
    }
}
