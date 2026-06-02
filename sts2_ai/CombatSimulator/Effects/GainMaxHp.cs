using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class GainMaxHp : IEffect
{
    public decimal Amount { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        source.MaxHp += Amount;
        source.Heal(Amount);
    }

    public IEffect Clone() => new GainMaxHp { Amount = Amount };
}
