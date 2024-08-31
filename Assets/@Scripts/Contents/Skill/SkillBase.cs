using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using static Define;

public abstract class SkillBase : InitBase
{
    public CreatureController Owner { get; private set; }

    public Data.SkillData SkillData { get; private set; }
    public List<Vector3> CastingRange { get; set; } = new List<Vector3>();
    public List<Vector3> SkillSizeRange  { get; set; } = new List<Vector3>();

    public string Name { get; protected set; }
    
    int level = 0;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public bool IsSkillUnlocked { get { return Level > 0; } }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(CreatureController owner, ESkillID skillID)
    {
        Owner = owner;
        SkillData = Managers.Data.SkillDataDic[(int)skillID];
    }

    public virtual bool DoSkill()
    {
        if (IsSkillUsable())
        {
            Owner.Mp -= SkillData.ManaCost;
            return true;
        }

        return false;
    }

    public bool IsSkillUsable()
    {
        if (IsSkillUnlocked == false)
            return false;
        
        if (Owner.Mp < SkillData.ManaCost)
            return false;

        return true;
    }

    public Data.SkillData UpdateSkillData()
    {
        int skillID = SkillData.DataID;
        if (skillID == 0)
            return SkillData;

        SkillData skillData = new SkillData();
        // TODO
        // 레벨에 따라 스킬 강화
        if (Managers.Data.SkillDataDic.TryGetValue(skillID, out skillData) == false)    // 강화한 스킬 불러오기
            return SkillData;

        SkillData = skillData;
        return SkillData;
    }

    public virtual void OnChangedSkillData() 
    {

    }

    public virtual void LevelUp()
    {

    }

    public void SetCastingRange()
    {
        ClearCastingRange();
        if (SkillData.CastingRange == null)
            return;

        foreach (Vector3Int delta in SkillData.CastingRange)
        {
            Vector3 pos = Managers.Map.GetTilePos(transform.position, delta);
            if (Managers.Map.CanGo(pos, true))
            {
                CastingRange.Add(pos);
                // 스킬을 사용한 크리처가 플레이어 유닛이라면 범위 시각화
                if (Owner.CreatureType == ECreatureType.PlayerUnit)
                {
                    GameObject go = Managers.Resource.Instantiate("RangeTile", pooling: true);
                    go.transform.position = pos;
                }
            }
        }
    }

    public void SetSizeRange()
    {
        ClearSizeRange();
        if (SkillData.SkillSize == null)
            return;

        foreach (Vector3Int delta in SkillData.SkillSize)
        {
            Vector3 pos = Managers.Map.GetTilePos(Owner.TargetPos, delta);
            if (Managers.Map.CanGo(pos, true))
            {
                SkillSizeRange.Add(pos);
                // 스킬을 사용한 크리처가 플레이어 유닛이라면 범위 시각화
                if (Owner.CreatureType == ECreatureType.PlayerUnit)
                {
                    GameObject go = Managers.Resource.Instantiate("SelectTile", pooling: true);
                    go.transform.position = pos;
                }
            }
        }
    }

    public void ClearCastingRange()
    {
        CastingRange.Clear();

        // 스킬을 사용한 크리처가 플레이어 유닛이라면 시각화한 범위를 없앤다
        if (Owner.CreatureType == ECreatureType.PlayerUnit)
        {
            GameObject go = GameObject.Find("RangeTile");
            if (go == null)
                return;

            foreach (Transform child in go.transform.parent)
            {
                if (child != null && child.gameObject.activeInHierarchy)
                    Managers.Resource.Destroy(child.gameObject);
            }
        }
    }

    public void ClearSizeRange()
    {
        SkillSizeRange.Clear();

        // 스킬을 사용한 크리처가 플레이어 유닛이라면 시각화한 범위를 없앤다
        if (Owner.CreatureType == ECreatureType.PlayerUnit)
        {
            GameObject go = GameObject.Find("SelectTile");
            if (go == null)
                return;

            foreach (Transform child in go.transform.parent)
            {
                if (child != null && child.gameObject.activeInHierarchy)
                    Managers.Resource.Destroy(child.gameObject);
            }
        }
    }
}
