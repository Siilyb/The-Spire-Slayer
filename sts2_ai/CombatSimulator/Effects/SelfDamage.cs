using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class SelfDamage : IEffect
{
    public decimal Amount { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        source.LoseHp(Amount);
    }

    public IEffect Clone() => new SelfDamage { Amount = Amount };
}
