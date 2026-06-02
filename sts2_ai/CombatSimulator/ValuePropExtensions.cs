namespace Sts2Ai.CombatSimulator;

public static class ValuePropExtensions
{
    public static bool IsPoweredAttack(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
    }

    public static bool IsPoweredCardOrMonsterMoveBlock(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
    }
}
