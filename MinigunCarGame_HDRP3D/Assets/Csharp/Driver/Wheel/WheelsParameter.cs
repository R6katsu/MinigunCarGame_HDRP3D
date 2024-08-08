using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[Serializable]
public class WheelsParameter
{
    /// <summary>
    /// �ԗւ̋쓮�̎��
    /// </summary>
    public enum WheelDriveType
    {
        [Tooltip("�O��")] FrontWheels,
        [Tooltip("���")] RearWheels,
        [Tooltip("�S��")] AllWheels
    }

    #region Field
    [SerializeField, Header("�X�e�A�����O�쓮�̎��")]
    private WheelDriveType _wheelsSteeringType = 0;

    [SerializeField, Header("�A�N�Z���쓮�̎��")]
    private WheelDriveType _wheelsAcceleratorType = 0;

    [SerializeField, Header("�u���[�L�쓮�̎��")]
    private WheelDriveType _wheelsBrakeType = 0;

    [SerializeField, Header("�ԗւ̔z��")]
    private Wheel[] _wheels = null;

    private List<Wheel> _frontWheels = new();     // �O�ւ̔z��
    private List<Wheel> _rearWheels = new();      // ��ւ̔z��

    [Tooltip("����������Ă��邩")]
    private bool _isInitialize = false;
    #endregion

    #region Get/Set
    /// <summary>
    /// �X�e�A�����O�쓮�̎��
    /// </summary>
    public WheelDriveType WheelsSteeringType => _wheelsSteeringType;

    /// <summary>
    /// �A�N�Z���쓮�̎��
    /// </summary>
    public WheelDriveType WheelsAcceleratorType => _wheelsAcceleratorType;

    /// <summary>
    /// �u���[�L�쓮�̎��
    /// </summary>
    public WheelDriveType WheelsBrakeType => _wheelsBrakeType;

    /// <summary>
    /// �ԗւ̔z��
    /// </summary>
    public ReadOnlyCollection<Wheel> Wheels
    {
        get
        {
            if (_wheels == null)
            {
                _wheels = new Wheel[0];
            }

            return new(_wheels);
        }
    }

    /// <summary>
    /// �O�ւ̔z��
    /// </summary>
    public ReadOnlyCollection<Wheel> FrontWheels
    {
        get
        {
            // ����������Ă��Ȃ���Ώ���������
            if (!_isInitialize) { Initialize(); }

            return new(_frontWheels);
        }
    }

    /// <summary>
    /// ��ւ̔z��
    /// </summary>
    public ReadOnlyCollection<Wheel> RearWheels
    {
        get
        {
            // ����������Ă��Ȃ���Ώ���������
            if (!_isInitialize) { Initialize(); }

            return new(_rearWheels);
        }
    }
    #endregion

    // �X�e�A�����O�ȂǁA�S�Ẵ^�C���ɓ����ݒ������

    // �^�C���ɑ΂��ĉ�����X�e�A�����O���s�������ꍇ�A_wheels��T������

    // _wheelPosition����O�ւ���ւ����ʂ��A�O�֔z��A��֔z��ɐU�蕪����

    /// <summary>
    /// ������
    /// </summary>
    private void Initialize()
    {
        if (_isInitialize) { return; }
        _isInitialize = true;

        foreach (var wheel in Wheels)
        {
            WheelPositionAssignment(wheel);
        }
    }

    /// <summary>
    /// �ԗւ̈ʒu�ɉ����ă��X�g�ɐU�蕪����
    /// </summary>
    private void WheelPositionAssignment(Wheel wheel)
    {
        switch (wheel.ItsWheelPosition)
        {
            case Wheel.WheelPosition.FrontWheel:
                _frontWheels.Add(wheel);
                break;

            case Wheel.WheelPosition.RearWheel:
                _rearWheels.Add(wheel);
                break;

            default:
                break;
        }
    }
}
