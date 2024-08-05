using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerUnitController : CreatureController
{
    Vector3 _destPos;
    public Vector3 DestPos
    {
        get { return _destPos; }
        set 
        { 
            _destPos = value;
            OnDestPosChanged?.Invoke();
        }
    }

    public event Action OnDestPosChanged;

    public Transform SelectTileRoot 
    { 
        get
        {
            GameObject tile = GameObject.Find("SelectTile");
            if (tile == null)
                return null;

            return tile.transform.parent;
        }
    }

    public Transform SkillSizeTileRoot
    {
        get
        {
            GameObject tile = GameObject.Find("SkillSizeTile");
            if (tile == null)
                return null;

            return tile.transform.parent;
        }
    }

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        CreatureType = ECreatureType.PlayerUnit;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnActionStateChanged -= HandleOnActionStateChanged;
        Managers.Game.OnActionStateChanged += HandleOnActionStateChanged;
        Managers.Game.OnCursorPosChanged -= ShowSkillSize;
        Managers.Game.OnCursorPosChanged += ShowSkillSize;

        OnDestPosChanged -= HandleOnDestPosChanged;
        OnDestPosChanged += HandleOnDestPosChanged;

        StartCoroutine(CoUpdate());
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
    }

    protected override void UpdateIdle()
    {

    }

    protected override void UpdateMove()
    {
        if (CreatureState == ECreatureState.Move)
        {
            Debug.Log("UpdateMove");
            // TODO
            // 플레이어 유닛의 무브 포인트만큼 이동 가능한 구역 표시
        }
    }

    protected override void UpdateSkill()
    {
        if (CreatureState == ECreatureState.Skill && CastingSkill != null)
        {
            
        }
    }

    void HandleOnActionStateChanged(EActionState actionState)
    {
        switch (actionState)
        {
            case EActionState.None:
                CreatureState = ECreatureState.Idle;
                ClearSkillCastingRange();
                ClearSkillSize();
                break;
            case EActionState.Move:
                CreatureState = ECreatureState.Move;
                break;
            case EActionState.Skill:
                CreatureState = ECreatureState.Skill;
                ShowSkillCastingRange();
                break;
        }
    }

    void HandleOnDestPosChanged()
    {
        FindPathAndMoveToCellPos(_destPos, Mov);
    }

    void ShowSkillCastingRange()
    {
        foreach (Vector3Int delta in CastingSkill.SkillData.CastingRange)
        {
            Vector3 pos = Managers.Map.GetTilePosition(transform.position, delta, new Vector3(0, -0.25f, 0));
            if (Managers.Map.CanGo(pos))
            {
                CastingSkill.CastingRange.Add(pos);
                GameObject go = Managers.Resource.Instantiate("SelectTile", pooling: true);
                go.transform.position = pos;
            }
        }
    }

    void ShowSkillSize(Vector3 cursorPos)
    {
        ClearSkillSize();
        foreach (Vector3Int delta in CastingSkill.SkillData.SkillSize)
        {
            Vector3 pos = Managers.Map.GetTilePosition(cursorPos, delta, new Vector3(0, -0.25f, 0));
            if (Managers.Map.CanGo(pos))
            {
                CastingSkill.SkillSizeRange.Add(delta);
                GameObject go = Managers.Resource.Instantiate("SkillSizeTile", pooling: true);
                go.transform.position = pos;
            }
        }
    }

    void ClearSkillCastingRange()
    {
        if (SelectTileRoot == null)
            return;

        foreach (Transform child in SelectTileRoot)
            Managers.Resource.Destroy(child.gameObject);
    }

    public void ClearSkillSize()
    {
        if (SkillSizeTileRoot == null)
            return;

        foreach (Transform child in SkillSizeTileRoot)
            Managers.Resource.Destroy(child.gameObject);
    }

    bool IsValidPosition(Vector3 worldPos)
    {
        return Managers.Map.CanGo(worldPos);
    }
}
