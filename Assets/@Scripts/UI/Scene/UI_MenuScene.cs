using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuScene : UI_Scene
{
    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        StartLoadAssets();

        return true;
    }

    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            if (count == totalCount)
            {
                Managers.Data.Init();
            }
        });
    }
}
