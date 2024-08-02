using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Define;

public class CursorController : InitBase
{
    SpriteRenderer _sprite;
    public float Speed { get; set; } = 0.5f;

    public Action<Vector3> OnSkillSizeChanged;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _sprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingOrder = SortingLayers.CURSOR;

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.Game.Cursor = this;

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

        if (Managers.Game.ActionState == EActionState.Skill)
        {
            List<Vector3> castingRange = Managers.Game.CurrentUnit.CastingSkill.CastingRange;
            if (castingRange == null)
                return;
            
            foreach (var range in castingRange)
            {
                if (worldPos == range)
                {
                    transform.position = worldPos;
                    OnSkillSizeChanged?.Invoke(worldPos);
                    return;
                }
            }
        }

        transform.position = worldPos;
    }

    void OnMouseEvent(EMouseEvent type)
    {
        if (type == EMouseEvent.Click)
        {
            Vector3 worldPos = GetMouseWorldPosition();
            switch (Managers.Game.ActionState)
            {
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

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        return Managers.Map.GetTilePosition(mousePos, Vector3Int.zero, new Vector3(0, -0.25f, 0));
    }

    void HandleSpawnAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos))
        {
            transform.position = worldPos;
            PlayerUnitController playerUnit = Managers.Object.Spawn<PlayerUnitController>(worldPos, PLAYER_UNIT_WARRIOR_ID);
            Managers.Game.CurrentUnit = playerUnit;
        }
        
        Managers.Game.ActionState = EActionState.None;
    }

    void HandleMoveAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos) && Managers.Game.CurrentUnit != null)
        {
            transform.position = worldPos;
            PlayerUnitController playerUnit = Managers.Game.CurrentUnit.GetComponent<PlayerUnitController>();
            if (playerUnit != null)
                playerUnit.DestPos = worldPos;
        }
     
        Managers.Game.ActionState = EActionState.None;
    }

    void HandleSkillAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos, true) && Managers.Game.CurrentUnit != null && Managers.Game.CurrentUnit.CastingSkill != null)
        {
            transform.position = worldPos;
            Vector3Int cellPos = Managers.Map.WorldToCell(worldPos);
            Managers.Game.CurrentUnit.TargetCellPos = cellPos;
        }

        Managers.Game.ActionState = EActionState.None;
        Managers.Game.CurrentUnit.CastingSkill = null;
    }

    bool IsValidPosition(Vector3 worldPos, bool ignoreObjects = false)
    {
        return Managers.Map.CanGo(worldPos, ignoreObjects);
    }
}
