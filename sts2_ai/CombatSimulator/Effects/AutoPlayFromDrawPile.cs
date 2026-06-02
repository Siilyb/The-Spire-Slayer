using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class AutoPlayFromDrawPile : IEffect
{
    public bool ExhaustAfterPlay { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (engine.State.DrawPile.Count == 0) return;
        var card = engine.State.DrawPile.Draw();
        if (card == null) return;
        if (ExhaustAfterPlay)
            card.Keywords.Add("Exhaust");
        engine.State.Hand.Add(card);
        engine.PlayCard(card);
    }

    public IEffect Clone() => new AutoPlayFromDrawPile { ExhaustAfterPlay = ExhaustAfterPlay };
}
