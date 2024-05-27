using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : InitBase
{
    public EObjectType EObjectType { get; protected set; }

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        return true;
    }
}
