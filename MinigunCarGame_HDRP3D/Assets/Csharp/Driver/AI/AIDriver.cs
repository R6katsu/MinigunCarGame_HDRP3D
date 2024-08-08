using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �����^�]�ɂ���ē���
/// </summary>
public class AIDriver : DriverBehaviour
{
    /// <summary>
    /// �O���ォ
    /// </summary>
    public enum FrontOrBehind
    {
        None,
        [Tooltip("�O")] Front,
        [Tooltip("��")] Behind
    }

    /// <summary>
    /// �E������
    /// </summary>
    public enum RightOrLeft
    {
        None,
        [Tooltip("�E")] Right,
        [Tooltip("��")] Left,
        [Tooltip("�^������")] Straight
    }

    // SerializeField
    [SerializeField, Min(0.0f), Header("���x���")]
    private float _speedLimit = 0.0f;

    [SerializeField, Header("�ڕW�ʒu���q�Ɏ��eTransform")]
    private Transform _objectiveTransformsParent = null;

    // private
    private bool _isSteering = false;
    private bool _isAccelerator = false;
    private bool _isBrake = false;

    private int _currentDestinationNumber = 0;

    [Tooltip("�ڕW�ʒu�̃��X�g")]
    private List<Transform> _objectiveTransforms = new();

    override protected void OnEnable()
    {
        base.OnEnable();

        // �ڕW�ʒu���q�Ɏ��eTransform��T�����A�ڕW�ʒu�̃��X�g���쐬����
        foreach (Transform transform in _objectiveTransformsParent)
        {
            _objectiveTransforms.Add(transform);
        }

        StartCoroutine(Accelerator(1.0f));
    }

    private FrontOrBehind _currentFrontOrBehind = 0;
    private RightOrLeft _currentRightOrLeft = 0;

    private RightOrLeft _lastTimeRightOrLeft = 0;

    private FrontOrBehind CurrentFrontOrBehind
    {
        get => _currentFrontOrBehind;
        set
        {
            if (_currentFrontOrBehind != value)
            {
                _currentFrontOrBehind = value;
                ChangeFrontOrBehind();
            }
        }
    }

    private RightOrLeft CurrentRightOrLeft
    {
        get => _currentRightOrLeft;
        set
        {
            if (_currentRightOrLeft != value)
            {
                _currentRightOrLeft = value;
                ChangeRightOrLeft();
            }
        }
    }

    private void ChangeFrontOrBehind()
    {
        float accelerator = 0.0f;
        accelerator = 1.0f;
        switch (CurrentFrontOrBehind)
        {
            case FrontOrBehind.Front:
                //accelerator = 1.0f;
                break;

            case FrontOrBehind.Behind:
                //accelerator = 0.0f;

                if (CurrentRightOrLeft == RightOrLeft.Straight)
                {
                    int random = Mathf.RoundToInt(UnityEngine.Random.value);

                    if (random == 1)
                    {
                        CurrentRightOrLeft = RightOrLeft.Right;
                    }
                    else
                    {
                        CurrentRightOrLeft = RightOrLeft.Left;
                    }
                }
                break;

            default:
                break;
        }

        //StartCoroutine(Accelerator(accelerator));
    }

    private void ChangeRightOrLeft()
    {
        float steering = 0.0f;
        switch (CurrentRightOrLeft)
        {
            case RightOrLeft.Right:
                steering = 1.0f;
                break;

            case RightOrLeft.Left:
                steering = -1.0f;
                break;

            case RightOrLeft.Straight:
                if (CurrentFrontOrBehind != FrontOrBehind.Front)
                {
                    if (!_isSteering)
                    {
                        StartCoroutine(Steering(steering));
                    }

                    return;
                }

                HorizontalCanceled();

                // �O��͖ڕW�n�_�ɑ΂��Đ^�������ł͂Ȃ�����
                if (_lastTimeRightOrLeft != RightOrLeft.Straight)
                {
                    // ��]�� 0�ɂ���
                    Vector3 angle = Rigidbody.angularVelocity;
                    angle.y = 0.0f;
                    Rigidbody.angularVelocity = angle;

                    // �X�e�A�����O��K�p����ԗ֔z��
                    var steerWheels = DriveTypeWheels(_wheelsParameter, _wheelsParameter.WheelsSteeringType);

                    // �S�Ă̎ԗւ̃X�e�A�����O�� 0�ɂ���
                    for (int i = 0; i < steerWheels.Count; i++)
                    {
                        steerWheels[i].WheelCollider.steerAngle = 0.0f;
                    }
                }
                break;

            default:
                break;
        }

        _lastTimeRightOrLeft = CurrentRightOrLeft;      // �����RightOrLeft�����̑O���RightOrLeft�ɑ��

        HorizontalCanceled();
        StartCoroutine(Steering(steering));
    }

    /// <summary>
    /// �ڕW�n�_�͑O��ǂ���̕����ɂ��邩�A���E�ǂ���̕����ɂ��邩�𔻒肵���ʂ�Ԃ�
    /// </summary>
    /// <param name="destination">�ڕW�n�_</param>
    /// <returns>�ڕW�n�_�͑O��ǂ���̕����ɂ��邩�A���E�ǂ���̕����ɂ��邩</returns>
    private (FrontOrBehind, RightOrLeft) GetTargetDirection(Transform destination)
    {
        // �ڕW�n�_�͑O��ǂ���̕����ɂ��邩�A���E�ǂ���̕����ɂ��邩
        FrontOrBehind responseFrontOrBehind = FrontOrBehind.Front;
        RightOrLeft responseRightOrLeft = RightOrLeft.Right;

        // ���E�𔻒肷�邽�߂�臒l���`
        float sideDotThreshold = 0.1f;

        // ���݂̈ʒu����ړI�n�܂ł̕����x�N�g�����v�Z
        Vector3 directionToDestination = destination.position - transform.position;

        // �x�N�g���̒����𖳎����ĕ����������擾���邽�߂ɐ��K��
        directionToDestination.Normalize();

        // ���ʂ������Ă��邩�ǂ������h�b�g�ςŔ���
        float dotProduct = Vector3.Dot(transform.forward, directionToDestination);

        // ���E�̕����𔻒�
        float crossProductY = Vector3.Cross(transform.forward, directionToDestination).y;

        // �ڕW�n�_���O��ǂ���̕����ɂ��邩
        if (dotProduct > 0)
        {
            // Dot�����̒l�̎��͑O���ɖڕW������
            responseFrontOrBehind = FrontOrBehind.Front;
        }
        else
        {
            // Dot�����̒l�̎��͌���ɖڕW������
            responseFrontOrBehind = FrontOrBehind.Behind;
        }

        // �ڕW�n�_�����E�ǂ���̕����ɂ��邩
        if (crossProductY > sideDotThreshold)
        {
            // �ڕW���E�ɂ���
            responseRightOrLeft = RightOrLeft.Right;
        }
        else if (crossProductY < -sideDotThreshold)
        {
            // �ڕW�����ɂ���
            responseRightOrLeft = RightOrLeft.Left;
        }
        else
        {
            // �ڕW�ɑ΂��Đ^������
            responseRightOrLeft = RightOrLeft.Straight;
        }

        return (responseFrontOrBehind, responseRightOrLeft);
    }

    private void Update()
    {
        (FrontOrBehind frontOrBehind, RightOrLeft rightOrLeft) targetDirection = GetTargetDirection(_objectiveTransforms[_currentDestinationNumber]);

        CurrentFrontOrBehind = targetDirection.frontOrBehind;
        CurrentRightOrLeft = targetDirection.rightOrLeft;

        // ���̖ڕW�Ƃ̋���
        float arrivalThreshold = 10.0f; // �ړI�n���B�Ƃ݂Ȃ�������臒l

        Vector3 targetPoint = _objectiveTransforms[_currentDestinationNumber].position;
        targetPoint.y = transform.position.y;

        // ���݂̈ʒu�ƖړI�n�̈ʒu�Ƃ̋������v�Z
        float distanceToDestination = Vector3.Distance(transform.position, targetPoint);

        // ���������B臒l�ȉ����ǂ����𔻒�
        if (distanceToDestination <= arrivalThreshold)
        {
            int x = UnityEngine.Random.Range(-30, 30);
            int z = UnityEngine.Random.Range(-30, 30);
            _objectiveTransforms[0].position = new Vector3(x, 0, z);
            /*
            _currentDestinationNumber++;

            // �ړI�n�̐��𒴉�
            if (_objectiveTransforms.Count <= _currentDestinationNumber)
            {
                _currentDestinationNumber = 0;
            }
            */
        }
    }

    private void HorizontalCanceled()
    {
        _isSteering = false;

        // �X�e�A�����O�쓮��0.0f�ɂ���
        FinishSteering(_wheelsParameter);
    }

    private void VerticalCanceled()
    {
        _isAccelerator = false;

        // �A�N�Z���쓮��0.0f�ɂ���
        FinishAccelerator(_wheelsParameter);
    }

    private void BrakeStarted()
    {
        StartCoroutine(Brake());
    }

    private void BrakeCanceled()
    {
        _isBrake = false;

        // �u���[�L��0.0f�ɂ���
        FinishBrake(_wheelsParameter);
    }

    /// <summary>
    /// �X�e�A�����O
    /// </summary>
    /// <returns></returns>
    private IEnumerator Steering(float value)
    {
        // ���ǒ��Ȃ�؂�グ��
        if (_isSteering) { yield break; }

        _isSteering = true;

        // FixedUpdate
        while (_isSteering)
        {
            ApplySteering(_wheelsParameter, value);

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// �A�N�Z��
    /// </summary>
    /// <returns></returns>
    private IEnumerator Accelerator(float value)
    {
        // �ړ����Ȃ�؂�グ��
        if (_isAccelerator) { yield break; }

        _isAccelerator = true;

        // FixedUpdate
        while (_isAccelerator)
        {
            float speedLimit = _speedLimit;

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = _objectiveTransforms[_currentDestinationNumber].position;

            // ���݂̌������擾
            Vector3 currentForward = transform.forward;

            // �ڕW�ւ̕����x�N�g�����v�Z
            Vector3 directionToTarget = (targetPosition - currentPosition).normalized;

            // ���݂̌����ƖڕW�ւ̕����x�N�g���̊Ԃ̊p�x���v�Z
            float entryAngle = Vector3.Angle(currentForward, directionToTarget);

            // ���݂̌������猩�č����E���𔻒�
            float sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(currentForward, directionToTarget)));
            float signedAngle = entryAngle * sign;

            // �덷10�𒴉߂��Ă����瑬�x����������������
            if (Mathf.Abs(signedAngle) > 15.0f)
            {
                speedLimit = 7.5f;
            }
            else if (Mathf.Abs(signedAngle) > 10.0f)
            {
                speedLimit = 10.0f;
            }

            if (_objectiveTransforms.Count > _currentDestinationNumber + 1)
            {
                // ���̖ڕW�̊p�x
                Vector3 vectorA = transform.position;
                Vector3 vectorB = _objectiveTransforms[_currentDestinationNumber].position;
                Vector3 vectorC = _objectiveTransforms[_currentDestinationNumber + 1].position;

                // vectorB����_�Ƃ���vectorA��vectorC�̕����x�N�g�����v�Z
                Vector3 AB = vectorA - vectorB;
                Vector3 CB = vectorC - vectorB;

                // �p�x�����߂�
                float angle = Vector3.Angle(AB, CB);

                vectorB.y = vectorA.y;

                // ���݂̈ʒu�ƖړI�n�̈ʒu�Ƃ̋������v�Z
                float distanceToDestination = Vector3.Distance(vectorA, vectorB);

                // ���x�����߂��Ă���A���߂���Α��x�����������
                if (angle <= 100.0f && distanceToDestination <= 20.0f)
                {
                    speedLimit = 7.5f;
                }
            }

            ApplyAcceleratorWithSpeedLimit(_wheelsParameter, value, speedLimit);

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// ������x�t���A�N�Z���쓮
    /// </summary>
    /// <param name="wheelsParameter">�ԗւ̃p�����[�^�[</param>
    /// <param name="direction">����</param>
    /// <param name="speedLimit">������x</param>
    private void ApplyAcceleratorWithSpeedLimit(WheelsParameter wheelsParameter, float direction, float speedLimit)
    {
        base.ApplyAccelerator(wheelsParameter, direction);

        // �A�N�Z����K�p����ԗ֔z��
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // �A�N�Z���𔽉f
        foreach (var acceleWheel in acceleWheels)
        {
            if (speedLimit < Speed)
            {
                // �O�i���x��������x�𒴉߂��Ă���ꍇ�͌���
                acceleWheel.WheelCollider.motorTorque = 0;
                continue;
            }
        }
    }

    /// <summary>
    /// �u���[�L
    /// </summary>
    private IEnumerator Brake()
    {
        // �������Ȃ�؂�グ��
        if (_isBrake) { yield break; }

        _isBrake = true;

        // FixedUpdate
        while (_isBrake)
        {
            ApplyBrake(_wheelsParameter);

            yield return new WaitForFixedUpdate();
        }
    }
}
