using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class ConditionalDraw : IEffect
{
    public int Amount { get; set; }
    public Func<SimEngine, bool>? Condition { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (Condition == null || Condition(engine))
            engine.PlayerDrawCards(Amount);
    }

    public IEffect Clone() => new ConditionalDraw { Amount = Amount, Condition = Condition };
}
