using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力によって動かす
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerDriver : DriverBehaviour
{
    // SerializeField
    [SerializeField, Min(0.0f), Header("速度上限")]
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

        // アクションマップの取得
        var car = InputActionsName.PlayerDriverControlsActionMapsName.Car;
        var actionMap = playerInput.actions.FindActionMap(InputActionsName.GetActionMapsName(car));

        // アクションの取得
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

        // ステアリング駆動を0.0fにする
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

        // アクセル駆動を0.0fにする
        FinishAccelerator(_wheelsParameter);
    }

    private void BrakeStarted(InputAction.CallbackContext context)
    {
        StartCoroutine(Brake());
    }

    private void BrakeCanceled(InputAction.CallbackContext context)
    {
        _isBrake = false;

        // ブレーキを0.0fにする
        FinishBrake(_wheelsParameter);
    }

    /// <summary>
    /// ステアリング
    /// </summary>
    /// <returns></returns>
    private IEnumerator Steering(float value)
    {
        // 操舵中なら切り上げる
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
    /// アクセル
    /// </summary>
    /// <returns></returns>
    private IEnumerator Accelerator(float value)
    {
        // 移動中なら切り上げる
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
    /// 上限速度付きアクセル駆動
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    /// <param name="direction">方向</param>
    /// <param name="speedLimit">上限速度</param>
    private void ApplyAcceleratorWithSpeedLimit(WheelsParameter wheelsParameter, float direction, float speedLimit)
    {
        base.ApplyAccelerator(wheelsParameter, direction);

        // アクセルを適用する車輪配列
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // アクセルを反映
        foreach (var acceleWheel in acceleWheels)
        {
            if (speedLimit < Speed)
            {
                // 前進速度が上限速度を超過している場合は減速
                acceleWheel.WheelCollider.motorTorque = 0;
                continue;
            }
        }
    }

    /// <summary>
    /// ブレーキ
    /// </summary>
    private IEnumerator Brake()
    {
        // 減速中なら切り上げる
        if (_isBrake) { yield break; }

        _isBrake = true;

        // FixedUpdate
        while (_isBrake)
        {
            ApplyBrake(_wheelsParameter);
            Debug.Log("減速中");

            yield return new WaitForFixedUpdate();
        }
    }
}
