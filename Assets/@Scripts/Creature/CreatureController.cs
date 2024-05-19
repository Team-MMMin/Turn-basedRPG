using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureController : BaseController
{
    [SerializeField]
    protected SpriteRenderer CreatureSprite;
    protected string SpriteName;

    public CreatureData CreatureData;

    public virtual int DataId { get; set; }
    public virtual float Hp { get; set; }
    public virtual float Mp { get; set; }
    public virtual float Atk { get; set; }
    public virtual float Def { get; set; }
    public virtual int Mov { get; set; }

    void Awake()
    {
        Init();    
    }

    public override bool Init()
    {
        base.Init();

        CreatureSprite = GetComponent<SpriteRenderer>();
        if (CreatureSprite == null)
            CreatureSprite = Util.FindChild<SpriteRenderer>(gameObject);

        return true;
    }
}
