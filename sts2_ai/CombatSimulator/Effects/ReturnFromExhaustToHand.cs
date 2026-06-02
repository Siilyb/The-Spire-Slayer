using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class ReturnFromExhaustToHand : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var exhaust = engine.State.ExhaustPile;
        if (exhaust.Count == 0) return;
        var card = exhaust.Cards.Last();
        exhaust.Remove(card);
        engine.State.Hand.Add(card);
    }

    public IEffect Clone() => new ReturnFromExhaustToHand();
}
