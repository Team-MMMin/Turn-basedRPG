using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    #region enum
    enum GameObjects
    {
        SkillScrollView,
        SkillContent,
    }

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

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);
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
        ClearSkillList();
        Managers.Game.ActionState = EActionState.Skill;

        // 현재 유닛의 스킬을 가져온다
        CreatureController unit = Managers.Game.CurrentUnit;
        if (unit != null)
        {
            GetObject((int)GameObjects.SkillScrollView).SetActive(true);
            GameObject content = GetObject((int)GameObjects.SkillContent).gameObject;

            for (int i = 0; i < unit.Skills.SkillList.Count; i++)
            {
                GameObject go = Managers.Resource.Instantiate("UI_Skill_Item");
                go.name = unit.Skills.SkillList[i].SkillData.PrefabLabel;
                go.transform.parent = content.transform;

                TMP_Text txt = Util.FindChild<TMP_Text>(go);
                txt.text = unit.Skills.SkillList[i].Name;
            }
        }
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

    void ClearSkillList()
    {
        GameObject content = GetObject((int)GameObjects.SkillContent).gameObject;
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);
    }
}
