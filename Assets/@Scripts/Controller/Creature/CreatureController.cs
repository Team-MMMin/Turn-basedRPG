using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class CreatureController : BaseController
{
    public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;
    protected ECreatureState _creatureState = ECreatureState.None;
    public virtual ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value) 
            {
                _creatureState = value;
            }
        }
    }

    [SerializeField]
    protected SpriteRenderer CreatureSprite;
    protected string SpriteName;

    public CreatureData CreatureData;

    public virtual int DataId { get; set; }
    public virtual string Name { get; set; }
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
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Creature;
        CreatureState = ECreatureState.Idle;

        CreatureSprite = GetComponent<SpriteRenderer>();
        if (CreatureSprite == null)
            CreatureSprite = Util.FindChild<SpriteRenderer>(gameObject);

        return true;
    }
}
