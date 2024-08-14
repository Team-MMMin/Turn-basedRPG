using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    EGameState _gameState;
    public EGameState GameState // 턴 상태, 승패 여부
    {
        get { return _gameState; }
        set 
        {
            _gameState = value;
            OnGameStateChanged?.Invoke(_gameState);
        }
    }

    EPlayerActionState _playerActionState;
    public EPlayerActionState PlayerActionState // 플레이어 행동 상태 (소환 중, 유닛 행동 컨트롤 중 등)
    {
        get { return _playerActionState; }
        set 
        {
            _playerActionState = value;
            OnActionStateChanged?.Invoke(_playerActionState);
        }
    }

    CreatureController _currentUnit;
    public CreatureController CurrentUnit   // 현재 행동 중인 유닛
    {
        get { return _currentUnit; }
        set 
        {
            _currentUnit = value;
            if (_currentUnit != null)
                Camera.Target = _currentUnit;
            else
                Camera.Target = null;
        }
    }

    public CameraController Camera;
    public CursorController Cursor;

    public event Action<EPlayerActionState> OnActionStateChanged;
    public event Action<EGameState> OnGameStateChanged;
}
