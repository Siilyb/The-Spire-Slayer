using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class PlatingPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void BeforeSideTurnEndEarly(CombatSide side, IEnumerable<SimCreature> participants)
    {
        if (participants.Contains(Owner))
            Owner.GainBlock(Amount);
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new PlatingPower { Amount = Amount };
}

public class RegenPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterSideTurnEnd(CombatSide side, IEnumerable<SimCreature> participants)
    {
        if (participants.Contains(Owner) && Owner.IsAlive)
        {
            Owner.Heal(Amount);
            if (Amount > 0) Amount--;
        }
    }

    public override SimPower Clone() => new RegenPower { Amount = Amount };
}

public class BufferPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHpLostAfterOstyLate(SimCreature target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (target != Owner) return amount;
        if (Amount > 0 && amount > 0)
        {
            Amount--;
            return 0m;
        }
        return amount;
    }

    public override SimPower Clone() => new BufferPower { Amount = Amount };
}

public class IntangiblePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHpLostAfterOsty(SimCreature target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (target != Owner) return amount;
        if (amount < 1m) return amount;
        return 1m;
    }

    public override decimal ModifyDamageCap(SimCreature? target, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (target != Owner) return decimal.MaxValue;
        return 1m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new IntangiblePower { Amount = Amount };
}

public class BarricadePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldClearBlock(SimCreature creature)
    {
        if (creature == Owner) return false;
        return true;
    }

    public override SimPower Clone() => new BarricadePower { Amount = Amount };
}

public class BlurPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldClearBlock(SimCreature creature)
    {
        if (creature == Owner) return false;
        return true;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new BlurPower { Amount = Amount };
}
