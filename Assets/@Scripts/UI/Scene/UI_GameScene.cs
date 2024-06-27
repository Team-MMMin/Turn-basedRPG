using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    #region enum
    enum Buttons
    {
        SettingButton,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        BindButton(typeof(Buttons));

        BindEvent(GetButton((int)Buttons.SettingButton).gameObject, OnClickSettingButton);

        return true;
    }

    void OnClickSettingButton()
    {
        // 설정창 팝업 활성화
        Debug.Log("OnClickSettingButton");
    }
}
