using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class UpgradeFromHand : IEffect
{
    public int Amount { get; set; }
    public Func<SimCard, bool>? Filter { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        // If MCTS pre-chose a card, use it
        if (engine.CurrentChoiceCard != null)
        {
            engine.CurrentChoiceCard.IsUpgraded = true;
            return;
        }

        var candidates = Filter != null
            ? engine.State.Hand.Cards.Where(Filter).ToList()
            : engine.State.Hand.Cards.ToList();

        int toUpgrade = Amount <= 0 ? candidates.Count : Math.Min(Amount, candidates.Count);
        for (int i = 0; i < toUpgrade; i++)
            candidates[i].IsUpgraded = true;
    }

    public IEffect Clone() => new UpgradeFromHand { Amount = Amount, Filter = Filter };
}
