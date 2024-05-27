using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : CreatureController
{
    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        CreatureType = ECreatureType.Player;

        return true;
    }
}
