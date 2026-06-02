using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class ExhaustFromHand : IEffect
{
    public int Amount { get; set; }
    public Func<SimCard, bool>? Filter { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var candidates = Filter != null
            ? engine.State.Hand.Cards.Where(Filter).ToList()
            : engine.State.Hand.Cards.ToList();

        int toExhaust = Amount <= 0 ? candidates.Count : Math.Min(Amount, candidates.Count);
        for (int i = 0; i < toExhaust && i < candidates.Count; i++)
        {
            var card = candidates[i];
            engine.State.Hand.Remove(card);
            engine.State.ExhaustPile.Add(card);
            foreach (var power in engine.State.Players.SelectMany(p => p.Powers))
                power.AfterCardExhausted(engine.State.Players[0], card);
        }
    }

    public IEffect Clone() => new ExhaustFromHand { Amount = Amount, Filter = Filter };
}
