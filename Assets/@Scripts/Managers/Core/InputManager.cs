using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class InputManager
{
    public Action<EMouseEvent> MouseAction = null;
    public Action<EMouseEvent> MouseDragAction = null;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey == false)
            return;

        if (MouseAction != null && MouseDragAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                MouseAction.Invoke(EMouseEvent.Click);
                MouseDragAction.Invoke(EMouseEvent.Drag);
            }
        }
    }

    public void Clear()
    {
        MouseAction = null;
        MouseDragAction = null;
    }
}
