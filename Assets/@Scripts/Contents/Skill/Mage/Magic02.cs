using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic02 : SkillBase
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

        Name = "Magic 02";
    }

    public override bool DoSkill()
    {
        if (base.DoSkill() == false) 
            return false;

        Debug.Log("Magic02");

        foreach (var pos in Size)
        {
            CreatureController target = Managers.Map.GetObject(pos) as CreatureController;
            if (target == null)
                continue;

            target.OnDamaged((SkillData.DamageMultiplier * Owner.Atk) - target.Def);
        }

        return true;
    }
}
