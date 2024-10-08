using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HP情報
/// </summary>
[Serializable]
public class HealthInfo
{
    [SerializeField, Min(0), Header("体力")]
    private int _health = 0;
    private int _currentHealth = -1;   // 現在の体力

    /// <summary>
    /// HP変化時の処理
    /// </summary>
    public Action ChangeHPAction { private get; set; } = null;

    /// <summary>
    /// 死亡時の処理
    /// </summary>
    public Action DeathAction { private get; set; } = null;

    /// <summary>
    /// 現在の体力
    /// </summary>
    public int CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            // 前回から変更があった
            if (_currentHealth != value)
            {
                _currentHealth = value;

                ChangeHPAction?.Invoke();
            }

            // 体力が0以下だった
            if (_currentHealth <= 0)
            {
                DeathAction?.Invoke();
            }
        }
    }

    public float HPValue => (float)CurrentHealth / _health;

    public void Heal(AttackInfo attackInfo)
    {

    }

    public void Damage(AttackInfo attackInfo)
    {
        if (_currentHealth == -1)
        {
            _currentHealth = _health;
        }

        CurrentHealth = _currentHealth - attackInfo.Damage;
    }
}
