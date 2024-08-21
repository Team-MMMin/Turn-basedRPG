using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : InitBase
{
    public int ExtraCells { get; set; } = 0;

    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public BoxCollider2D Collider { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

    public Vector3 CenterPos { get { return transform.position + new Vector3(Collider.offset.x, Collider.offset.y); } }

    public int DataTemplateID { get; set; }

    bool _lookLeft = true;
    public bool LookLeft { get { return _lookLeft; } set { _lookLeft = value; } }

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        Collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();

        return true;
    }

    #region Map
    public bool LerpCellPosCompleted { get; protected set; }

    Vector3Int _cellPos;
    public Vector3Int CellPos
    {
        get { return _cellPos; }
        protected set
        {
            _cellPos = value;
            LerpCellPosCompleted = false;
        }
    }

    public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
    {
        CellPos = cellPos;
        LerpCellPosCompleted = false;

        if (forceMove)
        {
            Vector3 worldPos = Managers.Map.CellToWorld(CellPos);
            worldPos += new Vector3(0, 0.5f, 0);    // �ϴܿ��� �߾����� �ǹ� ����
            transform.position = worldPos;
            LerpCellPosCompleted = true;
        }
    }

    public void LerpToCellPos(float moveSpeed)  // �� ��ġ�� Lerp�ϰ� �̵�
    {
        if (LerpCellPosCompleted)
            return;

        Vector3 destPos = Managers.Map.CellToWorld(CellPos);
        destPos += new Vector3(0, 0.5f, 0); // �ϴܿ��� �߾����� �ǹ� ����
        Vector3 dir = destPos - transform.position;

        if (dir.x < 0)
            LookLeft = true;
        else
            LookLeft = false;

        if (dir.magnitude < 0.01f)
        {
            transform.position = destPos;
            LerpCellPosCompleted = true;
            return;
        }

        float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
        transform.position += dir.normalized * moveDist;
    }
    #endregion
}
