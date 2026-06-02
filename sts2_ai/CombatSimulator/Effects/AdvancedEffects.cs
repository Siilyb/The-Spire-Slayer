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

// For Rampage: permanent damage increase per instance
public class RampageDamage : IEffect
{
    public decimal BaseDamage { get; set; }
    public decimal IncreasePerPlay { get; set; }
    private decimal _extraDamage;

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var enemy = engine.State.Enemies.FirstOrDefault(e => e.IsAlive);
        if (enemy == null) return;

        decimal currentDmg = BaseDamage + _extraDamage;
        decimal finalDmg = engine.CalculateDamage(source, enemy, currentDmg, 1, ValueProp.Move, null);
        enemy.LoseHp(finalDmg);

        _extraDamage += IncreasePerPlay;
    }

    public IEffect Clone() => new RampageDamage { BaseDamage = BaseDamage, IncreasePerPlay = IncreasePerPlay, _extraDamage = _extraDamage };
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

// Feed: deal damage, if target dies gain MaxHP
public class FeedEffect : IEffect
{
    public decimal DamageAmount { get; set; }
    public decimal MaxHpGain { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null || !target.IsAlive) return;
        decimal hpBefore = target.CurrentHp;
        decimal dmg = engine.CalculateDamage(source, target, DamageAmount, 1, ValueProp.Move, null);
        target.LoseHp(dmg);
        if (hpBefore > 0 && !target.IsAlive)
        {
            source.MaxHp += MaxHpGain;
            source.Heal(MaxHpGain);
        }
    }
    public IEffect Clone() => new FeedEffect { DamageAmount = DamageAmount, MaxHpGain = MaxHpGain };
}

// EvilEye: block doubles if any card was exhausted this turn
public class EvilEyeEffect : IEffect
{
    public decimal BaseBlock { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int times = engine.CardsExhaustedThisTurn > 0 ? 2 : 1;
        for (int i = 0; i < times; i++)
            engine.CalculateBlock(source, BaseBlock, ValueProp.Move, null);
    }
    public IEffect Clone() => new EvilEyeEffect { BaseBlock = BaseBlock };
}

// Thrash: 2-hit attack, then exhaust random attack from hand, add its damage permanently
public class ThrashEffect : IEffect
{
    public decimal BaseDamage { get; set; }
    private decimal _accumulatedDamage;

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null || !target.IsAlive) return;
        decimal dmg = engine.CalculateDamage(source, target, BaseDamage + _accumulatedDamage, 1, ValueProp.Move, null);
        target.LoseHp(dmg);
        if (target.IsAlive)
        {
            dmg = engine.CalculateDamage(source, target, BaseDamage + _accumulatedDamage, 1, ValueProp.Move, null);
            target.LoseHp(dmg);
        }

        // Exhaust a random attack from hand and add its damage
        var attacks = engine.State.Hand.Cards.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count > 0)
        {
            int idx = engine.State.Rng.Next(attacks.Count);
            var attackCard = attacks[idx];
            engine.State.Hand.Remove(attackCard);
            engine.State.ExhaustPile.Add(attackCard);

            // Find the card's damage
            var dmgEffect = attackCard.CurrentEffects.OfType<DealDamage>().FirstOrDefault();
            if (dmgEffect != null)
            {
                decimal addedDmg = dmgEffect.BaseAmount;
                _accumulatedDamage += addedDmg;
            }
        }
    }
    public IEffect Clone() => new ThrashEffect { BaseDamage = BaseDamage, _accumulatedDamage = _accumulatedDamage };
}

// PrimalForce: transform all attack cards in hand to GiantRocks
public class PrimalForceEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var attacks = engine.State.Hand.Cards.Where(c => c.Type == CardType.Attack).ToList();
        foreach (var attack in attacks)
        {
            engine.State.Hand.Remove(attack);
            var giantRock = new SimCard
            {
                Id = "GIANT_ROCK", Name = "Giant Rock", Cost = 0,
                Type = CardType.Attack, Rarity = CardRarity.Token, DefaultTargetType = TargetType.SingleEnemy,
                Keywords = new HashSet<string> { "Exhaust" },
                Effects = new List<IEffect> { new DealDamage { BaseAmount = 5, Props = ValueProp.Move } }
            };
            engine.State.Hand.Add(giantRock);
        }
    }
    public IEffect Clone() => new PrimalForceEffect();
}

// Stoke: exhaust all hand, create random cards
public class StokeEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int count = engine.State.Hand.Count;
        var cards = engine.State.Hand.Cards.ToList();
        foreach (var card in cards)
        {
            engine.State.Hand.Remove(card);
            engine.State.ExhaustPile.Add(card);
        }
        bool upgraded = engine.CurrentPlayingCard?.IsUpgraded ?? false;
        for (int i = 0; i < count; i++)
        {
            var newCard = new SimCard
            {
                Id = "STOKE_GENERATED", Name = "Generated", Cost = 1,
                Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
                Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } },
                IsUpgraded = upgraded
            };
            if (upgraded)
            {
                newCard.Effects = new List<IEffect> { new DealDamage { BaseAmount = 9, Props = ValueProp.Move } };
            }
            engine.State.Hand.Add(newCard);
        }
    }
    public IEffect Clone() => new StokeEffect();
}

// HowlFromBeyond: when this card is exhausted, auto-play it from the exhaust pile
public class HowlFromBeyondEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target) { }

    public IEffect Clone() => new HowlFromBeyondEffect();
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

// Omnislice: deal damage to main target + cleave equal to damage dealt to others
public class OmnisliceEffect : IEffect
{
    public decimal BaseDamage { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null || !target.IsAlive) return;
        decimal dmg = engine.CalculateDamage(source, target, BaseDamage, 1, ValueProp.Move, null);
        target.LoseHp(dmg);
        foreach (var enemy in engine.State.Enemies.Where(e => e.IsAlive && e != target))
        {
            enemy.LoseHp(dmg);
        }
    }
    public IEffect Clone() => new OmnisliceEffect { BaseDamage = BaseDamage };
}

// Fisticuffs: deal damage, gain block equal to total damage dealt
public class FisticuffsEffect : IEffect
{
    public decimal BaseDamage { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null || !target.IsAlive) return;
        decimal hpBefore = target.CurrentHp;
        decimal dmg = engine.CalculateDamage(source, target, BaseDamage, 1, ValueProp.Move, null);
        target.LoseHp(dmg);
        decimal hpLost = hpBefore - target.CurrentHp;
        engine.CalculateBlock(source, hpLost, ValueProp.Move, null);
    }
    public IEffect Clone() => new FisticuffsEffect { BaseDamage = BaseDamage };
}

// SeekerStrike: pick a card from draw pile to hand
public class SeekerStrikeEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (engine.State.DrawPile.Count == 0) return;
        int idx = engine.State.Rng.Next(engine.State.DrawPile.Count);
        var card = engine.State.DrawPile.Cards[idx];
        engine.State.DrawPile.Remove(card);
        engine.State.Hand.Add(card);
    }
    public IEffect Clone() => new SeekerStrikeEffect();
}

// Prolong: convert current block to BlockNextTurnPower
public class ProlongEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        decimal block = source.Block;
        source.Block = 0;
        source.GainBlock(block);
    }
    public IEffect Clone() => new ProlongEffect();
}

// Scrawl: draw to fill hand (max 10)
public class ScrawlEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int toDraw = 10 - engine.State.Hand.Count;
        if (toDraw > 0) engine.PlayerDrawCards(toDraw);
    }
    public IEffect Clone() => new ScrawlEffect();
}

// Restlessness: if only card in hand, draw N and gain N energy
public class RestlessnessEffect : IEffect
{
    public int DrawAmount { get; set; }
    public int EnergyAmount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (engine.State.Hand.Count == 1)
        {
            engine.PlayerDrawCards(DrawAmount);
            engine.ApplyEnergyGain(EnergyAmount);
        }
    }
    public IEffect Clone() => new RestlessnessEffect { DrawAmount = DrawAmount, EnergyAmount = EnergyAmount };
}

// Volley: X-cost, deal damage × X to random enemies
public class VolleyEffect : IEffect
{
    public decimal DamagePerHit { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int x = engine.State.Energy;
        engine.State.Energy = 0;
        var alive = engine.State.Enemies.Where(e => e.IsAlive).ToList();
        for (int i = 0; i < x; i++)
        {
            if (alive.Count == 0) break;
            var enemy = alive[engine.State.Rng.Next(alive.Count)];
            decimal dmg = engine.CalculateDamage(source, enemy, DamagePerHit, 1, ValueProp.Move, null);
            enemy.LoseHp(dmg);
        }
    }
    public IEffect Clone() => new VolleyEffect { DamagePerHit = DamagePerHit };
}

// JackOfAllTrades: generate N random colorless cards
public class JackOfAllTradesEffect : IEffect
{
    public int Count { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        for (int i = 0; i < Count; i++)
        {
            var card = new SimCard
            {
                Id = "JACK_GENERATED", Name = "Card", Cost = 0,
                Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
                Effects = new List<IEffect> { new DealDamage { BaseAmount = 4, Props = ValueProp.Move } }
            };
            engine.State.Hand.Add(card);
        }
    }
    public IEffect Clone() => new JackOfAllTradesEffect { Count = Count };
}

// Discovery: choose 1 of 3 random cards, make free this turn
public class DiscoveryEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var card = new SimCard
        {
            Id = "DISCOVERED", Name = "Discovered", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } }
        };
        engine.State.Hand.Add(card);
    }
    public IEffect Clone() => new DiscoveryEffect();
}

// Splash: choose an attack from another class, make free
public class SplashEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var card = new SimCard
        {
            Id = "SPLASH_GENERATED", Name = "Splashed", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 8, Props = ValueProp.Move } }
        };
        engine.State.Hand.Add(card);
    }
    public IEffect Clone() => new SplashEffect();
}

// Jackpot: gain 3 random 0-cost cards
public class JackpotEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        for (int i = 0; i < 3; i++)
        {
            var card = new SimCard
            {
                Id = "JACKPOT_CARD", Name = "0-Cost", Cost = 0,
                Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
                Effects = new List<IEffect> { new DealDamage { BaseAmount = 4, Props = ValueProp.Move } }
            };
            engine.State.Hand.Add(card);
        }
    }
    public IEffect Clone() => new JackpotEffect();
}

// Alchemize: gain a random potion (simplified: gain energy)
public class AlchemizeEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.ApplyEnergyGain(2);
    }
    public IEffect Clone() => new AlchemizeEffect();
}

// Anointed: draw rare cards to fill hand
public class AnointedEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        int toDraw = 10 - engine.State.Hand.Count;
        if (toDraw > 0) engine.PlayerDrawCards(toDraw);
    }
    public IEffect Clone() => new AnointedEffect();
}

// SecretTechnique/Weapon: draw a card of specific type from draw pile to hand
public class SecretTechniqueEffect : IEffect
{
    public CardType CardType { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var match = engine.State.DrawPile.Cards.FirstOrDefault(c => c.Type == CardType);
        if (match == null && engine.State.DiscardPile.Count > 0)
        {
            engine.ReshuffleDiscardToDraw();
            match = engine.State.DrawPile.Cards.FirstOrDefault(c => c.Type == CardType);
        }
        if (match != null)
        {
            engine.State.DrawPile.Remove(match);
            engine.State.Hand.Add(match);
        }
    }
    public IEffect Clone() => new SecretTechniqueEffect { CardType = CardType };
}

// DarkShackles: temporarily reduce enemy strength
public class DarkShacklesEffect : IEffect
{
    public decimal StrengthLoss { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null) return;
        var str = target.GetPower<StrengthPower>();
        if (str != null)
            str.Amount -= (int)StrengthLoss;
        else
            target.ApplyPower(new StrengthPower { Amount = -(int)StrengthLoss });
    }
    public IEffect Clone() => new DarkShacklesEffect { StrengthLoss = StrengthLoss };
}

// HandOfGreed: deal damage, if target dies gain "gold" (strength)
public class HandOfGreedEffect : IEffect
{
    public decimal DamageAmount { get; set; }
    public decimal GoldAmount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        if (target == null || !target.IsAlive) return;
        decimal hpBefore = target.CurrentHp;
        decimal dmg = engine.CalculateDamage(source, target, DamageAmount, 1, ValueProp.Move, null);
        target.LoseHp(dmg);
        if (hpBefore > 0 && !target.IsAlive)
        {
            var str = source.GetPower<StrengthPower>();
            if (str != null) str.Amount += (int)GoldAmount / 5;
            else source.ApplyPower(new StrengthPower { Amount = (int)GoldAmount / 5 });
        }
    }
    public IEffect Clone() => new HandOfGreedEffect { DamageAmount = DamageAmount, GoldAmount = GoldAmount };
}

// Mimic: gain block equal to target's block
public class MimicEffect : IEffect
{
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var enemy = engine.State.Enemies.FirstOrDefault(e => e.IsAlive);
        decimal blockAmount = enemy?.Block ?? 0;
        if (blockAmount > 0)
            engine.CalculateBlock(source, blockAmount, ValueProp.Move, null);
    }
    public IEffect Clone() => new MimicEffect();
}

// BeatDown: auto-play N attacks from discard pile
public class BeatDownEffect : IEffect
{
    public int CardCount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var attacks = engine.State.DiscardPile.Cards.Where(c => c.Type == CardType.Attack).ToList();
        int count = Math.Min(CardCount, attacks.Count);
        for (int i = 0; i < count; i++)
        {
            var card = attacks[i];
            engine.State.DiscardPile.Remove(card);
            engine.State.Hand.Add(card);
            engine.PlayCard(card);
            if (engine.State.IsCombatOver) break;
        }
    }
    public IEffect Clone() => new BeatDownEffect { CardCount = CardCount };
}

// Catastrophe: auto-play N cards from draw pile
public class CatastropheEffect : IEffect
{
    public int CardCount { get; set; }
    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        for (int i = 0; i < CardCount; i++)
        {
            if (engine.State.DrawPile.Count == 0) break;
            var card = engine.State.DrawPile.Draw();
            if (card == null) break;
            engine.State.Hand.Add(card);
            engine.PlayCard(card);
            if (engine.State.IsCombatOver) break;
        }
    }
    public IEffect Clone() => new CatastropheEffect { CardCount = CardCount };
}
