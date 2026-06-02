using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Colorless;

public static partial class ColorlessCards
{
    // ===== Group B: Calculated/conditional cards =====

    public static SimCard MindBlast()
    {
        return new SimCard
        {
            Id = "MIND_BLAST", Name = "Mind Blast", Cost = 1, UpgradedCost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Innate" },
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = (e, s) => e.State.DrawPile.Count }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = (e, s) => e.State.DrawPile.Count }
            }
        };
    }

    public static SimCard GoldAxe()
    {
        return new SimCard
        {
            Id = "GOLD_AXE", Name = "Gold Axe", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = (e, s) => e.State.TurnNumber }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = (e, s) => e.State.TurnNumber }
            },
            UpgradedKeywords = new HashSet<string> { "Retain" },
            UpgradedKeywordsOverrides = true
        };
    }

    public static SimCard Omnislice()
    {
        return new SimCard
        {
            Id = "OMNISLICE", Name = "Omnislice", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new OmnisliceEffect { BaseDamage = 8 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new OmnisliceEffect { BaseDamage = 11 }
            }
        };
    }

    public static SimCard DramaticEntrance()
    {
        return new SimCard
        {
            Id = "DRAMATIC_ENTRANCE", Name = "Dramatic Entrance", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.AllEnemies,
            Keywords = new HashSet<string> { "Exhaust", "Innate" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 11, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 15, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Fisticuffs()
    {
        return new SimCard
        {
            Id = "FISTICUFFS", Name = "Fisticuffs", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new FisticuffsEffect { BaseDamage = 7 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new FisticuffsEffect { BaseDamage = 9 }
            }
        };
    }

    public static SimCard SeekerStrike()
    {
        return new SimCard
        {
            Id = "SEEKER_STRIKE", Name = "Seeker Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 9, Props = ValueProp.Move },
                new SeekerStrikeEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 12, Props = ValueProp.Move },
                new SeekerStrikeEffect()
            }
        };
    }

    public static SimCard Rend()
    {
        return new SimCard
        {
            Id = "REND", Name = "Rend", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = RendDamage }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = RendDamageUpgraded }
            }
        };
    }
    private static decimal RendDamage(SimEngine e, SimCreature s)
    {
        var target = e.State.Enemies.FirstOrDefault(en => en.IsAlive);
        int debuffs = target?.Powers.Count(p => p.Type == PowerType.Debuff) ?? 0;
        return 15 + debuffs * 5;
    }
    private static decimal RendDamageUpgraded(SimEngine e, SimCreature s)
    {
        var target = e.State.Enemies.FirstOrDefault(en => en.IsAlive);
        int debuffs = target?.Powers.Count(p => p.Type == PowerType.Debuff) ?? 0;
        return 18 + debuffs * 8;
    }

    public static SimCard Salvo()
    {
        return new SimCard
        {
            Id = "SALVO", Name = "Salvo", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 12, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 16, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard TheGambit()
    {
        return new SimCard
        {
            Id = "THE_GAMBIT", Name = "The Gambit", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 50, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 75, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Prolong()
    {
        return new SimCard
        {
            Id = "PROLONG", Name = "Prolong", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new ProlongEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new ProlongEffect()
            }
        };
    }

    public static SimCard ThinkingAhead()
    {
        return new SimCard
        {
            Id = "THINKING_AHEAD", Name = "Thinking Ahead", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DrawCards { Amount = 2 },
                new PutCardOnDrawPile { CardFilter = c => true }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DrawCards { Amount = 2 },
                new PutCardOnDrawPile { CardFilter = c => true }
            }
        };
    }

    public static SimCard Scrawl()
    {
        return new SimCard
        {
            Id = "SCRAWL", Name = "Scrawl", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new ScrawlEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new ScrawlEffect()
            },
            UpgradedKeywords = new HashSet<string> { "Retain" },
            UpgradedKeywordsOverrides = true
        };
    }

    public static SimCard Restlessness()
    {
        return new SimCard
        {
            Id = "RESTLESSNESS", Name = "Restlessness", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Retain" },
            Effects = new List<IEffect>
            {
                new RestlessnessEffect { DrawAmount = 2, EnergyAmount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new RestlessnessEffect { DrawAmount = 3, EnergyAmount = 3 }
            }
        };
    }

    public static SimCard Purity()
    {
        return new SimCard
        {
            Id = "PURITY", Name = "Purity", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust", "Retain" },
            Effects = new List<IEffect>
            {
                new ExhaustFromHand { Amount = 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ExhaustFromHand { Amount = 5 }
            }
        };
    }

    public static SimCard Bolas()
    {
        return new SimCard
        {
            Id = "BOLAS", Name = "Bolas", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 3, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 4, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard ThrummingHatchet()
    {
        return new SimCard
        {
            Id = "THRUMMING_HATCHET", Name = "Thrumming Hatchet", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 11, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 14, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Volley()
    {
        return new SimCard
        {
            Id = "VOLLEY", Name = "Volley", Cost = 0, HasXCost = true,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.RandomEnemy,
            Effects = new List<IEffect>
            {
                new VolleyEffect { DamagePerHit = 10 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new VolleyEffect { DamagePerHit = 13 }
            }
        };
    }

    public static SimCard Lift()
    {
        return new SimCard
        {
            Id = "LIFT", Name = "Lift", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 11, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 16, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Intercept()
    {
        return new SimCard
        {
            Id = "INTERCEPT", Name = "Intercept", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 9, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 13, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Rally()
    {
        return new SimCard
        {
            Id = "RALLY", Name = "Rally", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 12, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 17, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard BelieveInYou()
    {
        return new SimCard
        {
            Id = "BELIEVE_IN_YOU", Name = "Believe in You", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainEnergy { Amount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainEnergy { Amount = 3 }
            }
        };
    }

    public static SimCard Coordinate()
    {
        return new SimCard
        {
            Id = "COORDINATE", Name = "Coordinate", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 5, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 8, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard TagTeam()
    {
        return new SimCard
        {
            Id = "TAG_TEAM", Name = "Tag Team", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 11, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 15, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Knockdown()
    {
        return new SimCard
        {
            Id = "KNOCKDOWN", Name = "Knockdown", Cost = 3,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 10, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 14, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard GangUp()
    {
        return new SimCard
        {
            Id = "GANG_UP", Name = "Gang Up", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard HuddleUp()
    {
        return new SimCard
        {
            Id = "HUDDLE_UP", Name = "Huddle Up", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DrawCards { Amount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DrawCards { Amount = 3 }
            }
        };
    }

    public static SimCard BeaconOfHope()
    {
        return new SimCard
        {
            Id = "BEACON_OF_HOPE", Name = "Beacon of Hope", Cost = 1,
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

    // ===== Group C: Need new effects =====

    public static SimCard JackOfAllTrades()
    {
        return new SimCard
        {
            Id = "JACK_OF_ALL_TRADES", Name = "Jack of All Trades", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new JackOfAllTradesEffect { Count = 1 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new JackOfAllTradesEffect { Count = 2 }
            }
        };
    }

    public static SimCard Discovery()
    {
        return new SimCard
        {
            Id = "DISCOVERY", Name = "Discovery", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DiscoveryEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DiscoveryEffect()
            }
        };
    }

    public static SimCard Splash()
    {
        return new SimCard
        {
            Id = "SPLASH", Name = "Splash", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new SplashEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new SplashEffect()
            }
        };
    }

    public static SimCard Jackpot()
    {
        return new SimCard
        {
            Id = "JACKPOT", Name = "Jackpot", Cost = 3,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 25, Props = ValueProp.Move },
                new JackpotEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 30, Props = ValueProp.Move },
                new JackpotEffect()
            }
        };
    }

    public static SimCard Alchemize()
    {
        return new SimCard
        {
            Id = "ALCHEMIZE", Name = "Alchemize", Cost = 1, UpgradedCost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new AlchemizeEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new AlchemizeEffect()
            }
        };
    }

    public static SimCard Anointed()
    {
        return new SimCard
        {
            Id = "ANOINTED", Name = "Anointed", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new AnointedEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new AnointedEffect()
            },
            UpgradedKeywords = new HashSet<string> { "Retain" },
            UpgradedKeywordsOverrides = true
        };
    }

    public static SimCard SecretTechnique()
    {
        return new SimCard
        {
            Id = "SECRET_TECHNIQUE", Name = "Secret Technique", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new SecretTechniqueEffect { CardType = CardType.Skill }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SecretTechniqueEffect { CardType = CardType.Skill }
            }
        };
    }

    public static SimCard SecretWeapon()
    {
        return new SimCard
        {
            Id = "SECRET_WEAPON", Name = "Secret Weapon", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new SecretTechniqueEffect { CardType = CardType.Attack }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SecretTechniqueEffect { CardType = CardType.Attack }
            }
        };
    }

    public static SimCard DarkShackles()
    {
        return new SimCard
        {
            Id = "DARK_SHACKLES", Name = "Dark Shackles", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DarkShacklesEffect { StrengthLoss = 9 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DarkShacklesEffect { StrengthLoss = 15 }
            }
        };
    }

    public static SimCard HandOfGreed()
    {
        return new SimCard
        {
            Id = "HAND_OF_GREED", Name = "Hand of Greed", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new HandOfGreedEffect { DamageAmount = 20, GoldAmount = 20 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new HandOfGreedEffect { DamageAmount = 25, GoldAmount = 25 }
            }
        };
    }

    public static SimCard HiddenGem()
    {
        return new SimCard
        {
            Id = "HIDDEN_GEM", Name = "Hidden Gem", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 1, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 1, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard Mimic()
    {
        return new SimCard
        {
            Id = "MIMIC", Name = "Mimic", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new MimicEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new MimicEffect()
            }
        };
    }

    public static SimCard BeatDown()
    {
        return new SimCard
        {
            Id = "BEAT_DOWN", Name = "Beat Down", Cost = 3,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.RandomEnemy,
            Effects = new List<IEffect>
            {
                new BeatDownEffect { CardCount = 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new BeatDownEffect { CardCount = 4 }
            }
        };
    }

    public static SimCard Catastrophe()
    {
        return new SimCard
        {
            Id = "CATASTROPHE", Name = "Catastrophe", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new CatastropheEffect { CardCount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new CatastropheEffect { CardCount = 3 }
            }
        };
    }

    public static SimCard TheBomb()
    {
        return new SimCard
        {
            Id = "THE_BOMB", Name = "The Bomb", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 40, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 50, TargetType = TargetType.Self }
            }
        };
    }
}
