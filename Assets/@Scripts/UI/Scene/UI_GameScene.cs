using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    #region enum
    enum GameObjects
    {
        StatInfoObject,
        ActionControllerObject,
        SkillScrollView,
        SkillContent,
    }

    enum Texts
    {
        ClassValueText,
        HPVauleText,
        MPVauleText,
        ATKVauleText,
        DEFVauleText,
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
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);
        BindEvent(GetButton((int)Buttons.MoveButton).gameObject, OnClickMoveButton);
        BindEvent(GetButton((int)Buttons.SkillButton).gameObject, OnClickSkillButton);
        BindEvent(GetButton((int)Buttons.EndTurnButton).gameObject, OnClickEndTurnButton);
        BindEvent(GetButton((int)Buttons.SettingButton).gameObject, OnClickSettingButton);

        Managers.Game.OnActionStateChanged -= HandleOnActionStateChanged;
        Managers.Game.OnActionStateChanged += HandleOnActionStateChanged;

        return true;
    }

    public void SetInfo(CreatureController unit = null) // 데이터 받아올때
    {
        if (unit == null)
            unit = Managers.Game.CurrentUnit;

        Refresh(unit);
    }

    void Refresh(CreatureController unit = null)
    {
        if (unit == null)
            return;

        GetText((int)Texts.ClassValueText).text = unit.ClassData.PrefabLabel;   // 한글 폰트를 지원하지 않아 임시적으로 프리팹 라벨을 사용하기로 했다
        GetText((int)Texts.HPVauleText).text = unit.Hp.ToString();
        GetText((int)Texts.MPVauleText).text = unit.Mp.ToString();
        GetText((int)Texts.ATKVauleText).text = unit.Atk.ToString();
        GetText((int)Texts.DEFVauleText).text = unit.Hp.ToString();
    }

    void HandleOnActionStateChanged(EActionState actionState)
    {
        switch (actionState)
        {
            case EActionState.None:
                HandleNoneAction();
                break;
            case EActionState.Hand:
                HandleHandAction();
                break;
            case EActionState.Move:
                break;
            case EActionState.Skill:
                break;
            case EActionState.Spawn:
                break;
        }
    }

    void HandleNoneAction()
    {
        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
    }

    void HandleHandAction()
    {
        Refresh(Managers.Game.CurrentUnit);
        gameObject.SetActive(true);
        GetObject((int)GameObjects.ActionControllerObject).SetActive(true);
        GetObject((int)GameObjects.SkillScrollView).SetActive(false);
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
            // 스킬 스크롤 뷰
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

    void OnClickEndTurnButton()
    {
        Debug.Log("OnClickEndTurnButton");
        Managers.Game.ActionState = EActionState.None;
        Managers.Game.CurrentUnit.CreatureState = ECreatureState.EndTurn;
    }

    void OnClickSettingButton()
    {
        // 설정창 팝업 활성화
        Debug.Log("OnClickSettingButton");
    }
}
