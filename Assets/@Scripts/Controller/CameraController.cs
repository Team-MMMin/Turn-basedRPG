using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase
{
    BaseController _target;
    public BaseController Target 
    {
        get { return _target; } 
        set { _target = value; } 
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Camera.main.orthographicSize = 10.0f;

        return true;
    }

    void LateUpdate()
    {
        if (Target == null)
            return;

        Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10);
        transform.position = targetPosition;
    }
}
