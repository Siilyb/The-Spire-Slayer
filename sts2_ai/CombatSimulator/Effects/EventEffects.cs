using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class ApotheosisEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        foreach (var card in engine.State.Hand.Cards)
            card.IsUpgraded = true;
        foreach (var card in engine.State.DrawPile.Cards)
            card.IsUpgraded = true;
        foreach (var card in engine.State.DiscardPile.Cards)
            card.IsUpgraded = true;
    }
    public IEffect Clone() => new ApotheosisEffect();
}

public class LoseMaxHpEffect : IEffect
{
    public decimal Amount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        source.MaxHp -= Amount;
        source.CurrentHp = Math.Min(source.CurrentHp, source.MaxHp);
    }
    public IEffect Clone() => new LoseMaxHpEffect { Amount = Amount };
}

public class GenerateRandomSkillInHand : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var card = new SimCard
        {
            Id = "RANDOM_SKILL", Name = "Skill", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect> { new GainBlock { BaseAmount = 5, Props = ValueProp.Move } }
        };
        engine.State.Hand.Add(card);
    }
    public IEffect Clone() => new GenerateRandomSkillInHand();
}

public class DualWieldEffect : IEffect
{
    public int CopyCount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var attacks = engine.State.Hand.Cards.Where(c => c.Type == CardType.Attack || c.Type == CardType.Power).ToList();
        if (attacks.Count == 0) return;
        var selected = attacks.First();
        for (int i = 0; i < CopyCount; i++)
            engine.State.Hand.Add(selected.Clone());
    }
    public IEffect Clone() => new DualWieldEffect { CopyCount = CopyCount };
}

public class EnlightenmentEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        foreach (var card in engine.State.Hand.Cards)
            card.Cost = Math.Min(card.Cost, 1);
    }
    public IEffect Clone() => new EnlightenmentEffect();
}

public class EnlightenmentUpgradedEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        foreach (var card in engine.State.Hand.Cards)
            card.Cost = Math.Min(card.Cost, 1);
    }
    public IEffect Clone() => new EnlightenmentUpgradedEffect();
}

public class EntrenchEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.CalculateBlock(source, source.Block, ValueProp.Unpowered | ValueProp.Move, null);
    }
    public IEffect Clone() => new EntrenchEffect();
}

public class MetamorphosisEffect : IEffect
{
    public int CardCount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        for (int i = 0; i < CardCount; i++)
        {
            var card = new SimCard
            {
                Id = "META_ATTACK", Name = "Attack", Cost = 0,
                Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
                Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } }
            };
            engine.State.DrawPile.Add(card);
        }
        engine.State.DrawPile.Shuffle(engine.State.Rng);
    }
    public IEffect Clone() => new MetamorphosisEffect { CardCount = CardCount };
}

public class NeowsFuryEffect : IEffect
{
    public int CardCount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var cards = engine.State.DiscardPile.Cards.Take(CardCount).ToList();
        foreach (var card in cards)
        {
            engine.State.DiscardPile.Remove(card);
            engine.State.Hand.Add(card);
        }
    }
    public IEffect Clone() => new NeowsFuryEffect { CardCount = CardCount };
}

public class StackEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.CalculateBlock(source, engine.State.DiscardPile.Count, ValueProp.Move, null);
    }
    public IEffect Clone() => new StackEffect();
}

public class StackUpgradedEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.CalculateBlock(source, 3 + engine.State.DiscardPile.Count, ValueProp.Move, null);
    }
    public IEffect Clone() => new StackUpgradedEffect();
}
