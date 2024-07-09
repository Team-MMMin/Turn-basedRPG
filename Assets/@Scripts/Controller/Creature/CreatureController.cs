using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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

    #region Stats
    public float Hp { get; set; }
    public float Mp { get; set; }
    public float Atk { get; set; }
    public float Def { get; set; }
    public int Mov { get; set; }
    #endregion

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

        CreatureSprite = gameObject.GetOrAddComponent<SpriteRenderer>();

        Collider.isTrigger = true;
        
        RigidBody.mass = 0;
        RigidBody.simulated = true;
        RigidBody.gravityScale = 0;
        RigidBody.freezeRotation = true;

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        CreatureData = Managers.Data.PlayerUnitDataDic[templateID];
        gameObject.name = $"{CreatureData.DataID}_{CreatureData.Name}";

        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingOrder = SortingLayers.CREATURE;

        // TODO
        // ½ºÅ³

        // Stat
        Hp = CreatureData.Hp;
        Mp = CreatureData.Mp;
        Atk = CreatureData.Atk;
        Def = CreatureData.Def;
        
        // Mov

        // State
        CreatureState = ECreatureState.Idle;
    }

}
