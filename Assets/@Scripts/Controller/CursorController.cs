using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class CursorController : InitBase
{
    SpriteRenderer _sprite;
    public float Speed { get; set; } = 0.5f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _sprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingOrder = SortingLayers.CURSOR;

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.Game.PlayerActionState = EPlayerActionState.Spawn; // Spawn Test
        return true;
    }

    void Update()
    {
        Managers.Input.OnUpdate();
        UpdateCursor();
    }

    void UpdateCursor()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        if (IsValidPosition(worldPos, true) == false)
            return;

        EPlayerActionState actionState = Managers.Game.PlayerActionState;
        CreatureController unit = Managers.Game.CurrentUnit;
        
        if (actionState == EPlayerActionState.Skill)    // 커서가 캐스팅 범위 내에 있는지 확인한다
        {
            List<Vector3> castingRange = unit.CastingSkill.CastingRange;
            if (castingRange == null)
                return;

            if (IsValidRange(worldPos, castingRange))
            {
                unit.TargetPos = worldPos;
                unit.CastingSkill.SetSizeRange();
            }
            else
                unit.CastingSkill.ClearSizeRange();
        }
        else if (actionState == EPlayerActionState.Move)    // 커서가 이동 범위 내에 있는지 확인한다
        {

        }

        ShowCreatureInfoUI(worldPos);
        transform.position = worldPos;
    }

    void OnMouseEvent(EMouseEvent type, bool isLeftMouseClick = true)
    {
        // 플레이어의 턴일 때 마우스 조작이 가능하다
        if (Managers.Game.GameState != EGameState.PlayerTurn)
            return;

        // 왼쪽 마우스 클릭 여부
        if (isLeftMouseClick == false)
        {
            OnClickRigthMouthButton();
            return;
        }

        // 왼쪽 마우스 클릭 이벤트
        if (type == EMouseEvent.Click)
        {
            Vector3 worldPos = GetMouseWorldPosition();
            transform.position = worldPos;

            switch (Managers.Game.PlayerActionState)
            {
                case EPlayerActionState.None:
                    HandleNoneAction(worldPos);
                    break;
                case EPlayerActionState.Spawn:
                    HandleSpawnAction(worldPos);
                    break;
                case EPlayerActionState.Move:
                    HandleMoveAction(worldPos);
                    break;
                case EPlayerActionState.Skill:
                    HandleSkillAction(worldPos);
                    break;
            }
        }
        else if (type == EMouseEvent.Drag)
        {
            Camera.main.transform.Translate(-Input.GetAxis("Mouse X") * Speed, -Input.GetAxis("Mouse Y") * Speed, 0);
        }
    }

    void OnClickRigthMouthButton()  // 오른쪽 마우스 클릭 이벤트
    {
        CreatureController unit = Managers.Game.CurrentUnit;
        if (unit == null)
            return;

        EPlayerActionState playerActionState = Managers.Game.PlayerActionState;
        if (playerActionState != EPlayerActionState.None)
        {
            if (playerActionState == EPlayerActionState.Hand)
                Managers.Game.PlayerActionState = EPlayerActionState.None;
            else
                Managers.Game.PlayerActionState = EPlayerActionState.Hand;

            unit.CreatureState = ECreatureState.Idle;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        return Managers.Map.GetTilePosition(mousePos, Vector3Int.zero);
    }

    void ShowCreatureInfoUI(Vector3 worldPos)   // 해당 위치에 크리처가 있다면 UI 정보를 띄운다
    {
        BaseController obj = Managers.Map.GetObject(worldPos);
        if (obj != null)
        {
            CreatureController unit = obj.GetComponent<CreatureController>();
            Managers.UI.GetSceneUI<UI_GameScene>().SetInfo(unit);
        }
    }

    void HandleNoneAction(Vector3 worldPos)
    {
        // 조작할 플레이어 유닛을 선택한다
        if (IsValidPosition(worldPos, true))
        {
            BaseController obj = Managers.Map.GetObject(worldPos);
            if (obj == null)
                return;
            
            CreatureController unit = obj.GetComponent<CreatureController>();
            if (unit == null)
                return;

            if (unit.CreatureType == ECreatureType.PlayerUnit && unit.CreatureState != ECreatureState.EndTurn)
            {
                Managers.Game.CurrentUnit = unit;
                Managers.Game.PlayerActionState = EPlayerActionState.Hand;
            }
        }
    }

    void HandleSpawnAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos))
        {
            PlayerUnitController playerUnit = Managers.Object.Spawn<PlayerUnitController>(worldPos, PLAYER_UNIT_WARRIOR_ID);
            Managers.Game.CurrentUnit = playerUnit;
        }
        
        Managers.Game.PlayerActionState = EPlayerActionState.Hand;
    }

    void HandleMoveAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos) && Managers.Game.CurrentUnit != null)
        {
            PlayerUnitController playerUnit = Managers.Game.CurrentUnit.GetComponent<PlayerUnitController>();
            if (playerUnit != null)
            {
                playerUnit.DestPos = worldPos;
                playerUnit.CreatureState = ECreatureState.Move;
            }
        }
     
        Managers.Game.PlayerActionState = EPlayerActionState.Hand;
    }

    void HandleSkillAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos, true) && Managers.Game.CurrentUnit != null && Managers.Game.CurrentUnit.CastingSkill != null)
        {
            List<Vector3> castingRange = Managers.Game.CurrentUnit.CastingSkill.CastingRange;
            if (castingRange == null)
                return;

            if (IsValidRange(worldPos, castingRange))
            {
                Managers.Game.CurrentUnit.CreatureState = ECreatureState.Skill;
                Managers.Game.PlayerActionState = EPlayerActionState.Hand;
            }
            else
            {
                Debug.Log("캐스팅 범위내에서 스킬을 사용해주세요.");
            }
        }
    }

    bool IsValidPosition(Vector3 worldPos, bool ignoreObjects = false)
    {
        return Managers.Map.CanGo(worldPos, ignoreObjects);
    }

    bool IsValidRange(Vector3 worldPos, List<Vector3> range)
    {
        foreach (var pos in range)
        {
            if (worldPos == pos)
                return true;
        }

        return false;
    }
}