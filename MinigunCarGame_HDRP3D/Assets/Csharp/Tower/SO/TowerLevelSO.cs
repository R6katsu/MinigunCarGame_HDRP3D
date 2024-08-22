using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �^���[���x�����Ƃ̐ݒ��ێ�����f�[�^�R���e�i
/// </summary>
[CreateAssetMenu(fileName = "TowerLevelSO", menuName = "ScriptableObject/TowerLevelSO")]
public class TowerLevelSO : ScriptableObject
{
    [SerializeField, Header("UI�ɕ\������^���[�̐���")]
    private string _description = "UI�ɕ\������^���[�̐���";

    [SerializeField, Header("UI�ɕ\������^���[�̃A�b�v�O���[�h����")]
    private string _upgradeDescription = "UI�ɕ\������^���[�̃A�b�v�O���[�h����";

    [SerializeField, Header("���̃��x���ɃA�b�v�O���[�h���邽�߂̃R�X�g")]
    private int _cost = 0;

    [SerializeField, Header("�^���[�𔄋p����ۂ̉��i")]
    private int _sell = 0;

    [SerializeField, Min(0), Header("�ő�̗�")]
    private int _maxHealth = 0;

    [SerializeField, Min(0), Header("�����̗�")]
    private int _startingHealth = 0;

    [SerializeField, Header("�^���[�̃A�C�R��")]
    private Sprite _icon = null;

    /// <summary>
    /// UI�ɕ\������^���[�̐���
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// UI�ɕ\������^���[�̃A�b�v�O���[�h����
    /// </summary>
    public string UpgradeDescription => _upgradeDescription;

    /// <summary>
    /// ���̃��x���ɃA�b�v�O���[�h���邽�߂̃R�X�g
    /// </summary>
    public int Cost => _cost;

    /// <summary>
    /// �^���[�𔄋p����ۂ̉��i
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

