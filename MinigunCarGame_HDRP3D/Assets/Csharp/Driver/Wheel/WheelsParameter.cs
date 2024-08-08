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
    /// 車輪の駆動の種類
    /// </summary>
    public enum WheelDriveType
    {
        [Tooltip("前輪")] FrontWheels,
        [Tooltip("後輪")] RearWheels,
        [Tooltip("全輪")] AllWheels
    }

    #region Field
    [SerializeField, Header("ステアリング駆動の種類")]
    private WheelDriveType _wheelsSteeringType = 0;

    [SerializeField, Header("アクセル駆動の種類")]
    private WheelDriveType _wheelsAcceleratorType = 0;

    [SerializeField, Header("ブレーキ駆動の種類")]
    private WheelDriveType _wheelsBrakeType = 0;

    [SerializeField, Header("車輪の配列")]
    private Wheel[] _wheels = null;

    private List<Wheel> _frontWheels = new();     // 前輪の配列
    private List<Wheel> _rearWheels = new();      // 後輪の配列

    [Tooltip("初期化されているか")]
    private bool _isInitialize = false;
    #endregion

    #region Get/Set
    /// <summary>
    /// ステアリング駆動の種類
    /// </summary>
    public WheelDriveType WheelsSteeringType => _wheelsSteeringType;

    /// <summary>
    /// アクセル駆動の種類
    /// </summary>
    public WheelDriveType WheelsAcceleratorType => _wheelsAcceleratorType;

    /// <summary>
    /// ブレーキ駆動の種類
    /// </summary>
    public WheelDriveType WheelsBrakeType => _wheelsBrakeType;

    /// <summary>
    /// 車輪の配列
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
    /// 前輪の配列
    /// </summary>
    public ReadOnlyCollection<Wheel> FrontWheels
    {
        get
        {
            // 初期化されていなければ初期化する
            if (!_isInitialize) { Initialize(); }

            return new(_frontWheels);
        }
    }

    /// <summary>
    /// 後輪の配列
    /// </summary>
    public ReadOnlyCollection<Wheel> RearWheels
    {
        get
        {
            // 初期化されていなければ初期化する
            if (!_isInitialize) { Initialize(); }

            return new(_rearWheels);
        }
    }
    #endregion

    // ステアリングなど、全てのタイヤに同じ設定をする

    // タイヤに対して加速やステアリングを行いたい場合、_wheelsを探索する

    // _wheelPositionから前輪か後輪か判別し、前輪配列、後輪配列に振り分ける

    /// <summary>
    /// 初期化
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
    /// 車輪の位置に応じてリストに振り分ける
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
