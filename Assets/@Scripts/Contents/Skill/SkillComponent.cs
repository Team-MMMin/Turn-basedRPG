using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillComponent : InitBase
{
    public List<SkillBase> SkillList = new List<SkillBase>();

    CreatureController _owner;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public void SetInfo(CreatureController owner)
    {
        _owner = owner;

        InitSkill(_owner.ClassData);
    }

    public void InitSkill(Data.ClassData ClassData)
    {
        foreach (ESkillID skill in ClassData.SkillIDList)
        {
            AddSkill(skill);
        }
    }

    public void AddSkill(ESkillID skillID)
    {
        if (skillID == ESkillID.None)
            return;

        if (Managers.Data.SkillDataDic.TryGetValue((int)skillID, out var data) == false)
            return;

        SkillBase skill = gameObject.AddComponent(Type.GetType(data.PrefabLabel)) as SkillBase;
        if (skill == null) 
            return;

        skill.SetInfo(_owner, skillID);
        SkillList.Add(skill);
    }
}
