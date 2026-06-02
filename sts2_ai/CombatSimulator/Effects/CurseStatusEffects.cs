using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class CurseTurnEndDamage : IEffect
{
    public decimal Amount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        source.LoseHp(Amount);
    }
    public IEffect Clone() => new CurseTurnEndDamage { Amount = Amount };
}

public class CurseTurnEndApplyPower : IEffect
{
    public Func<SimPower> PowerFactory { get; set; } = () => new PlaceholderPower();
    public int Amount { get; set; } = 1;
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var power = PowerFactory();
        power.Amount = Amount;
        source.ApplyPower(power);
    }
    public IEffect Clone() => new CurseTurnEndApplyPower { PowerFactory = PowerFactory, Amount = Amount };
}

public class CurseTurnEndHandSizeDamage : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        decimal dmg = engine.State.Hand.Count;
        source.LoseHp(dmg);
    }
    public IEffect Clone() => new CurseTurnEndHandSizeDamage();
}

public class VoidEnergyLoss : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (engine.State.Energy > 0)
            engine.State.Energy--;
    }
    public IEffect Clone() => new VoidEnergyLoss();
}
