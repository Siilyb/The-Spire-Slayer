using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class PoisonPower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterSideTurnStart(CombatSide side, IReadOnlyList<SimCreature> participants)
    {
        if (!participants.Contains(Owner)) return;

        int triggerCount = Math.Min(Amount, 1);
        for (int i = 0; i < triggerCount; i++)
        {
            if (!Owner.IsAlive) break;
            Owner.LoseHp(Amount);
            if (Owner.IsAlive && Amount > 0)
                Amount--;
        }
    }

    public override SimPower Clone() => new PoisonPower { Amount = Amount };
}

public class ThornsPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void BeforeDamageReceived(SimCreature target, ref decimal amount, SimCreature? dealer, ValueProp props)
    {
        if (target == Owner && dealer != null && props.IsPoweredAttack())
        {
            dealer.LoseHp(Amount);
        }
    }

    public override SimPower Clone() => new ThornsPower { Amount = Amount };
}

public class AccuracyPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer != Owner) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        if (card == null || !card.Tags.Contains("Shiv")) return 0m;
        return Amount;
    }

    public override SimPower Clone() => new AccuracyPower { Amount = Amount };
}
