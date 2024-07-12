using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class InputManager
{
    public Action<EMouseEvent> MouseAction = null;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey == false)
            return;

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                MouseAction.Invoke(EMouseEvent.Click);
            }
            // TODO
            // 마우스 드래그 인식
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}
