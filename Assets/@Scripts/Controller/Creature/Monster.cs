using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CreatureController
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
