using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;
using System;  
using System.Collections.Generic;
using System.Linq;

namespace Sts2Ai.CombatSimulator.Effects;

// For Dominate: gain Strength = target's Vulnerable stacks
public class DominateStrength : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int vuln = target?.GetPowerAmount<VulnerablePower>() ?? 0;
        if (vuln > 0)
            source.ApplyPower(new StrengthPower { Amount = vuln });
    }
    public IEffect Clone() => new DominateStrength();
}

// For ExpectAFight: gain energy = attack cards in hand
public class GainEnergyPerAttackInHand : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int attackCount = engine.State.Hand.Cards.Count(c => c.Type == CardType.Attack);
        engine.State.Energy += attackCount;
    }
    public IEffect Clone() => new GainEnergyPerAttackInHand();
}

// For ForgottenRitual: energy only if condition met
public class ConditionalGainEnergy : IEffect
{
    public int Amount { get; set; }
    public Func<SimEngine, bool>? Condition { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (Condition == null || Condition(engine))
            engine.State.Energy += Amount;
    }
    public IEffect Clone() => new ConditionalGainEnergy { Amount = Amount, Condition = Condition };
}

// For InfernalBlade: generate a random attack card in hand
public class GenerateRandomAttackInHand : IEffect
{
    public bool FreeToPlay { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var card = new SimCard
        {
            Id = "GENERATED_ATTACK", Name = "Strike", Cost = FreeToPlay ? 0 : 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } }
        };
        engine.State.Hand.Add(card);
    }
    public IEffect Clone() => new GenerateRandomAttackInHand { FreeToPlay = FreeToPlay };
}

// For Pillage: draw until condition met
public class DrawUntilCondition : IEffect
{
    public Func<SimCard, bool>? Condition { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        while (engine.State.Hand.Count < 10)
        {
            if (engine.State.DrawPile.Count == 0)
                engine.ReshuffleDiscardToDraw();
            if (engine.State.DrawPile.Count == 0) break;
            var card = engine.State.DrawPile.Draw();
            if (card == null) break;
            engine.State.Hand.Add(card);
            if (Condition != null && Condition(card)) break;
        }
    }
    public IEffect Clone() => new DrawUntilCondition { Condition = Condition };
}

// For Rampage: permanent damage increase
public class RampageDamage : IEffect
{
    public decimal BaseDamage { get; set; }
    public decimal IncreasePerPlay { get; set; }
    private static readonly Dictionary<string, decimal> _rampageTracker = new();

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var enemy = engine.State.Enemies.FirstOrDefault(e => e.IsAlive);
        if (enemy == null) return;

        string key = "rampage_" + (engine.CurrentPlayingCard?.Id ?? "unknown");
        if (!_rampageTracker.ContainsKey(key))
            _rampageTracker[key] = BaseDamage;

        decimal currentDmg = _rampageTracker[key];

        decimal finalDmg = engine.CalculateDamage(source, enemy, currentDmg, 1, ValueProp.Move, null);
        enemy.LoseHp(finalDmg);

        _rampageTracker[key] = currentDmg + IncreasePerPlay;
    }
    public IEffect Clone() => new RampageDamage { BaseDamage = BaseDamage, IncreasePerPlay = IncreasePerPlay };
}

// For SecondWind: exhaust non-attacks, gain block per card
public class SecondWindEffect : IEffect
{
    public decimal BlockPerCard { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var nonAttacks = engine.State.Hand.Cards.Where(c => c.Type != CardType.Attack).ToList();
        foreach (var card in nonAttacks)
        {
            engine.State.Hand.Remove(card);
            engine.State.ExhaustPile.Add(card);
            decimal block = engine.CalculateBlock(source, BlockPerCard, ValueProp.Move, null);
            source.GainBlock(block);
        }
    }
    public IEffect Clone() => new SecondWindEffect { BlockPerCard = BlockPerCard };
}

// For Whirlwind: X-cost multi-hit AOE
public class WhirlwindDamage : IEffect
{
    public decimal DamagePerHit { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int x = engine.State.Energy;
        engine.State.Energy = 0;
        var enemies = engine.State.Enemies.Where(e => e.IsAlive).ToList();
        for (int i = 0; i < x; i++)
        {
            foreach (var enemy in enemies)
            {
                decimal dmg = engine.CalculateDamage(source, enemy, DamagePerHit, 1, ValueProp.Move, null);
                enemy.LoseHp(dmg);
            }
        }
    }
    public IEffect Clone() => new WhirlwindDamage { DamagePerHit = DamagePerHit };
}

// FiendFire: exhaust all hand, damage per card
public class FiendFireEffect : IEffect
{
    public decimal DamagePerCard { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var enemy = engine.State.Enemies.FirstOrDefault(e => e.IsAlive);
        if (enemy == null) return;

        int cardCount = engine.State.Hand.Count;
        var handCards = engine.State.Hand.Cards.ToList();
        foreach (var card in handCards)
        {
            engine.State.Hand.Remove(card);
            engine.State.ExhaustPile.Add(card);
        }

        decimal totalDmg = engine.CalculateDamage(source, enemy, DamagePerCard * cardCount, 1, ValueProp.Move, null);
        enemy.LoseHp(totalDmg);
    }
    public IEffect Clone() => new FiendFireEffect { DamagePerCard = DamagePerCard };
}

// Cascade: X-cost auto-play from draw pile
public class CascadeEffect : IEffect
{
    public int BonusCount { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int x = engine.State.Energy;
        engine.State.Energy = 0;
        int count = x + BonusCount;
        for (int i = 0; i < count; i++)
        {
            if (engine.State.DrawPile.Count == 0) break;
            var card = engine.State.DrawPile.Draw();
            if (card == null) break;
            engine.State.Hand.Add(card);
            engine.PlayCard(card);
            if (engine.State.IsCombatOver) break;
        }
    }
    public IEffect Clone() => new CascadeEffect { BonusCount = BonusCount };
}

// ConditionalDealDamage: only deals if condition met
public class ConditionalDealDamage : IEffect
{
    public decimal BaseAmount { get; set; }
    public ValueProp Props { get; set; } = ValueProp.Move;
    public Func<SimEngine, bool>? Condition { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (Condition != null && !Condition(engine)) return;
        var enemies = engine.State.Enemies.Where(e => e.IsAlive).ToList();
        foreach (var enemy in enemies)
        {
            decimal dmg = engine.CalculateDamage(source, enemy, BaseAmount, 1, Props, null);
            enemy.LoseHp(dmg);
        }
    }
    public IEffect Clone() => new ConditionalDealDamage { BaseAmount = BaseAmount, Props = Props, Condition = Condition };
}

// ColossusPower: temporary block each turn
public class ColossusPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void BeforeSideTurnEndEarly(CombatSide side, IEnumerable<SimCreature> participants)
    {
        if (participants.Contains(Owner))
            Owner.GainBlock(Amount);
    }
    public override SimPower Clone() => new ColossusPower { Amount = Amount };
}
