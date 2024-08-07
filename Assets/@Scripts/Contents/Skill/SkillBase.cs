using Data;
using System.Collections;
using System.Collections.Generic;
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
        if (Owner.Mp >= SkillData.ManaCost)
        {
            Owner.Mp -= SkillData.ManaCost;
            return true;
        }

        return false;
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

    public void ShowCastingRange()
    {
        foreach (Vector3Int delta in SkillData.CastingRange)
        {
            Vector3 pos = Managers.Map.GetTilePosition(transform.position, delta, new Vector3(0, -0.25f, 0));
            if (Managers.Map.CanGo(pos, true))
            {
                CastingRange.Add(pos);
                GameObject go = Managers.Resource.Instantiate("RangeTile", pooling: true);
                go.transform.position = pos;
            }
        }
    }

    public void ShowSizeRange()
    {
        ClearSizeRange();
        foreach (Vector3Int delta in SkillData.SkillSize)
        {
            Vector3 pos = Managers.Map.GetTilePosition(Managers.Game.CursorPos, delta, new Vector3(0, -0.25f, 0));
            if (Managers.Map.CanGo(pos, true))
            {
                SkillSizeRange.Add(pos);
                GameObject go = Managers.Resource.Instantiate("SelectTile", pooling: true);
                go.transform.position = pos;
            }
        }
    }

    public void ClearCastingRange()
    {
        GameObject tile = GameObject.Find("RangeTile");
        if (tile == null)
            return;

        foreach (Transform child in tile.transform.parent)
            Managers.Resource.Destroy(child.gameObject);

        CastingRange.Clear();
    }

    public void ClearSizeRange()
    {
        GameObject tile = GameObject.Find("SelectTile");
        if (tile == null)
            return;

        foreach (Transform child in tile.transform.parent)
            Managers.Resource.Destroy(child.gameObject);

        SkillSizeRange.Clear();
    }
}
