using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// 駆動装置の挙動
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DriverBehaviour : MonoBehaviour
{
    #region Field
    [SerializeField, Header("車輪のパラメーター")]
    protected WheelsParameter _wheelsParameter = null;

    [SerializeField, Min(0.0f), Header("操舵力")]
    protected float _steeringPower = 30.0f;

    [SerializeField, Min(0.0f), Header("加速力")]
    protected float _acceleratorPower = 1000.0f;

    [SerializeField, Min(0.0f), Header("減速力")]
    protected float _brakePower = 10000.0f;

    [SerializeField, Header("重心位置")]
    protected Vector3 _centerOfMass = new Vector3(0, 0.25f, 0);

    private Rigidbody _selfRigidbody = null;
    #endregion

    #region Get/Set
    protected Rigidbody Rigidbody
    {
        get
        {
            if (_selfRigidbody == null)
            {
                // RequireComponent
                _selfRigidbody = GetComponent<Rigidbody>();
            }

            return _selfRigidbody;
        }
    }

    /// <summary>
    /// 現在の速度
    /// </summary>
    public float Speed => Rigidbody.velocity.magnitude;
    #endregion

    virtual protected void OnEnable()
    {
        // 横転防止の重心変更
        Rigidbody.centerOfMass = _centerOfMass;
    }

    /// <summary>
    /// ステアリング駆動
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    /// <param name="direction">入力方向</param>
    virtual protected void ApplySteering(WheelsParameter wheelsParameter, float direction)
    {
        // ステアリングを適用する車輪配列
        var steerWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsSteeringType);

        // ステアリングを反映
        foreach (var steeringWheel in steerWheels)
        {
            steeringWheel.WheelCollider.steerAngle = _steeringPower * direction;

            steeringWheel.WheelCollider.GetWorldPose(out Vector3 pos, out Quaternion ang);
            steeringWheel.WheelOffset.position = steeringWheel.WheelCollider.transform.position;    // タイヤモデルに座標を適用
            steeringWheel.WheelOffset.rotation = ang;                                               // タイヤモデルに回転角を適用
        }
    }

    /// <summary>
    /// ステアリングを終える
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    virtual protected void FinishSteering(WheelsParameter wheelsParameter)
    {
        // ステアリングを適用する車輪配列
        var steerWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsSteeringType);

        // ステアリングを反映
        foreach (var steeringWheel in steerWheels)
        {
            steeringWheel.WheelCollider.steerAngle = 0.0f;
            steeringWheel.WheelOffset.rotation = transform.rotation;
        }
    }

    /// <summary>
    /// アクセル駆動
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    /// <param name="direction">入力方向</param>
    virtual protected void ApplyAccelerator(WheelsParameter wheelsParameter, float direction)
    {
        // アクセルを適用する車輪配列
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // アクセルを反映
        foreach (var acceleWheel in acceleWheels)
        {
            acceleWheel.WheelCollider.motorTorque = _acceleratorPower * direction;
        }
    }

    /// <summary>
    /// アクセルを終える
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    virtual protected void FinishAccelerator(WheelsParameter wheelsParameter)
    {
        // アクセルを適用する車輪配列
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // アクセルを反映
        foreach (var acceleWheel in acceleWheels)
        {
            acceleWheel.WheelCollider.motorTorque = 0.0f;
        }
    }

    /// <summary>
    /// ブレーキトルクをブレーキホイールに適用する。
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    virtual protected void ApplyBrake(WheelsParameter wheelsParameter)
    {
        // ブレーキを適用する車輪配列
        var brakeWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsBrakeType);

        // ブレーキを反映
        foreach (var brakeWheel in brakeWheels)
        {
            brakeWheel.WheelCollider.brakeTorque = _brakePower;
        }
    }

    /// <summary>
    /// ブレーキを終える
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    virtual protected void FinishBrake(WheelsParameter wheelsParameter)
    {
        // ブレーキを適用する車輪配列
        var brakeWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsBrakeType);

        // ブレーキを反映
        foreach (var brakeWheel in brakeWheels)
        {
            brakeWheel.WheelCollider.brakeTorque = 0.0f;
        }
    }

    /// <summary>
    /// 車輪の駆動の種類に応じた車輪を返す
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    /// <param name="wheelDriveType">車輪の駆動の種類</param>
    /// <returns>車輪の駆動の種類に応じた車輪</returns>
    protected ReadOnlyCollection<Wheel> DriveTypeWheels(WheelsParameter wheelsParameter, WheelsParameter.WheelDriveType wheelDriveType)
    {
        switch (wheelDriveType)
        {
            case WheelsParameter.WheelDriveType.FrontWheels:    // 前輪を操舵
                return wheelsParameter.FrontWheels;

            case WheelsParameter.WheelDriveType.RearWheels:     // 後輪を操舵
                return wheelsParameter.RearWheels;

            case WheelsParameter.WheelDriveType.AllWheels:      // 全輪を操舵
                return wheelsParameter.Wheels;

            default:
#if UNITY_EDITOR
                Debug.LogWarning("想定していないWheelDriveTypeだった");
#endif
                return null;
        }
    }
}
