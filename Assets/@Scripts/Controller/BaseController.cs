using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        return true;
    }
}
