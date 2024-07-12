using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    ECursorType _cursorType;
    public ECursorType CursorType
    {
        get { return _cursorType; }
        set 
        {
            _cursorType = value;
            OnCursorChanged?.Invoke(_cursorType);
        }
    }

    public event Action<ECursorType> OnCursorChanged;
}
