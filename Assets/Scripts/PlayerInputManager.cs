using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput _map;
    private int _controllerIndex;
    [SerializeField, ReadOnly] private PlayerBehaviour _player;

    public bool IsInputLocked { get; private set; }
    public PlayerBehaviour Player { get; private set; }

    private void Start()
    {
        _map = GetComponent<PlayerInput>();

        IsInputLocked = false;
        _controllerIndex = _map.devices[0].deviceId;

        _player = GameManager.Instance.ConnectPlayer( _controllerIndex );
        _player.PlayerInputs = this;

        //Vibrate(.5f);
    }

    /*
    private void Vibrate(float seconds) => StartCoroutine(WaitForVibrate(seconds));

    private IEnumerator WaitForVibrate(float seconds)
    {
        Gamepad.all[_controllerIndex].SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(seconds);
        Gamepad.all[_controllerIndex].SetMotorSpeeds(0f, 0f);
    }
    */
}
