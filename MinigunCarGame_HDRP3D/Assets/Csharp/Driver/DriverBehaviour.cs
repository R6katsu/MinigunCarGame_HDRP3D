using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// �쓮���u�̋���
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DriverBehaviour : MonoBehaviour
{
    #region Field
    [SerializeField, Header("�ԗւ̃p�����[�^�[")]
    protected WheelsParameter _wheelsParameter = null;

    [SerializeField, Min(0.0f), Header("���Ǘ�")]
    protected float _steeringPower = 30.0f;

    [SerializeField, Min(0.0f), Header("������")]
    protected float _acceleratorPower = 1000.0f;

    [SerializeField, Min(0.0f), Header("������")]
    protected float _brakePower = 10000.0f;

    [SerializeField, Header("�d�S�ʒu")]
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
    /// ���݂̑��x
    /// </summary>
    public float Speed => Rigidbody.velocity.magnitude;
    #endregion

    virtual protected void OnEnable()
    {
        // ���]�h�~�̏d�S�ύX
        Rigidbody.centerOfMass = _centerOfMass;
    }

    /// <summary>
    /// �X�e�A�����O�쓮
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    /// <param name="direction">���͕���</param>
    virtual protected void ApplySteering(WheelsParameter wheelsParameter, float direction)
    {
        // �X�e�A�����O��K�p����ԗ֔z��
        var steerWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsSteeringType);

        // �X�e�A�����O�𔽉f
        foreach (var steeringWheel in steerWheels)
        {
            steeringWheel.WheelCollider.steerAngle = _steeringPower * direction;

            steeringWheel.WheelCollider.GetWorldPose(out Vector3 pos, out Quaternion ang);
            steeringWheel.WheelOffset.position = steeringWheel.WheelCollider.transform.position;    // �^�C�����f���ɍ��W��K�p
            steeringWheel.WheelOffset.rotation = ang;                                               // �^�C�����f���ɉ�]�p��K�p
        }
    }

    /// <summary>
    /// �X�e�A�����O���I����
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    virtual protected void FinishSteering(WheelsParameter wheelsParameter)
    {
        // �X�e�A�����O��K�p����ԗ֔z��
        var steerWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsSteeringType);

        // �X�e�A�����O�𔽉f
        foreach (var steeringWheel in steerWheels)
        {
            steeringWheel.WheelCollider.steerAngle = 0.0f;
            steeringWheel.WheelOffset.rotation = transform.rotation;
        }
    }

    /// <summary>
    /// �A�N�Z���쓮
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    /// <param name="direction">���͕���</param>
    virtual protected void ApplyAccelerator(WheelsParameter wheelsParameter, float direction)
    {
        // �A�N�Z����K�p����ԗ֔z��
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // �A�N�Z���𔽉f
        foreach (var acceleWheel in acceleWheels)
        {
            acceleWheel.WheelCollider.motorTorque = _acceleratorPower * direction;
        }
    }

    /// <summary>
    /// �A�N�Z�����I����
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    virtual protected void FinishAccelerator(WheelsParameter wheelsParameter)
    {
        // �A�N�Z����K�p����ԗ֔z��
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // �A�N�Z���𔽉f
        foreach (var acceleWheel in acceleWheels)
        {
            acceleWheel.WheelCollider.motorTorque = 0.0f;
        }
    }

    /// <summary>
    /// �u���[�L�g���N���u���[�L�z�C�[���ɓK�p����B
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    virtual protected void ApplyBrake(WheelsParameter wheelsParameter)
    {
        // �u���[�L��K�p����ԗ֔z��
        var brakeWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsBrakeType);

        // �u���[�L�𔽉f
        foreach (var brakeWheel in brakeWheels)
        {
            brakeWheel.WheelCollider.brakeTorque = _brakePower;
        }
    }

    /// <summary>
    /// �u���[�L���I����
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    virtual protected void FinishBrake(WheelsParameter wheelsParameter)
    {
        // �u���[�L��K�p����ԗ֔z��
        var brakeWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsBrakeType);

        // �u���[�L�𔽉f
        foreach (var brakeWheel in brakeWheels)
        {
            brakeWheel.WheelCollider.brakeTorque = 0.0f;
        }
    }

    /// <summary>
    /// �ԗւ̋쓮�̎�ނɉ������ԗւ�Ԃ�
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    /// <param name="wheelDriveType">�ԗւ̋쓮�̎��</param>
    /// <returns>�ԗւ̋쓮�̎�ނɉ������ԗ�</returns>
    protected ReadOnlyCollection<Wheel> DriveTypeWheels(WheelsParameter wheelsParameter, WheelsParameter.WheelDriveType wheelDriveType)
    {
        switch (wheelDriveType)
        {
            case WheelsParameter.WheelDriveType.FrontWheels:    // �O�ւ𑀑�
                return wheelsParameter.FrontWheels;

            case WheelsParameter.WheelDriveType.RearWheels:     // ��ւ𑀑�
                return wheelsParameter.RearWheels;

            case WheelsParameter.WheelDriveType.AllWheels:      // �S�ւ𑀑�
                return wheelsParameter.Wheels;

            default:
#if UNITY_EDITOR
                Debug.LogWarning("�z�肵�Ă��Ȃ�WheelDriveType������");
#endif
                return null;
        }
    }
}
