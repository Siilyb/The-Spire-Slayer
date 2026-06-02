using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class CreateCardInDiscard : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var card = engine.CurrentPlayingCard;
        if (card == null) return;
        var clone = card.Clone();
        engine.State.DiscardPile.Add(clone);
    }

    public IEffect Clone() => new CreateCardInDiscard();
}

public class UpgradeAllInHand : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        foreach (var card in engine.State.Hand.Cards.ToList())
            card.IsUpgraded = true;
    }

    public IEffect Clone() => new UpgradeAllInHand();
}

public class ExhaustRandomFromHand : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var hand = engine.State.Hand;
        if (hand.Count == 0) return;
        int idx = engine.State.Rng.Next(hand.Count);
        var card = hand.Cards[idx];
        hand.Remove(card);
        engine.State.ExhaustPile.Add(card);
    }

    public IEffect Clone() => new ExhaustRandomFromHand();
}

public class CopyEnemyVulnerable : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null || !target.IsAlive) return;
        int vulnAmount = target.GetPowerAmount<VulnerablePower>();
        if (vulnAmount <= 0) return;
        target.ApplyPower(new VulnerablePower { Amount = vulnAmount });
    }

    public IEffect Clone() => new CopyEnemyVulnerable();
}
