using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput _map;
    private int _controllerIndex;
    [SerializeField, ReadOnly] private PlayerBehaviour _player;

    public bool IsInputLocked { get; private set; }
    public PlayerInput Map => _map;

    private void Start()
    {
        _map = GetComponent<PlayerInput>();

        IsInputLocked = false;
        _controllerIndex = _map.devices[0].deviceId;

        _player = GameManager.Instance.ConnectPlayer( _controllerIndex );
        _player.OnConnectController(this);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _player.OnMove(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            _player.OnJump(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        _player.OnInteract(context);
    }
}
