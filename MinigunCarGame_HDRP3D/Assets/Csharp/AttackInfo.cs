using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃情報
/// </summary>
[Serializable]
public struct AttackInfo
{
    [SerializeField, Header("ダメージ")]
    private int _damage;

    public int Damage => _damage;

    // 属性(水、草etc...)
    // 付随効果(連撃、貫通etc...)

    /// <summary>
    /// 攻撃情報のコンストラクタ
    /// </summary>
    /// <param name="damage">ダメージ</param>
    public AttackInfo(int damage)
    {
        _damage = damage;
    }
}
