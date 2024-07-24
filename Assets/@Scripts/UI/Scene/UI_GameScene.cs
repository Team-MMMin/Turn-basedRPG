using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    #region enum
    enum Buttons
    {
        MoveButton,
        SkillButton,
        EndTurnButton,
        SettingButton,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        BindButton(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.MoveButton).gameObject, OnClickMoveButton);
        BindEvent(GetButton((int)Buttons.SkillButton).gameObject, OnClickSkillButton);
        BindEvent(GetButton((int)Buttons.EndTurnButton).gameObject, OnClickEndTurnButton);
        BindEvent(GetButton((int)Buttons.SettingButton).gameObject, OnClickSettingButton);

        return true;
    }

    void OnClickMoveButton()
    {
        Debug.Log("OnClickMoveButton");
        Managers.Game.ActionState = EActionState.Move;
    }

    void OnClickSkillButton()
    {
        Debug.Log("OnClickSkillButton");
        Managers.Game.ActionState = EActionState.Skill;
    }

    void OnClickEndTurnButton()
    {
        Debug.Log("OnClickEndTurnButton");
        Managers.Game.ActionState = EActionState.Hand;
    }

    void OnClickSettingButton()
    {
        // 설정창 팝업 활성화
        Debug.Log("OnClickSettingButton");
    }
}
