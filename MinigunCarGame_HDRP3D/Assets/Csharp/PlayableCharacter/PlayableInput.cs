using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayableInput : MonoBehaviour
{
    #region Field
    [SerializeField, Header("�����ȓ��͂�L���ɂ���")]
    private bool _isHorizontal = false;

    [SerializeField, Header("�����ȓ��͂�L���ɂ���")]
    private bool _isVertical = false;

    [SerializeField, Header("�W�����v���͂�L���ɂ���")]
    private bool _isJump = false;

    [Tooltip("����")]
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

        // �A�N�V�����}�b�v�̎擾
        var actionMap = _playerInput.actions.FindActionMap("Player");

        // Horizontal�̐ݒ�
        var horizontalAction = actionMap.FindAction("Horizontal");
        horizontalAction.started += HorizontalStarted;
        horizontalAction.canceled += HorizontalCanceled;

        // Vertical�̐ݒ�
        var verticalAction = actionMap.FindAction("Vertical");
        verticalAction.started += VerticalStarted;
        verticalAction.canceled += VerticalCanceled;

        // Jump�̐ݒ�
        var jumpAction = actionMap.FindAction("Jump");
        jumpAction.started += JumpStarted;
        jumpAction.canceled += JumpCanceled;
    }

    /// <summary>
    /// �����ȓ��͂��J�n
    /// </summary>
    /// <param name="context"></param>
    private void HorizontalStarted(InputAction.CallbackContext context)
    {
        if (!_isHorizontal) { return; }

        HorizontalStartedAction?.Invoke(context);
    }

    /// <summary>
    /// �����ȓ��͂��I��
    /// </summary>
    /// <param name="context"></param>
    private void HorizontalCanceled(InputAction.CallbackContext context)
    {
        if (!_isHorizontal) { return; }

        HorizontalCanceledAction?.Invoke(context);
    }

    /// <summary>
    /// �����ȓ��͂��J�n
    /// </summary>
    /// <param name="context"></param>
    private void VerticalStarted(InputAction.CallbackContext context)
    {
        if (!_isVertical) { return; }

        VerticalStartedAction?.Invoke(context);
    }

    /// <summary>
    /// �����ȓ��͂��I��
    /// </summary>
    private void VerticalCanceled(InputAction.CallbackContext context)
    {
        if (!_isVertical) { return; }

        VerticalCanceledAction?.Invoke(context);
    }

    /// <summary>
    /// �W�����v���͂��J�n
    /// </summary>
    /// <param name="context"></param>
    private void JumpStarted(InputAction.CallbackContext context)
    {
        if (!_isJump) { return; }

        JumpStartedAction?.Invoke(context);
    }

    /// <summary>
    /// �W�����v���͂��I��
    /// </summary>
    private void JumpCanceled(InputAction.CallbackContext context)
    {
        if (!_isJump) { return; }

        JumpCanceledAction?.Invoke(context);
    }
    #endregion
}
