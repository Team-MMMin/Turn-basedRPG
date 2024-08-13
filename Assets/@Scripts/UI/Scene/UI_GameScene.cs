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

    void HandleOnActionStateChanged(EPlayerActionState actionState)
    {
        switch (actionState)
        {
            case EPlayerActionState.None:
                HandleNoneAction();
                break;
            case EPlayerActionState.Hand:
                HandleHandAction();
                break;
            case EPlayerActionState.EndTurn:
                HandleEndTurnAction();
                break;
        }
    }

    void HandleNoneAction()
    {
        Managers.Game.CurrentUnit = null;
        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
    }

    void HandleHandAction()
    {
        Debug.Log("HandleHandAction");
        gameObject.SetActive(true);
        GetObject((int)GameObjects.ActionControllerObject).SetActive(true);
        GetObject((int)GameObjects.SkillScrollView).SetActive(false);
        GetButton((int)Buttons.MoveButton).interactable = !Managers.Game.CurrentUnit.IsMove;
        GetButton((int)Buttons.SkillButton).interactable = !Managers.Game.CurrentUnit.IsSkill;
        Refresh(Managers.Game.CurrentUnit);
    }

    void HandleEndTurnAction()
    {
        gameObject.SetActive(false);
    }

    void HandleOnPlayerActionSelected()
    {
        Refresh(Managers.Game.CurrentUnit);
    }

    void OnClickMoveButton()
    {
        Debug.Log("OnClickMoveButton");
        if (Managers.Game.CurrentUnit.IsMove)
            return;

        gameObject.SetActive(false);
        Managers.Game.CurrentUnit.SetMovementRange();
        Managers.Game.PlayerActionState = EPlayerActionState.Move;
    }

    void OnClickSkillButton()
    {
        Debug.Log("OnClickSkillButton");
        if (Managers.Game.CurrentUnit.IsSkill)
            return;

        ClearSkillList();
        GetObject((int)GameObjects.SkillScrollView).SetActive(true);
        GameObject content = GetObject((int)GameObjects.SkillContent).gameObject;
        
        // 스킬 스크롤 뷰
        List<SkillBase> skillList = Managers.Game.CurrentUnit.Skills.SkillList; // 현재 유닛의 스킬을 가져온다
        for (int i = 0; i < skillList.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI_Skill_Item", content.transform);
            go.name = skillList[i].SkillData.PrefabLabel;
            // 텍스트
            TMP_Text txt = Util.FindChild<TMP_Text>(go);
            txt.text = skillList[i].Name;
            // 이벤트 바인딩
            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickSkill_ItemButton(go.name));
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
                skill.SetCastingRange();
                Managers.Game.PlayerActionState = EPlayerActionState.Skill;
                break;
            }
        }
    }

    void OnClickEndTurnButton()
    {
        Debug.Log("OnClickEndTurnButton");

        // 모든 유닛이 턴 종료 상태인지 확인
        bool isValid = true;
        foreach (var unit in Managers.Object.PlayerUnits)
        {
            if (unit == this)
                continue;

            if (unit.CreatureState == ECreatureState.Dead)
                continue;

            if (unit.CreatureState != ECreatureState.EndTurn)
            {
                isValid = false;
                break;
            }
        }

        Managers.Game.CurrentUnit.CreatureState = ECreatureState.EndTurn;
        Managers.Game.PlayerActionState = EPlayerActionState.None;

        // 모든 유닛이 턴을 종료했다면 몬스터 턴으로 전환
        if (isValid)
        {
            gameObject.SetActive(false);
            Managers.Game.GameState = EGameState.MonsterTurn;
        }
    }

    void OnClickSettingButton()
    {
        // 설정창 팝업 활성화
        Debug.Log("OnClickSettingButton");
    }
}
