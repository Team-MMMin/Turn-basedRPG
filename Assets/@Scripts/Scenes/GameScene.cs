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
        Managers.UI.ShowSceneUI<UI_GameScene>();

        #region Test
        Managers.Map.LoadMap("001");
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        Managers.Game.Camera = camera;
        GameObject cursor = Managers.Resource.Instantiate("Cursor");
        Managers.Object.Spawn<PlayerUnitController>(new Vector3(0, 4.5f, 0), Define.PLAYER_UNIT_WARRIOR_ID);
        Managers.Object.Spawn<PlayerUnitController>(new Vector3(0, 5.5f, 0), Define.PLAYER_UNIT_WARRIOR_ID + 1);
        Managers.Object.Spawn<MonsterController>(new Vector3(0, 6.5f, 0), Define.MONSTER_WARRIOR_ID);

        Managers.Game.GameState = Define.EGameState.PlayerTurn;
        #endregion

        return true;
    }

    public override void Clear()
    {

    }
}
