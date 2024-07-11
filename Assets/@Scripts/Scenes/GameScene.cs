using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.EScene.GameScene;

        // Test
        Managers.Map.LoadMap("001");
        
        return true;
    }

    public override void Clear()
    {

    }
}
