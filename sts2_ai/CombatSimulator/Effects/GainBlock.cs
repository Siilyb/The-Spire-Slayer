using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class GainBlock : IEffect
{
    public decimal BaseAmount { get; set; }
    public TargetType TargetType { get; set; } = TargetType.Self;
    public ValueProp Props { get; set; } = ValueProp.Move;
    public Func<SimEngine, SimCreature, decimal>? CalculatedBlock { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var targets = TargetType == TargetType.Self
            ? new List<SimCreature> { source }
            : engine.State.Players.Where(p => p.IsAlive).ToList();

        foreach (var t in targets)
        {
            decimal amt = CalculatedBlock != null ? CalculatedBlock(engine, t) : BaseAmount;
            engine.CalculateBlock(t, amt, Props, null);
        }
    }

    public IEffect Clone() => new GainBlock { BaseAmount = BaseAmount, TargetType = TargetType, Props = Props, CalculatedBlock = CalculatedBlock };
}
