using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour _player1, _player2;
    private Dictionary<PlayerBehaviour, int> _players = new();

    #region Singleton
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    private void InitSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Awake()
    {
        InitSingleton();
        _players.Add(_player1, -1);
        _players.Add(_player2, -1);
        InputSystem.onDeviceChange += DisconnectPlayer;
    }

    public PlayerBehaviour ConnectPlayer(int controllerId)
    {

        if (_players[_player1] == -1)
        {
            _players[_player1] = controllerId;
            _player1.PlayerIndex = 0;
            Debug.Log("Connect player 1");
            return _player1;
        }
        else if( _players[_player2] == -1) 
        {
            _players[_player2] = controllerId;
            _player1.PlayerIndex = 1;
            Debug.Log("Connect player 2");
            return _player2;
        }
        else
            return null;
    }

    public void DisconnectPlayer(InputDevice device, InputDeviceChange change)
{
        if (change != InputDeviceChange.Disconnected)
            return;

        if(_players[_player1] == device.deviceId)
        {
            _players[_player1] = -1;
            Destroy(_player1.PlayerInputs.gameObject);
            Debug.Log("Disconnect player 1");
        }
        else if (_players[_player2] == device.deviceId)
        {
            _players[_player2] = -1;
            Destroy(_player2.PlayerInputs.gameObject);
            Debug.Log("Disconnect player 2");
        }
    }
}
