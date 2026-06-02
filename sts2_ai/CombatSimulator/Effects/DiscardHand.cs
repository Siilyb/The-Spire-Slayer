using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class DiscardHand : IEffect
{
    public int Amount { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (Amount <= 0 || Amount >= engine.State.Hand.Count)
            engine.State.Hand.MoveTo(engine.State.DiscardPile);
        else
        {
            var toDiscard = engine.State.Hand.Cards.Take(Amount).ToList();
            foreach (var card in toDiscard)
            {
                engine.State.Hand.Remove(card);
                engine.State.DiscardPile.Add(card);
            }
        }
    }

    public IEffect Clone() => new DiscardHand { Amount = Amount };
}
