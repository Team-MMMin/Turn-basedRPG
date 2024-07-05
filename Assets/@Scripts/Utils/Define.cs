public static class Define
{
    public enum EUIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag
    }

    public enum EScene
    {
        Unknown,
        MenuScene,
        GameScene,
    }

    public enum ECreatureState
    {
        None,
        Idle,
        Skill,
        Move,
        Dead,
    }

    public enum ECreatureType
    {
        None,
        PlayerUnitController,
        MonsterController,
    }

    public enum EObjectType
    {
        None,
        Creature,
        Projectile,
        Tile,
    }

    public enum ETileType
    {
        None,
        Free,
        Blocked,
        HpRecovery,
        MpRecovery,
        DefBoost,
        AtkBoost,
        Damage,
    }

    public enum EColliderSize
    {
        Small,
        Normal,
        Big
    }

    public enum EFindPathResult
    {
        Fail_LerpCell,
        Fail_NoPath,
        Fail_MoveTo,
        Success,
    }

    public enum ECellCollisionType
    {
        None, 
        SemiWall, //카메라만 갈수있는벽
        Wall,
    }

    public enum EClassType
    {
        Warrior,
        Mage,
    }

    public const int PLAYER_UNIT_WARRIOR_ID = 201000;
    
    public const int MONSTER_WARRIOR_ID = 202000;
}

public static class SortingLayers
{
    public const int CREATURE = 300;
}