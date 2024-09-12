using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAoEAttack : SkillBase
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

        Name = "Melee AoE Attack";
        Level = 1;  // test
    }

    public override bool DoSkill()
    {
        if (base.DoSkill() == false) 
            return false;

        Debug.Log("MeleeAoEAttack");

        foreach (var pos in CastingRange)
        {
            CreatureController target = Managers.Map.GetObject(pos) as CreatureController;
            if (target == null)
                continue;

            target.OnDamaged((SkillData.DamageMultiplier * Owner.Atk) - target.Def);
        }

        return true;
    }
}
