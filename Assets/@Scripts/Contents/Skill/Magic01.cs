using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic01 : SkillBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void SetInfo(CreatureController owner, Define.ESkillID skillID)
    {
        base.SetInfo(owner, skillID);

        Name = "Magic 01";
        Level = 1;  // test
    }

    public override bool DoSkill()
    {
        if (base.DoSkill() == false) 
            return false;

        Debug.Log("Magic01");

        foreach (var pos in SkillSize)
        {
            CreatureController target = Managers.Map.GetObject(pos) as CreatureController;
            if (target == null)
                continue;

            target.OnDamaged((SkillData.DamageMultiplier * Owner.Atk) - target.Def);
        }

        return true;
    }
}
