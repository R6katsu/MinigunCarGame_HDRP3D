using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HP���
/// </summary>
[Serializable]
public class HealthInfo
{
    [SerializeField, Min(0), Header("�̗�")]
    private int _health = 0;
    private int _currentHealth = -1;   // ���݂̗̑�

    /// <summary>
    /// HP�ω����̏���
    /// </summary>
    public Action ChangeHPAction { private get; set; } = null;

    /// <summary>
    /// ���S���̏���
    /// </summary>
    public Action DeathAction { private get; set; } = null;

    /// <summary>
    /// ���݂̗̑�
    /// </summary>
    public int CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            // �O�񂩂�ύX��������
            if (_currentHealth != value)
            {
                _currentHealth = value;

                ChangeHPAction?.Invoke();
            }

            // �̗͂�0�ȉ�������
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
