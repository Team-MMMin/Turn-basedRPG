using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class InputManager
{
    public Action<EMouseEvent, bool> MouseAction = null;

    bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (MouseAction == null)
            return;

        // 왼쪽 마우스
        if (Input.GetMouseButton(0))
        {
            if (_pressed == false)
                _pressedTime = Time.time;

            MouseAction.Invoke(EMouseEvent.Drag, true);
            _pressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_pressed)
            {
                if (Time.time < _pressedTime + 0.2f)
                    MouseAction.Invoke(EMouseEvent.Click, true);
            }

            _pressed = false;
            _pressedTime = 0;
        }

        // 오른쪽 마우스
        if (Input.GetMouseButton(1))
        {
            if (_pressed == false)
                _pressedTime = Time.time;

            _pressed = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (_pressed)
            {
                // 클릭이라면 행동 캔슬
                if (Time.time < _pressedTime + 0.2f)
                {
                    MouseAction.Invoke(EMouseEvent.Click, false);
                }
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
