using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuScene : UI_Scene
{
    bool isPreload = false;

    void Awake()
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            if (count == totalCount)
            {
                isPreload = true;
            }
        });

        return true;
    }
}
