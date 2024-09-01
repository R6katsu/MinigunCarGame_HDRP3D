using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayableInput : MonoBehaviour
{
    #region Field
    [SerializeField, Header("水平な入力を有効にする")]
    private bool _isHorizontal = false;

    [SerializeField, Header("垂直な入力を有効にする")]
    private bool _isVertical = false;

    [SerializeField, Header("ジャンプ入力を有効にする")]
    private bool _isJump = false;

    [Tooltip("入力")]
    protected private PlayerInput _playerInput = null;
    #endregion

    #region Get/Set
    public Action<InputAction.CallbackContext> HorizontalStartedAction { private get; set; } = null;
    public Action<InputAction.CallbackContext> HorizontalCanceledAction { private get; set; } = null;
    public Action<InputAction.CallbackContext> VerticalStartedAction { private get; set; } = null;
    public Action<InputAction.CallbackContext> VerticalCanceledAction { private get; set; } = null;
    public Action<InputAction.CallbackContext> JumpStartedAction { private get; set; } = null;
    public Action<InputAction.CallbackContext> JumpCanceledAction { private get; set; } = null;
    #endregion

    #region Method
    private void OnEnable()
    {
        _playerInput = GetComponent<PlayerInput>();     // RequireComponent
        _playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

        // アクションマップの取得
        var actionMap = _playerInput.actions.FindActionMap("Player");

        // Horizontalの設定
        var horizontalAction = actionMap.FindAction("Horizontal");
        horizontalAction.started += HorizontalStarted;
        horizontalAction.canceled += HorizontalCanceled;

        // Verticalの設定
        var verticalAction = actionMap.FindAction("Vertical");
        verticalAction.started += VerticalStarted;
        verticalAction.canceled += VerticalCanceled;

        // Jumpの設定
        var jumpAction = actionMap.FindAction("Jump");
        jumpAction.started += JumpStarted;
        jumpAction.canceled += JumpCanceled;
    }

    /// <summary>
    /// 水平な入力を開始
    /// </summary>
    /// <param name="context"></param>
    private void HorizontalStarted(InputAction.CallbackContext context)
    {
        if (!_isHorizontal) { return; }

        HorizontalStartedAction?.Invoke(context);
    }

    /// <summary>
    /// 水平な入力を終了
    /// </summary>
    /// <param name="context"></param>
    private void HorizontalCanceled(InputAction.CallbackContext context)
    {
        if (!_isHorizontal) { return; }

        HorizontalCanceledAction?.Invoke(context);
    }

    /// <summary>
    /// 垂直な入力を開始
    /// </summary>
    /// <param name="context"></param>
    private void VerticalStarted(InputAction.CallbackContext context)
    {
        if (!_isVertical) { return; }

        VerticalStartedAction?.Invoke(context);
    }

    /// <summary>
    /// 垂直な入力を終了
    /// </summary>
    private void VerticalCanceled(InputAction.CallbackContext context)
    {
        if (!_isVertical) { return; }

        VerticalCanceledAction?.Invoke(context);
    }

    /// <summary>
    /// ジャンプ入力を開始
    /// </summary>
    /// <param name="context"></param>
    private void JumpStarted(InputAction.CallbackContext context)
    {
        if (!_isJump) { return; }

        JumpStartedAction?.Invoke(context);
    }

    /// <summary>
    /// ジャンプ入力を終了
    /// </summary>
    private void JumpCanceled(InputAction.CallbackContext context)
    {
        if (!_isJump) { return; }

        JumpCanceledAction?.Invoke(context);
    }
    #endregion
}
