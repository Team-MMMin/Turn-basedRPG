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
        CurrentUnitInfoObject,
        SkillScrollView,
        SkillContent,
    }

    enum Images
    {
        ProfileImage,
    }

    enum Texts
    {
        ClassValueText,
        HPVauleText,
        MPVauleText,
        ATKVauleText,
        DEFVauleText,
        CurrentUnitNameValueText,
        CurrentUnitClassValueText,
        CurrentUnitLvVauleText,
        CurrentUnitHPVauleText,
        CurrentUnitMPVauleText,
        CurrentUnitATKVauleText,
        CurrentUnitDEFVauleText,
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
        Debug.Log("UI_GameScene");
        if (base.Init() == false) 
            return false;

        #region Obejct Bind
        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.StatInfoObject).SetActive(false);
        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
        GetObject((int)GameObjects.CurrentUnitInfoObject).gameObject.SetActive(false);
        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);
        BindEvent(GetButton((int)Buttons.MoveButton).gameObject, OnClickMoveButton);
        BindEvent(GetButton((int)Buttons.SkillButton).gameObject, OnClickSkillButton);
        BindEvent(GetButton((int)Buttons.EndTurnButton).gameObject, OnClickEndTurnButton);
        BindEvent(GetButton((int)Buttons.SettingButton).gameObject, OnClickSettingButton);
        #endregion

        Managers.Game.OnGameStateChanged -= HandleOnGameStateChanged;
        Managers.Game.OnGameStateChanged += HandleOnGameStateChanged;
        Managers.Game.OnActionStateChanged -= HandleOnActionStateChanged;
        Managers.Game.OnActionStateChanged += HandleOnActionStateChanged;
        Managers.Game.Cursor.OnCreatureInfoUIShowed -= HandleOnCreatureInfoUIShowed;
        Managers.Game.Cursor.OnCreatureInfoUIShowed += HandleOnCreatureInfoUIShowed;

        return true;
    }

    public void SetInfo(CreatureController unit) // 데이터 받아올때
    {
        if (unit == Managers.Game.CurrentUnit)
            return;

        Refresh(unit);
    }

    void Refresh(CreatureController unit)
    {
        if (unit == null)
            return;

        GetText((int)Texts.ClassValueText).text = unit.ClassData.PrefabLabel;   // 한글 폰트를 지원하지 않아 임시적으로 프리팹 라벨을 사용하기로 했다
        GetText((int)Texts.HPVauleText).text = unit.Hp.ToString();
        GetText((int)Texts.MPVauleText).text = unit.Mp.ToString();
        GetText((int)Texts.ATKVauleText).text = unit.Atk.ToString();
        GetText((int)Texts.DEFVauleText).text = unit.Hp.ToString();
    }

    void CurrentUnitRefresh()
    {
        if (Managers.Game.CurrentUnit == null)
            return;

        GetText((int)Texts.CurrentUnitNameValueText).text = Managers.Game.CurrentUnit.CreatureData.Name;    // 한글 폰트를 지원하지 않아 깨진다
        GetText((int)Texts.CurrentUnitClassValueText).text = Managers.Game.CurrentUnit.ClassData.PrefabLabel;   // 한글 폰트를 지원하지 않아 임시적으로 프리팹 라벨을 사용하기로 했다
        GetText((int)Texts.CurrentUnitLvVauleText).text = Managers.Game.CurrentUnit.CreatureData.Level.ToString();
        GetText((int)Texts.CurrentUnitHPVauleText).text = Managers.Game.CurrentUnit.Hp.ToString();
        GetText((int)Texts.CurrentUnitMPVauleText).text = Managers.Game.CurrentUnit.Mp.ToString();
        GetText((int)Texts.CurrentUnitATKVauleText).text = Managers.Game.CurrentUnit.Atk.ToString();
        GetText((int)Texts.CurrentUnitDEFVauleText).text = Managers.Game.CurrentUnit.Hp.ToString();
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

    void HandleOnGameStateChanged(EGameState gameState)
    {
        switch (gameState)
        {
            case EGameState.PlayerTurn:
                HandlePlayerTurnAction();
                break;
            case EGameState.MonsterTurn:
                HandleMonsterTurnAction();
                break;
            case EGameState.Win:
                HandleWinAction();
                break;
            case EGameState.Lose:
                HandleLoseAction();
                break;
        }
    }

    void HandleOnCreatureInfoUIShowed(bool active)
    {
        GetObject((int)GameObjects.StatInfoObject).SetActive(active);
    }

    #region ActionState에 따른 UI변화
    void HandleNoneAction()
    {
        Managers.Game.CurrentUnit = null;
        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
        GetObject((int)GameObjects.CurrentUnitInfoObject).SetActive(false);
        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);
    }

    void HandleHandAction()
    {
        Debug.Log("HandleHandAction");
        GetObject((int)GameObjects.StatInfoObject).SetActive(false);
        GetObject((int)GameObjects.ActionControllerObject).SetActive(true);
        GetObject((int)GameObjects.CurrentUnitInfoObject).SetActive(true);
        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);
        GetButton((int)Buttons.MoveButton).interactable = !Managers.Game.CurrentUnit.IsMove;
        GetButton((int)Buttons.SkillButton).interactable = !Managers.Game.CurrentUnit.IsSkill;
        CurrentUnitRefresh();
    }

    void HandleEndTurnAction()
    {
        GetObject((int)GameObjects.StatInfoObject).SetActive(false);
        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
        GetObject((int)GameObjects.CurrentUnitInfoObject).gameObject.SetActive(false);
        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);
    }
    #endregion

    #region GameState에 따른 UI변화
    void HandlePlayerTurnAction()
    {
        // TODO
        // 현재 턴의 주체를 화면에 표시
    }

    void HandleMonsterTurnAction()
    {
        // TODO
        // 현재 턴의 주체를 화면에 표시
    }

    void HandleWinAction()
    {
        // TODO
        // 승패 결과를 화면에 표시
    }

    void HandleLoseAction()
    {
        // TODO
        // 승패 결과를 화면에 표시
    }
    #endregion

    void OnClickMoveButton()
    {
        Debug.Log("OnClickMoveButton");
        if (Managers.Game.CurrentUnit.IsMove)
            return;

        GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
        GetObject((int)GameObjects.CurrentUnitInfoObject).SetActive(false);
        GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);

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
                GetObject((int)GameObjects.ActionControllerObject).SetActive(false);
                GetObject((int)GameObjects.CurrentUnitInfoObject).SetActive(true);
                GetObject((int)GameObjects.SkillScrollView).gameObject.SetActive(false);

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

        // 모든 유닛이 턴을 종료했다면 몬스터 턴으로 전환
        if (isValid)
        {
            Managers.Game.PlayerActionState = EPlayerActionState.EndTurn;
            Managers.Game.GameState = EGameState.MonsterTurn;
        }
        else
        {
            Managers.Game.PlayerActionState = EPlayerActionState.None;
        }
    }

    void OnClickSettingButton()
    {
        // 설정창 팝업 활성화
        Debug.Log("OnClickSettingButton");
    }
}
