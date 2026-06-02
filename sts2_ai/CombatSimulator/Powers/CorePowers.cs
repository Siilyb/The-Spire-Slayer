using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class StrengthPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => true;

    public override decimal ModifyDamageAdditive(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer != Owner) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        return Amount;
    }

    public override SimPower Clone() => new StrengthPower { Amount = Amount };
}

public class DexterityPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => true;

    public override decimal ModifyBlockAdditive(SimCreature target, decimal amount, ValueProp props, SimCard? card)
    {
        if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
        if (target != Owner) return 0m;
        return Amount;
    }

    public override SimPower Clone() => new DexterityPower { Amount = Amount };
}

public class VulnerablePower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (target != Owner) return 1m;
        if (!props.IsPoweredAttack()) return 1m;
        return 1.5m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new VulnerablePower { Amount = Amount };
}

public class WeakPower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer != Owner) return 1m;
        if (!props.IsPoweredAttack()) return 1m;
        return 0.75m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new WeakPower { Amount = Amount };
}

public class FrailPower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyBlockMultiplicative(SimCreature target, decimal amount, ValueProp props, SimCard? card)
    {
        if (target != Owner) return 1m;
        if (!props.IsPoweredCardOrMonsterMoveBlock()) return 1m;
        return 0.75m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new FrailPower { Amount = Amount };
}
