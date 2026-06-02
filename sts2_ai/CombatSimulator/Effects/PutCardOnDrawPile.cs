using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class PutCardOnDrawPile : IEffect
{
    public Func<SimCard, bool>? CardFilter { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var candidates = CardFilter != null
            ? engine.State.Hand.Cards.Where(CardFilter).ToList()
            : engine.State.Hand.Cards.ToList();

        if (candidates.Count == 0) return;
        var card = candidates.First();
        engine.State.Hand.Remove(card);
        engine.State.DrawPile.InsertAt(0, card);
    }

    public IEffect Clone() => new PutCardOnDrawPile { CardFilter = CardFilter };
}
