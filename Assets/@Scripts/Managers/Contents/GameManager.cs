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
            GameStateChanged?.Invoke(_gameState);
        }
    }

    EPlayerActionState _playerActionState;
    public EPlayerActionState PlayerActionState // 플레이어 행동 상태 (소환 중, 유닛 행동 컨트롤 중 등)
    {
        get { return _playerActionState; }
        set 
        {
            _playerActionState = value;
            PlayerActionStateChanged?.Invoke(_playerActionState);
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

    public event Action<EPlayerActionState> PlayerActionStateChanged;
    public event Action<EGameState> GameStateChanged;

    public IEnumerator CoHandletMonsterTurn()   // 랜덤으로 몬스터의 턴을 설정한다
    {
        Debug.Log("SetMonsterTurnPriority");
        List<MonsterController> monsters = new List<MonsterController>(Managers.Object.Monsters);
        List<MonsterController> selectedMonsters = new List<MonsterController>();

        int count = monsters.Count;
        for (int i = 0; i < count; i++)
        {
            int idx = UnityEngine.Random.Range(0, monsters.Count);
            selectedMonsters.Add(monsters[idx]);
            CurrentUnit = monsters[idx];
            monsters[idx].IsMyTurn = true;
            monsters.RemoveAt(idx);
            
            yield return new WaitForSeconds(0.7f);
        }
    }
}
