using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class DuplicateCardInHand : IEffect
{
    public Func<SimCard, bool>? CardFilter { get; set; }
    public bool UpgradeDuplicate { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var candidates = CardFilter != null
            ? engine.State.Hand.Cards.Where(CardFilter).ToList()
            : engine.State.Hand.Cards.ToList();

        foreach (var card in candidates)
        {
            var clone = card.Clone();
            if (UpgradeDuplicate) clone.IsUpgraded = true;
            engine.State.Hand.Add(clone);
        }
    }

    public IEffect Clone() => new DuplicateCardInHand { CardFilter = CardFilter, UpgradeDuplicate = UpgradeDuplicate };
}
