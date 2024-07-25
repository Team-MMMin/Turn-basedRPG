public static class Define
{
    public enum EUIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag
    }

    public enum EMouseEvent
    {
        Click,
        Drag,
    }

    public enum EActionState
    {
        None,
        Hand,
        Spawn,
        Move,
        Skill,
    }

    public enum EScene
    {
        Unknown,
        MenuScene,
        GameScene,
    }

    public enum EGameState
    {
        None,
        PlayerTurn,
        MonsterTurn,
        Win,
        Lose,
    }

    public enum ECreatureState
    {
        None,
        Idle,
        Skill,
        Move,
        Dead,
        EndTurn,
    }

    public enum ECreatureType
    {
        None,
        PlayerUnit,
        Monster,
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
        None,
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

    public enum ESkillID
    {
        None,
        MeleeNormalAttack = 10001,
        MeleeStrongAttack = 10011,
        MeleeAoEAttack = 10021,
        MeleeStrongAoEAttack = 10031,
        NormalMagic = 20001,
        Magic01 = 20011,
        Magic02 = 20021,
        StrongAoEMagic = 20031,
    }

    public const int PLAYER_UNIT_WARRIOR_ID = 201000;
    
    public const int MONSTER_WARRIOR_ID = 202000;

    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_NONE = '1';
    public const char MAP_TOOL_SEMI_WALL = '2';
}

public static class SortingLayers
{
    public const int CREATURE = 300;
    public const int CURSOR = 300;
}