using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuScene : UI_Scene
{
    enum Images
    {
        BackgroundImage,
    }

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        StartLoadAssets();

        BindImage(typeof(Images));
        BindEvent(GetImage((int)Images.BackgroundImage).gameObject, OnClickBackgroundImage);

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

    void OnClickBackgroundImage()
    {
        Debug.Log("Change GameScene");
        Managers.Scene.LoadScene(Define.EScene.GameScene);
    }
}
