using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class SkillBase : InitBase
{
    public CreatureController Owner { get; private set; }

    public Data.SkillData SkillData { get; private set; }

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
            return true;

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
        OnChangedSkillData();
        return SkillData;
    }

}
