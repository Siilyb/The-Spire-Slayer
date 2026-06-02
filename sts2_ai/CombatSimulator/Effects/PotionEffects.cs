using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

/// Pick a random card from CardRegistry of a specific type, add to hand
public class PotionChooseCardEffect : IEffect
{
    public CardType? CardTypeFilter { get; set; }
    public Func<SimCard, bool>? CardPoolFilter { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var candidates = Cards.CardRegistry.All.Values
            .Select(f => f())
            .Where(c =>
            {
                if (c.Effects.Count == 0) return false;
                if (CardTypeFilter.HasValue && c.Type != CardTypeFilter.Value) return false;
                if (CardPoolFilter != null && !CardPoolFilter(c)) return false;
                return true;
            })
            .ToList();

        if (candidates.Count == 0) return;

        int idx = engine.State.Rng.Next(candidates.Count);
        var card = candidates[idx];
        card.Cost = 0; // free this turn
        engine.State.Hand.Add(card);
    }

    public IEffect Clone() => new PotionChooseCardEffect { CardTypeFilter = CardTypeFilter, CardPoolFilter = CardPoolFilter };
}

/// Draw 7 cards, randomize each card's cost to 0-3
public class SneckoOilEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.PlayerDrawCards(7);
        foreach (var card in engine.State.Hand.Cards)
        {
            if (!card.HasXCost)
                card.Cost = engine.State.Rng.Next(4);
        }
    }
    public IEffect Clone() => new SneckoOilEffect();
}

/// Double current block
public class FortifierEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        decimal block = source.Block;
        if (block > 0)
            engine.CalculateBlock(source, block, ValueProp.Unpowered | ValueProp.Move, null);
    }
    public IEffect Clone() => new FortifierEffect();
}

/// Discard all cards in hand, draw that many
public class GamblersBrewEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int count = engine.State.Hand.Count;
        foreach (var card in engine.State.Hand.Cards.ToList())
        {
            engine.State.Hand.Remove(card);
            engine.State.DiscardPile.Add(card);
        }
        engine.PlayerDrawCards(count);
    }
    public IEffect Clone() => new GamblersBrewEffect();
}

/// Return hand to draw pile, shuffle, draw 5
public class BottledPotentialEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var hand = engine.State.Hand.Cards.ToList();
        foreach (var card in hand)
        {
            engine.State.Hand.Remove(card);
            engine.State.DrawPile.Add(card);
        }
        engine.State.DrawPile.Shuffle(engine.State.Rng);
        engine.PlayerDrawCards(5);
    }
    public IEffect Clone() => new BottledPotentialEffect();
}

/// Auto-play the top 3 cards of draw pile
public class DistilledChaosEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        for (int i = 0; i < 3; i++)
        {
            if (engine.State.DrawPile.Count == 0) break;
            var card = engine.State.DrawPile.Draw();
            if (card == null) break;
            engine.State.Hand.Add(card);
            engine.PlayCard(card);
            if (engine.State.IsCombatOver) break;
        }
    }
    public IEffect Clone() => new DistilledChaosEffect();
}

/// Pick a random card in hand that costs energy, set to 0-cost
public class TouchOfInsanityEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var candidates = engine.State.Hand.Cards.Where(c => c.Cost > 0).ToList();
        if (candidates.Count == 0) return;
        int idx = engine.State.Rng.Next(candidates.Count);
        candidates[idx].Cost = 0;
    }
    public IEffect Clone() => new TouchOfInsanityEffect();
}

/// Generate a card of the given type (simple placeholder card for potion effects)
public class GenerateCardOfType : IEffect
{
    public CardType CardType { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var card = new SimCard
        {
            Id = $"POTION_GEN_{CardType}", Name = $"临时{CardType}", Cost = 0,
            Type = CardType, Rarity = CardRarity.Common,
            DefaultTargetType = CardType == CardType.Attack ? TargetType.SingleEnemy : TargetType.Self,
            Effects = new List<IEffect>
            {
                CardType == CardType.Attack
                    ? new DealDamage { BaseAmount = 6, Props = ValueProp.Move } as IEffect
                    : CardType == CardType.Skill
                        ? new GainBlock { BaseAmount = 5, Props = ValueProp.Move }
                        : new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
        engine.State.Hand.Add(card);
    }
    public IEffect Clone() => new GenerateCardOfType { CardType = CardType };
}
