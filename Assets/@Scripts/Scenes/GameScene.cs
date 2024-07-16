using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        GameObject cursor = Managers.Resource.Instantiate("Cursor");

        return true;
    }

    public override void Clear()
    {

    }
}
