using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
    /// <summary>
    /// 車輪の位置
    /// </summary>
    public enum WheelPosition
    {
        [Tooltip("前輪")] FrontWheel,
        [Tooltip("後輪")] RearWheel
    }

    // 接地しているか

    [SerializeField, Header("車輪の位置")]
    private WheelPosition _wheelPosition = 0;

    [SerializeField, Header("車輪を配置する位置")]
    private Transform _wheelOffset = null;

    /// <summary>
    /// 車輪の位置
    /// </summary>
    public WheelPosition ItsWheelPosition => _wheelPosition;

    /// <summary>
    /// WheelCollider
    /// </summary>
    public WheelCollider WheelCollider { get; private set; } = null;

    /// <summary>
    /// 車輪を配置する位置
    /// </summary>
    public Transform WheelOffset => _wheelOffset;

    private void OnEnable()
    {
        WheelCollider = GetComponent<WheelCollider>();  // RequireComponent

        // タイヤの半径をsphereColで取得、不要になったsphereColを削除
        SphereCollider sphereCol = WheelOffset.gameObject.AddComponent<SphereCollider>();

        // 車輪の最大の大きさを取得、sphereColの半径にかけて半径を取得
        float wheelSscale = Mathf.Max(WheelOffset.localScale.x, WheelOffset.localScale.y, WheelOffset.localScale.z);
        WheelCollider.radius = sphereCol.radius * wheelSscale;
        Destroy(sphereCol);

        // wheelColを車輪モデルと同位置同角度に変更
        transform.localPosition = WheelOffset.localPosition;
        transform.localEulerAngles = WheelOffset.localEulerAngles;
    }
}
