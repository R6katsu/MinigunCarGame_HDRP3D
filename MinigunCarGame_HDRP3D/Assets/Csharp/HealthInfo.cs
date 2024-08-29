using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPî•ñ
/// </summary>
[Serializable]
public class HealthInfo
{
    [SerializeField, Min(0), Header("‘Ì—Í")]
    private int _health = 0;
    private int _currentHealth = -1;   // Œ»İ‚Ì‘Ì—Í

    /// <summary>
    /// HP•Ï‰»‚Ìˆ—
    /// </summary>
    public Action ChangeHPAction { private get; set; } = null;

    /// <summary>
    /// €–S‚Ìˆ—
    /// </summary>
    public Action DeathAction { private get; set; } = null;

    /// <summary>
    /// Œ»İ‚Ì‘Ì—Í
    /// </summary>
    public int CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            // ‘O‰ñ‚©‚ç•ÏX‚ª‚ ‚Á‚½
            if (_currentHealth != value)
            {
                _currentHealth = value;

                ChangeHPAction?.Invoke();
            }

            // ‘Ì—Í‚ª0ˆÈ‰º‚¾‚Á‚½
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
