using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �U�����
/// </summary>
[Serializable]
public struct AttackInfo
{
    [SerializeField, Header("�_���[�W")]
    private int _damage;

    public int Damage => _damage;

    // ����(���A��etc...)
    // �t������(�A���A�ђ�etc...)

    /// <summary>
    /// �U�����̃R���X�g���N�^
    /// </summary>
    /// <param name="damage">�_���[�W</param>
    public AttackInfo(int damage)
    {
        _damage = damage;
    }
}
