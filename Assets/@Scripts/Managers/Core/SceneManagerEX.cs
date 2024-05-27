using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.EScene type, Transform parents = null)
    {
        switch (CurrentScene.SceneType) 
        {
            case Define.EScene.MenuScene:
                Managers.Clear();
                SceneManager.LoadScene(GetSceneName(type));
                break;
            case Define.EScene.GameScene:
                Managers.Clear();
                SceneManager.LoadScene(GetSceneName(type));
                break;
        }
    }

    string GetSceneName(Define.EScene type) 
    {
        return Enum.GetName(typeof(Define.EScene), type);
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
