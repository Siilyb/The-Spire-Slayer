using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Ironclad;

public static partial class IroncladCards
{
    // ==================== BASIC (3) ====================

    public static SimCard StrikeIronclad()
    {
        return new SimCard
        {
            Id = "STRIKE_IRONCLAD", Name = "Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } },
            UpgradedEffects = new List<IEffect> { new DealDamage { BaseAmount = 9, Props = ValueProp.Move } }
        };
    }

    public static SimCard DefendIronclad()
    {
        return new SimCard
        {
            Id = "DEFEND_IRONCLAD", Name = "Defend", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.Self,
            Tags = new HashSet<string> { "Defend" },
            Effects = new List<IEffect> { new GainBlock { BaseAmount = 5, Props = ValueProp.Move } },
            UpgradedEffects = new List<IEffect> { new GainBlock { BaseAmount = 8, Props = ValueProp.Move } }
        };
    }

    public static SimCard Bash()
    {
        return new SimCard
        {
            Id = "BASH", Name = "Bash", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 8, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 2, TargetType = TargetType.SingleEnemy }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 10, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 3, TargetType = TargetType.SingleEnemy }
            }
        };
    }

    // ==================== COMMON (20) ====================

    public static SimCard Anger()
    {
        return new SimCard
        {
            Id = "ANGER", Name = "Anger", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 6, Props = ValueProp.Move },
                new CreateCardInDiscard()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 8, Props = ValueProp.Move },
                new CreateCardInDiscard()
            }
        };
    }

    public static SimCard Armaments()
    {
        return new SimCard
        {
            Id = "ARMAMENTS", Name = "Armaments", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 5, Props = ValueProp.Move },
                new UpgradeFromHand { Amount = 1, Filter = c => true }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 5, Props = ValueProp.Move },
                new UpgradeAllInHand()
            }
        };
    }

    public static SimCard BloodWall()
    {
        return new SimCard
        {
            Id = "BLOOD_WALL", Name = "Blood Wall", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 2 },
                new GainBlock { BaseAmount = 16, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 2 },
                new GainBlock { BaseAmount = 20, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Bloodletting()
    {
        return new SimCard
        {
            Id = "BLOODLETTING", Name = "Bloodletting", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 3 },
                new GainEnergy { Amount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 3 },
                new GainEnergy { Amount = 3 }
            }
        };
    }

    public static SimCard BodySlam()
    {
        return new SimCard
        {
            Id = "BODY_SLAM", Name = "Body Slam", Cost = 1, UpgradedCost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 0, Props = ValueProp.Move, CalculatedAmount = (engine, source) => source.Block }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 0, Props = ValueProp.Move, CalculatedAmount = (engine, source) => source.Block }
            }
        };
    }

    public static SimCard Breakthrough()
    {
        return new SimCard
        {
            Id = "BREAKTHROUGH", Name = "Breakthrough", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.AllEnemies,
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 1 },
                new DealDamage { BaseAmount = 9, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 1 },
                new DealDamage { BaseAmount = 13, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Cinder()
    {
        return new SimCard
        {
            Id = "CINDER", Name = "Cinder", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 18, Props = ValueProp.Move },
                new ExhaustRandomFromHand()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 24, Props = ValueProp.Move },
                new ExhaustRandomFromHand()
            }
        };
    }

    public static SimCard Havoc()
    {
        return new SimCard
        {
            Id = "HAVOC", Name = "Havoc", Cost = 1, UpgradedCost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new AutoPlayFromDrawPile { ExhaustAfterPlay = true }
            },
            UpgradedEffects = new List<IEffect>
            {
                new AutoPlayFromDrawPile { ExhaustAfterPlay = true }
            }
        };
    }

    public static SimCard Headbutt()
    {
        return new SimCard
        {
            Id = "HEADBUTT", Name = "Headbutt", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 9, Props = ValueProp.Move },
                new MoveCardFromDiscardToDrawTop()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 12, Props = ValueProp.Move },
                new MoveCardFromDiscardToDrawTop()
            }
        };
    }

    public static SimCard IronWave()
    {
        return new SimCard
        {
            Id = "IRON_WAVE", Name = "Iron Wave", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 5, Props = ValueProp.Move },
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 7, Props = ValueProp.Move },
                new DealDamage { BaseAmount = 7, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard MoltenFist()
    {
        return new SimCard
        {
            Id = "MOLTEN_FIST", Name = "Molten Fist", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 10, Props = ValueProp.Move },
                new CopyEnemyVulnerable()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 14, Props = ValueProp.Move },
                new CopyEnemyVulnerable()
            }
        };
    }

    public static SimCard PerfectedStrike()
    {
        return new SimCard
        {
            Id = "PERFECTED_STRIKE", Name = "Perfected Strike", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = PerfectedStrikeDamage }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = PerfectedStrikeDamageUpgraded }
            }
        };
    }

    private static decimal PerfectedStrikeDamage(SimEngine engine, SimCreature source)
    {
        int strikeCount = 1;
        foreach (var card in engine.State.DrawPile.Cards)
            if (card.Tags.Contains("Strike")) strikeCount++;
        foreach (var card in engine.State.Hand.Cards)
            if (card.Tags.Contains("Strike")) strikeCount++;
        foreach (var card in engine.State.DiscardPile.Cards)
            if (card.Tags.Contains("Strike")) strikeCount++;
        return 6 + strikeCount * 2;
    }

    private static decimal PerfectedStrikeDamageUpgraded(SimEngine engine, SimCreature source)
    {
        int strikeCount = 1;
        foreach (var card in engine.State.DrawPile.Cards)
            if (card.Tags.Contains("Strike")) strikeCount++;
        foreach (var card in engine.State.Hand.Cards)
            if (card.Tags.Contains("Strike")) strikeCount++;
        foreach (var card in engine.State.DiscardPile.Cards)
            if (card.Tags.Contains("Strike")) strikeCount++;
        return 6 + strikeCount * 3;
    }

    public static SimCard PommelStrike()
    {
        return new SimCard
        {
            Id = "POMMEL_STRIKE", Name = "Pommel Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 9, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 10, Props = ValueProp.Move },
                new DrawCards { Amount = 2 }
            }
        };
    }

    public static SimCard SetupStrike()
    {
        return new SimCard
        {
            Id = "SETUP_STRIKE", Name = "Setup Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 7, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new SetupStrikePower(), Amount = 2, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 9, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new SetupStrikePower(), Amount = 3, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard ShrugItOff()
    {
        return new SimCard
        {
            Id = "SHRUG_IT_OFF", Name = "Shrug It Off", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 8, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 11, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            }
        };
    }

    public static SimCard SwordBoomerang()
    {
        return new SimCard
        {
            Id = "SWORD_BOOMERANG", Name = "Sword Boomerang", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.RandomEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 3, Props = ValueProp.Move, TargetType = TargetType.RandomEnemy, HitCount = 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 3, Props = ValueProp.Move, TargetType = TargetType.RandomEnemy, HitCount = 4 }
            }
        };
    }

    public static SimCard Thunderclap()
    {
        return new SimCard
        {
            Id = "THUNDERCLAP", Name = "Thunderclap", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.AllEnemies,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 4, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 1, TargetType = TargetType.AllEnemies }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 7, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 1, TargetType = TargetType.AllEnemies }
            }
        };
    }

    public static SimCard Tremble()
    {
        return new SimCard
        {
            Id = "TREMBLE", Name = "Tremble", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 3, TargetType = TargetType.SingleEnemy }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 4, TargetType = TargetType.SingleEnemy }
            }
        };
    }

    public static SimCard TrueGrit()
    {
        return new SimCard
        {
            Id = "TRUE_GRIT", Name = "True Grit", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Common, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 7, Props = ValueProp.Move },
                new ExhaustRandomFromHand()
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 9, Props = ValueProp.Move },
                new ExhaustFromHand { Amount = 1 }
            }
        };
    }

    public static SimCard TwinStrike()
    {
        return new SimCard
        {
            Id = "TWIN_STRIKE", Name = "Twin Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Common, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move, HitCount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 7, Props = ValueProp.Move, HitCount = 2 }
            }
        };
    }
}
