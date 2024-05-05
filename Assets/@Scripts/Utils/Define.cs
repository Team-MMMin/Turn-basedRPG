public static class Define
{
    public enum UIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag
    }

    public enum Scene
    {
        Unknown,
        MenuScene,
        GameScene,
    }

    public enum CreatureState
    {
        Idle,
        Skill,
        Moving,
        OnDamaged,
        Dead,
    }

    public enum CreatureType
    {
        Player,
        Monster,
    }

    public enum ObjectType
    {
        Player,
        Monster,
    }

    public enum ClassType
    {
        Warrior,
        Mage,
    }
}
