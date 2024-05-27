using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.MenuScene;
        return true;
    }

    public override void Clear()
    {

    }
}
