using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class BaseScene : MonoBehaviour
{
    public Scene SceneType { get; protected set; } = Scene.Unknown;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        
    }

    public abstract void Clear();
}
