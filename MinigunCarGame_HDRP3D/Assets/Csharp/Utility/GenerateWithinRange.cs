using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GenerateWithinRange
{
    /// <summary>
    /// �����_���ȕ�����nM���ꂽ���W�Ƀ����_���ɐ�������
    /// </summary>
    /// <param name="center">���S</param>
    /// <param name="distance">�������鋗��</param>
    /// <param name="generateObject">��������I�u�W�F�N�g</param>
    /// <returns></returns>
    static public GameObject GenerateAtRandomDistance(GameObject generateObject, Vector3 center, float distance)
    {
        // �����_���ȕ����Ƀ����_���ȋ����������ꂽ�ꏊ���v�Z
        Vector3 spawnPosition = GetSpawnPosition(center, distance, distance);

        GameObject result = GameObject.Instantiate(generateObject, spawnPosition, Quaternion.identity);
        return result;
    }

    /// <summary>
    /// nM����mM�ȓ��̍��W�Ƀ����_���ɐ�������B
    /// �܂��́AnM�ȓ��̍��W�Ƀ����_���ɐ�������B
    /// </summary>
    /// <param name="generateObject">��������I�u�W�F�N�g</param>
    /// <param name="center">���S</param>
    /// <param name="maxRange">��������ő勗��</param>
    /// <param name="minRange">��������ŏ�����</param>
    /// <returns></returns>
    static public GameObject GenerateAtRandomDistance(GameObject generateObject, Vector3 center, float maxRange, float minRange = 0.0f)
    {
        // �����_���ȕ����Ƀ����_���ȋ����������ꂽ�ꏊ���v�Z
        Vector3 spawnPosition = GetSpawnPosition(center, minRange, maxRange);

        GameObject result = GameObject.Instantiate(generateObject, spawnPosition, Quaternion.identity);
        return result;
    }

    /// <summary>
    /// �����_���ȕ����Ƀ����_���ȋ����������ꂽ�ꏊ���v�Z
    /// </summary>
    /// <param name="center">���S</param>
    /// <param name="minRange">�ŏ�����</param>
    /// <param name="maxRange">�ő勗��</param>
    /// <returns></returns>
    static public Vector3 GetSpawnPosition(Vector3 center, float minRange, float maxRange)
    {
        // �~���̃����_���Ȉʒu���v�Z
        float theta = Random.Range(0f, Mathf.PI * 2);   // 0�x����360�x�܂ł̃����_���Ȋp�x
        float x = Mathf.Cos(theta);                     // Cos�l���v�Z�i�]���j
        float y = 0.0f;                                 // �������`
        float z = Mathf.Sin(theta);                     // Sin�l���v�Z�i�����j
        Vector3 randomDirection = new Vector3(x, y, z);

        // min, maxRange�������̏ꍇ�A�����_���ȋ������擾�����Ɍ��ʂ��o��
        if (minRange == maxRange) { return center + randomDirection * minRange; }

        float randomDistance = Random.Range(minRange, maxRange);    // �����_���ȋ������擾

        // �����_���ȕ����Ƀ����_���ȋ����������ꂽ�ꏊ���v�Z
        return center + randomDirection * randomDistance;
    }
}
