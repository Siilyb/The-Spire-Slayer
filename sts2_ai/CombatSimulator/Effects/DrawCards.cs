using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class DrawCards : IEffect
{
    public int Amount { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.PlayerDrawCards(Amount);
    }

    public IEffect Clone() => new DrawCards { Amount = Amount };
}
