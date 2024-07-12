using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Define;

public class CursorController : BaseController
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider.isTrigger = false;

        RigidBody.mass = 0;
        RigidBody.simulated = true;
        RigidBody.gravityScale = 0;
        RigidBody.freezeRotation = true;

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        return true;
    }

    void Update()
    {
        Managers.Input.OnUpdate();
    }

    void OnMouseEvent(EMouseEvent type)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (type == EMouseEvent.Click)
        {
            Vector3 cellSize = Managers.Map.CellGrid.cellSize;
            Vector3Int cellPos = Managers.Map.WorldToCell(mousePos);
            transform.position = Managers.Map.CellToWorld(cellPos) + new Vector3(cellSize.x / 2 - 1, cellSize.y / 2, 0);

            switch (Managers.Game.CursorType)
            {
                case ECursorType.Move:
                    Debug.Log("Move");
                    break;
                case ECursorType.Skill:
                    break;
            }
        }
        // TODO
        // 드래그 시 화면 이동

    }
}
