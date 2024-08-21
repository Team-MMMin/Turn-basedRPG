using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeNormalAttack : SkillBase
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

        Name = "Melee Normal Attack";
    }

    public override bool DoSkill()
    {
        if (base.DoSkill() == false) 
            return false;

        Debug.Log("MeleeNormalAttack");

        foreach (var pos in SkillSizeRange)
        {
            BaseController obj = Managers.Map.GetObject(pos);
            if (obj == null)
                continue;

            CreatureController target = obj.GetComponent<CreatureController>();
            if (target == null)
                continue;

            target.OnDamaged((SkillData.DamageMultiplier * Owner.Atk) - target.Def);
        }

        return true;
    }
}
