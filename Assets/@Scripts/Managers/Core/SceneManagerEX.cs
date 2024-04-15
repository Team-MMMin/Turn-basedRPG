using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type, Transform parents = null)
    {
        switch (CurrentScene.SceneType) 
        {
            case Define.Scene.MenuScene:
                Managers.Clear();
                SceneManager.LoadScene(GetSceneName(type));
                break;
            case Define.Scene.GameScene:
                Managers.Clear();
                SceneManager.LoadScene(GetSceneName(type));
                break;
        }
    }

    string GetSceneName(Define.Scene type) 
    {
        return Enum.GetName(typeof(Define.Scene), type);
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
