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

        Managers.Game.ActionState = EActionState.Spawn; // Spawn Test

        return true;
    }

    void Update()
    {
        Managers.Input.OnUpdate();
        UpdateCursorPosition();
    }

    void UpdateCursorPosition()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        if (IsValidPosition(worldPos, true) == false)
            return;

        // 커서가 캐스팅 범위 내에 있는지 확인한다
        if (Managers.Game.ActionState == EActionState.Skill)
        {
            if (worldPos == Managers.Game.CursorPos)
                return;

            List<Vector3> castingRange = Managers.Game.CurrentUnit.CastingSkill.CastingRange;
            if (castingRange == null)
                return;

            if (IsValidRange(worldPos, castingRange))
            {
                Managers.Game.CursorPos = worldPos;
                Managers.Game.CurrentUnit.TargetPos = worldPos;
                Managers.Game.CurrentUnit.CastingSkill.SetSizeRange();
            }
            else
                Managers.Game.CurrentUnit.CastingSkill.ClearSizeRange();
        }

        // 커서가 이동 범위 내에 있는지 확인한다
        if (Managers.Game.ActionState == EActionState.Move)
        {
            if (worldPos == Managers.Game.CursorPos)
                return;
        }

        ShowCreatureInfoUI(worldPos);
        transform.position = worldPos;
    }

    void OnMouseEvent(EMouseEvent type, bool isLeftMouseClick = true)
    {
        if (Managers.Game.GameState != EGameState.PlayerTurn)
            return;

        if (isLeftMouseClick == false)
        {
            HandleRigthMouth();
            return;
        }

        if (type == EMouseEvent.Click)
        {
            Vector3 worldPos = GetMouseWorldPosition();
            switch (Managers.Game.ActionState)
            {
                case EActionState.None:
                    HandleNoneAction(worldPos);
                    break;
                case EActionState.Spawn:
                    HandleSpawnAction(worldPos);
                    break;
                case EActionState.Move:
                    HandleMoveAction(worldPos);
                    break;
                case EActionState.Skill:
                    HandleSkillAction(worldPos);
                    break;
            }
        }
        else if (type == EMouseEvent.Drag)
        {
            Camera.main.transform.Translate(-Input.GetAxis("Mouse X") * Speed, -Input.GetAxis("Mouse Y") * Speed, 0);
        }
    }

    void HandleRigthMouth()
    {
        if (Managers.Game.CurrentUnit == null)
            return;

        Managers.Game.CurrentUnit.CreatureState = ECreatureState.Idle;
        
        // 현재 유닛의 행동을 선택하지 않았다면 조작을 취소한다.
        if (Managers.Game.ActionState == EActionState.Hand)
        {
            Managers.Game.ActionState = EActionState.None;
            return;
        }
        else if (Managers.Game.ActionState == EActionState.None)
            return;

        Managers.Game.ActionState = EActionState.Hand;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        return Managers.Map.GetTilePosition(mousePos, Vector3Int.zero, new Vector3(0, -0.25f, 0));
    }

    void ShowCreatureInfoUI(Vector3 worldPos)
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
        if (IsValidPosition(worldPos, true))
        {
            transform.position = worldPos;

            BaseController obj = Managers.Map.GetObject(worldPos);
            if (obj == null)
                return;
            
            CreatureController unit = obj.GetComponent<CreatureController>();
            if (unit == null)
                return;

            if (unit.CreatureType == ECreatureType.PlayerUnit && unit.CreatureState != ECreatureState.EndTurn)
            {
                Managers.Game.CurrentUnit = unit;
                Managers.Game.ActionState = EActionState.Hand;
            }
        }
    }

    void HandleSpawnAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos))
        {
            transform.position = worldPos;
            PlayerUnitController playerUnit = Managers.Object.Spawn<PlayerUnitController>(worldPos, PLAYER_UNIT_WARRIOR_ID);
            Managers.Game.CurrentUnit = playerUnit;
        }
        
        Managers.Game.ActionState = EActionState.Hand;
    }

    void HandleMoveAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos) && Managers.Game.CurrentUnit != null)
        {
            transform.position = worldPos;
            PlayerUnitController playerUnit = Managers.Game.CurrentUnit.GetComponent<PlayerUnitController>();
            if (playerUnit != null)
            {
                playerUnit.DestPos = worldPos;
                playerUnit.CreatureState = ECreatureState.Move;
            }
        }
     
        Managers.Game.ActionState = EActionState.Hand;
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
                transform.position = worldPos;
                Managers.Game.CurrentUnit.CreatureState = ECreatureState.Skill;
                Managers.Game.ActionState = EActionState.Hand;
            }
            else
            {
                Debug.Log("캐스팅 범위내에서 스킬을 사용해주세요.");
                transform.position = worldPos;
                return;
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