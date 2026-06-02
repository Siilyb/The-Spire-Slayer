using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class SetupStrikePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer != Owner) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        return Amount;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new SetupStrikePower { Amount = Amount };
}
