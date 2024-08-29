using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �^���[���x���̐ݒ�
/// </summary>
[CreateAssetMenu(fileName = "TowerLevelSettingSO", menuName = "ScriptableObject/TowerLevelSettingSO")]
public class TowerLevelSettingSO : ScriptableObject
{
    [SerializeField, Header("�^���[�̐���")]
    private string _description = "�^���[�̐���";

    [SerializeField, Header("�^���[�̃A�b�v�O���[�h����")]
    private string _upgradeDescription = "�^���[�̃A�b�v�O���[�h����";

    [SerializeField, Header("�A�b�v�O���[�h�ɕK�v�ȃR�X�g")]
    private int _cost = 0;

    [SerializeField, Header("�^���[�𔄋p���i")]
    private int _sell = 0;

    [SerializeField, Min(0), Header("�ő�̗�")]
    private int _maxHealth = 0;

    [SerializeField, Min(0), Header("�����̗�")]
    private int _startingHealth = 0;

    [SerializeField, Header("�^���[�̃A�C�R��")]
    private Sprite _icon = null;

    /// <summary>
    /// �^���[�̐���
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// �^���[�̃A�b�v�O���[�h����
    /// </summary>
    public string UpgradeDescription => _upgradeDescription;

    /// <summary>
    /// �A�b�v�O���[�h�ɕK�v�ȃR�X�g
    /// </summary>
    public int Cost => _cost;

    /// <summary>
    /// �^���[�𔄋p���i
    /// </summary>
    public int Sell => _sell;

    /// <summary>
    /// �ő�̗�
    /// </summary>
    public int MaxHealth => _maxHealth;

    /// <summary>
    /// �����̗�
    /// </summary>
    public int StartingHealth => _startingHealth;

    /// <summary>
    /// �^���[�̃A�C�R��
    /// </summary>
    public Sprite Icon => _icon;
}

