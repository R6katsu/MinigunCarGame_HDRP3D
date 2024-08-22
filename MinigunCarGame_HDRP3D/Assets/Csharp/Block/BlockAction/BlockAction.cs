using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockType;

public class BlockAction
{
    /// <summary>
    /// �u���b�N�̔j��
    /// </summary>
    /// <param name="hit">�v���C���[����u���b�N�Ɍ����Ă�Ray</param>
    /// <param name="sourcePosition">Ray�̔��ˈʒu</param>
    static public void BreakBlock(RaycastHit hit, Vector3 sourcePosition)
    {
        float rayLengthening = 0.1f;    // Ray���u���b�N�����ɉ���

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
    /// �u���b�N�̐ݒu
    /// </summary>
    /// <param name="hitPoint">�v���C���[����u���b�N�Ɍ����Ă�Ray</param>
    static public void PlacementBlock(Vector3Int hitPoint, BlockNameType blockNameType)
    {
        VoxelWorldBehaviour.Instance.SetBlock(hitPoint, blockNameType);
    }
}
