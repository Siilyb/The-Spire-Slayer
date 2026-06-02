using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class Heal : IEffect
{
    public decimal Amount { get; set; }
    public TargetType TargetType { get; set; } = TargetType.Self;

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var t = TargetType == TargetType.Self ? source : target;
        t?.Heal(Amount);
    }

    public IEffect Clone() => new Heal { Amount = Amount, TargetType = TargetType };
}
