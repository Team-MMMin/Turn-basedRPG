using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Define;

public class CursorController : InitBase
{
    SpriteRenderer _sprite;
    public float Speed { get; set; } = 0.2f;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _sprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingOrder = SortingLayers.CURSOR;

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.Input.MouseDragAction -= OnMouseDragEvent;
        Managers.Input.MouseDragAction += OnMouseDragEvent;

        return true;
    }

    void Update()
    {
        Managers.Input.OnUpdate();
    }

    void OnMouseEvent(EMouseEvent type)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // 임시적으로 0으로 설정했다

        if (type == EMouseEvent.Click)
        {
            Vector3 cellSize = Managers.Map.CellGrid.cellSize;
            Vector3Int cellPos = Managers.Map.WorldToCell(mousePos);

            switch (Managers.Game.CursorType)
            {
                case ECursorType.Move:
                    Debug.Log("Move");
                    // TODO
                    // 범위 내에서만 커서를 두게 한다.
                    Vector3 pos = Managers.Map.CellToWorld(cellPos) + new Vector3(cellSize.x / 2 - 1, cellSize.y / 2, 0);
                    transform.position = pos;
                    break;
                case ECursorType.Skill:
                    break;
            }
        }
    }

    void OnMouseDragEvent(EMouseEvent type)
    {
        Managers.Game.CursorType = ECursorType.Hand;
        if (type == EMouseEvent.Drag)
            Camera.main.transform.Translate(-Input.GetAxis("Mouse X") * Speed, -Input.GetAxis("Mouse Y") * Speed, 0);
    }
}
