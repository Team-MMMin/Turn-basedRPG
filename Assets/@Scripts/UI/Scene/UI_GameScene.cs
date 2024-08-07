using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static UnityEditor.Timeline.TimelinePlaybackControls;

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
        
        gameObject.SetActive(false);
        Managers.Game.ActionState = EActionState.Move;
    }

    void OnClickSkillButton()
    {
        Debug.Log("OnClickSkillButton");
        ClearSkillList();

        // 현재 유닛의 스킬을 가져온다
        CreatureController unit = Managers.Game.CurrentUnit;
        if (unit != null)
        {
            GetObject((int)GameObjects.SkillScrollView).SetActive(true);
            GameObject content = GetObject((int)GameObjects.SkillContent).gameObject;

            List<SkillBase> skillList = unit.Skills.SkillList;
            for (int i = 0; i < skillList.Count; i++)
            {
                GameObject go = Managers.Resource.Instantiate("UI_Skill_Item", content.transform);
                go.name = skillList[i].SkillData.PrefabLabel;

                TMP_Text txt = Util.FindChild<TMP_Text>(go);
                txt.text = skillList[i].Name;

                Button button = go.GetComponent<Button>();
                button.onClick.AddListener(() => OnClickSkill_ItemButton(go.name));
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

    void OnClickSkill_ItemButton(string name)
    {
        foreach (var skill in Managers.Game.CurrentUnit.Skills.SkillList)
        {
            if (skill.SkillData.PrefabLabel == name)
            {
                gameObject.SetActive(false);
                Managers.Game.CurrentUnit.CastingSkill = skill;
                Managers.Game.ActionState = EActionState.Skill;
                break;
            }
        }
    }
}
