using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Colorless;

public static partial class ColorlessCards
{
    // ===== Group A: Simple cards with existing effects =====

    public static SimCard UltimateStrike()
    {
        return new SimCard
        {
            Id = "ULTIMATE_STRIKE", Name = "Ultimate Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 14, Props = ValueProp.Move } },
            UpgradedEffects = new List<IEffect> { new DealDamage { BaseAmount = 20, Props = ValueProp.Move } }
        };
    }

    public static SimCard UltimateDefend()
    {
        return new SimCard
        {
            Id = "ULTIMATE_DEFEND", Name = "Ultimate Defend", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Tags = new HashSet<string> { "Defend" },
            Effects = new List<IEffect> { new GainBlock { BaseAmount = 11, Props = ValueProp.Move } },
            UpgradedEffects = new List<IEffect> { new GainBlock { BaseAmount = 15, Props = ValueProp.Move } }
        };
    }

    public static SimCard Finesse()
    {
        return new SimCard
        {
            Id = "FINESSE", Name = "Finesse", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 4, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 7, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            }
        };
    }

    public static SimCard FlashOfSteel()
    {
        return new SimCard
        {
            Id = "FLASH_OF_STEEL", Name = "Flash of Steel", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 8, Props = ValueProp.Move },
                new DrawCards { Amount = 1 }
            }
        };
    }

    public static SimCard MasterOfStrategy()
    {
        return new SimCard
        {
            Id = "MASTER_OF_STRATEGY", Name = "Master of Strategy", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect> { new DrawCards { Amount = 3 } },
            UpgradedEffects = new List<IEffect> { new DrawCards { Amount = 4 } }
        };
    }

    public static SimCard Production()
    {
        return new SimCard
        {
            Id = "PRODUCTION", Name = "Production", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect> { new GainEnergy { Amount = 2 } },
            UpgradedEffects = new List<IEffect> { new GainEnergy { Amount = 3 } }
        };
    }

    public static SimCard Impatience()
    {
        return new SimCard
        {
            Id = "IMPATIENCE", Name = "Impatience", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ConditionalDraw { Amount = 2, Condition = (e) => !e.State.Hand.Cards.Any(c => c.Type == CardType.Attack) }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ConditionalDraw { Amount = 3, Condition = (e) => !e.State.Hand.Cards.Any(c => c.Type == CardType.Attack) }
            }
        };
    }

    public static SimCard PanicButton()
    {
        return new SimCard
        {
            Id = "PANIC_BUTTON", Name = "Panic Button", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 30, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 2, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 40, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Prowess()
    {
        return new SimCard
        {
            Id = "PROWESS", Name = "Prowess", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 1, TargetType = TargetType.Self },
                new ApplyPower { PowerFactory = () => new DexterityPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 2, TargetType = TargetType.Self },
                new ApplyPower { PowerFactory = () => new DexterityPower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Equilibrium()
    {
        return new SimCard
        {
            Id = "EQUILIBRIUM", Name = "Equilibrium", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 13, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 16, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard EternalArmor()
    {
        return new SimCard
        {
            Id = "ETERNAL_ARMOR", Name = "Eternal Armor", Cost = 3,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlatingPower(), Amount = 9, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlatingPower(), Amount = 12, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Automation()
    {
        return new SimCard
        {
            Id = "AUTOMATION", Name = "Automation", Cost = 1, UpgradedCost = 0,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Mayhem()
    {
        return new SimCard
        {
            Id = "MAYHEM", Name = "Mayhem", Cost = 2, UpgradedCost = 1,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Nostalgia()
    {
        return new SimCard
        {
            Id = "NOSTALGIA", Name = "Nostalgia", Cost = 1, UpgradedCost = 0,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Stratagem()
    {
        return new SimCard
        {
            Id = "STRATAGEM", Name = "Stratagem", Cost = 1, UpgradedCost = 0,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Panache()
    {
        return new SimCard
        {
            Id = "PANACHE", Name = "Panache", Cost = 0,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 10, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 14, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard PrepTime()
    {
        return new SimCard
        {
            Id = "PREP_TIME", Name = "Prep Time", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 4, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 6, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Fasten()
    {
        return new SimCard
        {
            Id = "FASTEN", Name = "Fasten", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 4, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 6, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard RollingBoulder()
    {
        return new SimCard
        {
            Id = "ROLLING_BOULDER", Name = "Rolling Boulder", Cost = 3,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 5, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 10, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Calamity()
    {
        return new SimCard
        {
            Id = "CALAMITY", Name = "Calamity", Cost = 3, UpgradedCost = 2,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Entropy()
    {
        return new SimCard
        {
            Id = "ENTROPY", Name = "Entropy", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedKeywords = new HashSet<string> { "Innate" },
            UpgradedKeywordsOverrides = true
        };
    }
}
