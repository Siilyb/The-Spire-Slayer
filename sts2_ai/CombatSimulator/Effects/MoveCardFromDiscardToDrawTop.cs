using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class MoveCardFromDiscardToDrawTop : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var discard = engine.State.DiscardPile;
        if (discard.Count == 0) return;
        int idx = engine.State.Rng.Next(discard.Count);
        var card = discard.Cards[idx];
        discard.Remove(card);
        engine.State.DrawPile.InsertAt(0, card);
    }

    public IEffect Clone() => new MoveCardFromDiscardToDrawTop();
}
