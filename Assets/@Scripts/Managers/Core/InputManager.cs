using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class InputManager
{
    public Action<EMouseEvent> MouseAction = null;

    public void OnUpdate()
    {
        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                MouseAction.Invoke(EMouseEvent.Click);
            }
            else
            {
                MouseAction.Invoke(EMouseEvent.Press);
            }
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}
