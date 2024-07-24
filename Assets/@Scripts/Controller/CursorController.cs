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
    }

    void OnMouseEvent(EMouseEvent type)
    {
        if (type == EMouseEvent.Click)
        {
            Vector3 worldPos = GetMouseWorldPosition();
            transform.position = worldPos;
            switch (Managers.Game.ActionState)
            {
                case EActionState.Spawn:
                    HandleSpawnAction(worldPos);
                    break;
                case EActionState.Move:
                    HandleMoveAction(worldPos);
                    break;
                case EActionState.Skill:
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
        mousePos.z = 0; // 임시적으로 0으로 설정했다
        
        Vector3Int cellPos = Managers.Map.WorldToCell(mousePos);
        Vector3 worldPos = Managers.Map.CellGrid.GetCellCenterWorld(cellPos);
        Vector3 MouseWorldPos = worldPos + new Vector3(0, -0.25f, 0);   // 중앙에서 약간 아래로 피벗 보정

        return MouseWorldPos;
    }

    void HandleSpawnAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos))
        {
            PlayerUnitController playerUnit = Managers.Object.Spawn<PlayerUnitController>(worldPos, PLAYER_UNIT_WARRIOR_ID);
            Managers.Game.CurrentUnit = playerUnit;
            Managers.Game.ActionState = EActionState.None;
        }
    }

    void HandleMoveAction(Vector3 worldPos)
    {
        if (IsValidPosition(worldPos) && Managers.Game.CurrentUnit != null)
        {
            //Managers.Game.CurrentUnit.FindPathAndMoveToCellPos(worldPos, Managers.Game.CurrentUnit.Mov);
            PlayerUnitController playerUnit = Managers.Game.CurrentUnit.GetComponent<PlayerUnitController>();
            if (playerUnit != null)
                playerUnit.DestPos = worldPos;
            Managers.Game.ActionState = EActionState.None;
        }
    }

    bool IsValidPosition(Vector3 worldPos)
    {
        return Managers.Map.CanGo(worldPos);
    }
}
