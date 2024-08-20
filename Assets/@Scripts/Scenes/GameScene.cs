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

        #region Test
        Managers.Map.LoadMap("001");
        Managers.Object.Spawn<PlayerUnitController>(new Vector3(0, 4.5f, 0), Define.PLAYER_UNIT_WARRIOR_ID);
        Managers.Object.Spawn<PlayerUnitController>(new Vector3(0, 5.5f, 0), Define.PLAYER_UNIT_WARRIOR_ID + 1);
        Managers.Object.Spawn<MonsterController>(new Vector3(0, 6.5f, 0), Define.MONSTER_WARRIOR_ID);
        #endregion

        // 카메라
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        Managers.Game.Camera = camera;
        // 커서
        GameObject cursor = Managers.Resource.Instantiate("Cursor");
        Managers.Game.Cursor = cursor.GetOrAddComponent<CursorController>();

        Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.Game.GameState = Define.EGameState.PlayerTurn;
        
        return true;
    }

    public override void Clear()
    {

    }
}
