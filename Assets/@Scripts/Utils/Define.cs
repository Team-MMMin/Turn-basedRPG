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
        Player,
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

    public enum EClassType
    {
        Warrior,
        Mage,
    }
}
