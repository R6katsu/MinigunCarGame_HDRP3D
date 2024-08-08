using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

static public class InputActionsName
{
    // ����ł�PlayerDriverControls�ɂ����Ή����Ă��Ȃ�

    static private PlayerDriverControls _playerDriverControls = new();

    public enum PlayerDriverControlsActionMapsName
    {
        Car
    }

    public enum PlayerDriverControlsActionsName
    {
        Horizontal,
        Vertical,
        Brake
    }

    static public string GetActionMapsName(PlayerDriverControlsActionMapsName actionMapsName)
    {

        return _playerDriverControls.asset.actionMaps[(int)actionMapsName].name;
    }

    static public string GetActionsName(InputActionMap inputActionMap, PlayerDriverControlsActionsName actionMapsName)
    {
        if (inputActionMap.asset.name != _playerDriverControls.asset.name) { return string.Empty; }

        return inputActionMap.actions[(int)actionMapsName].name;
    }
}
