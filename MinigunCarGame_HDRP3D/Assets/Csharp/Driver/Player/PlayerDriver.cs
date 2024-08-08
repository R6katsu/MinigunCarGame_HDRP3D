using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �v���C���[�̓��͂ɂ���ē�����
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerDriver : DriverBehaviour
{
    // SerializeField
    [SerializeField, Min(0.0f), Header("���x���")]
    private float _speedLimit = 0.0f;

    // private
    private bool _isSteering = false;
    private bool _isAccelerator = false;
    private bool _isBrake = false;

    override protected void OnEnable()
    {
        base.OnEnable();

        var playerInput = GetComponent<PlayerInput>();     // RequireComponent
        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

        // �A�N�V�����}�b�v�̎擾
        var car = InputActionsName.PlayerDriverControlsActionMapsName.Car;
        var actionMap = playerInput.actions.FindActionMap(InputActionsName.GetActionMapsName(car));

        // �A�N�V�����̎擾
        var horizontal = InputActionsName.PlayerDriverControlsActionsName.Horizontal;
        var horizontalAction = actionMap.FindAction(InputActionsName.GetActionsName(actionMap, horizontal));

        horizontalAction.started += HorizontalStarted;
        horizontalAction.canceled += HorizontalCanceled;

        //
        var vertical = InputActionsName.PlayerDriverControlsActionsName.Vertical;
        var verticalAction = actionMap.FindAction(InputActionsName.GetActionsName(actionMap, vertical));

        verticalAction.started += VerticalStarted;
        verticalAction.canceled += VerticalCanceled;

        //
        var brake = InputActionsName.PlayerDriverControlsActionsName.Brake;
        var brakeAction = actionMap.FindAction(InputActionsName.GetActionsName(actionMap, brake));

        brakeAction.started += BrakeStarted;
        brakeAction.canceled += BrakeCanceled;
    }

    private void HorizontalStarted(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        StartCoroutine(Steering(value));
    }

    private void HorizontalCanceled(InputAction.CallbackContext context)
    {
        _isSteering = false;

        // �X�e�A�����O�쓮��0.0f�ɂ���
        FinishSteering(_wheelsParameter);
    }

    private void VerticalStarted(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        StartCoroutine(Accelerator(value));
    }

    private void VerticalCanceled(InputAction.CallbackContext context)
    {
        _isAccelerator = false;

        // �A�N�Z���쓮��0.0f�ɂ���
        FinishAccelerator(_wheelsParameter);
    }

    private void BrakeStarted(InputAction.CallbackContext context)
    {
        StartCoroutine(Brake());
    }

    private void BrakeCanceled(InputAction.CallbackContext context)
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
            ApplyAcceleratorWithSpeedLimit(_wheelsParameter, value, _speedLimit);

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
            Debug.Log("������");

            yield return new WaitForFixedUpdate();
        }
    }
}
