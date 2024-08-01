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

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        CreatureType = ECreatureType.PlayerUnit;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnActionStateChanged -= HandleOnActionStateChanged;
        Managers.Game.OnActionStateChanged += HandleOnActionStateChanged;
        
        OnDestPosChanged -= HandleOnDestPosChanged;
        OnDestPosChanged += HandleOnDestPosChanged;

        StartCoroutine(CoUpdate());
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
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
        if (CreatureState == ECreatureState.Skill)
        {
            
        }
    }

    void HandleOnActionStateChanged(EActionState actionState)
    {
        switch (actionState)
        {
            case EActionState.None:
                CreatureState = ECreatureState.Idle;
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
        GameObject SelectTiles = GameObject.Find("SelectTiles");
        if (SelectTiles == null)
            SelectTiles = new GameObject() { name = "SelectTiles" }; 
        
        foreach (Vector3Int delta in CastingSkill.SkillData.CastingRange)
        {
            Vector3 pos = GetTilePosition(delta);
            if (IsValidPosition(pos))
            {
                GameObject go = Managers.Resource.Instantiate("SelectTile", SelectTiles.transform);
                go.transform.position = pos;
            }
        }
    }

    bool IsValidPosition(Vector3 worldPos)
    {
        return Managers.Map.CanGo(worldPos);
    }

    Vector3 GetTilePosition(Vector3Int delta)
    {
        Vector3 worldPos = transform.position;
        worldPos.z = 0;

        Vector3Int cellPos = Managers.Map.WorldToCell(worldPos) + delta;
        Vector3 pos = Managers.Map.CellGrid.GetCellCenterWorld(cellPos) + new Vector3(0, -0.25f, 0);    // 중앙에서 약간 아래로 피벗 보정

        return pos;
    }
}
