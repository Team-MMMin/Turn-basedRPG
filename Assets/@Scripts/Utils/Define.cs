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
        ScrollUp,
        ScrollDown,
    }

    public enum EPlayerActionState
    {
        None,
        Hand,
        Spawn,
        Move,
        Skill,
        EndTurn,
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

    public enum EMonsterBehaviorPattern
    {
        None,
        Aggressive,
        Defensive,
        Strategic,
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
        SemiWall,   // 카메라만 갈 수 있는 벽
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

    public const int WARRIOR_ID = 101001;
    public const int MAGE_ID = 101002; 
    
    public const int PLAYER_UNIT_WARRIOR_ID = 201001;
    public const int MONSTER_WARRIOR_ID = 202001;

    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_NONE = '1';
    public const char MAP_TOOL_SEMI_WALL = '2';
}

public static class SortingLayers
{
    public const int CREATURE = 300;
    public const int CURSOR = 300;
    public const int DAMAGE_FONT = 400;
    public const int HP_BAR = 400;
}