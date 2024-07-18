using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class InputManager
{
    public Action<EMouseEvent> MouseAction = null;

    bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (MouseAction == null)
            return;

        if (Input.GetMouseButton(0))
        {
            if (_pressed == false)
                _pressedTime = Time.time;

            MouseAction.Invoke(EMouseEvent.Drag);
            _pressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_pressed)
            {
                if (Time.time < _pressedTime + 0.2f)
                    MouseAction.Invoke(EMouseEvent.Click);
            }

            _pressed = false;
            _pressedTime = 0;
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}
