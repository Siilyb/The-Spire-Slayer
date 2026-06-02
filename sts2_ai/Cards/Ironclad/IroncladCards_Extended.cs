using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Ironclad;

public static partial class IroncladCards
{
    // ==================== UNCOMMON (26) ====================

    public static SimCard AshenStrike()
    {
        return new SimCard
        {
            Id = "ASHEN_STRIKE", Name = "Ashen Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Tags = new HashSet<string> { "Strike" },
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = (e, s) => 6 + e.State.ExhaustPile.Count * 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = (e, s) => 6 + e.State.ExhaustPile.Count * 4 }
            }
        };
    }

    public static SimCard BattleTrance()
    {
        return new SimCard
        {
            Id = "BATTLE_TRANCE", Name = "Battle Trance", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new DrawCards { Amount = 3 },
                new ApplyPower { PowerFactory = () => new NoDrawPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DrawCards { Amount = 4 },
                new ApplyPower { PowerFactory = () => new NoDrawPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Bludgeon()
    {
        return new SimCard
        {
            Id = "BLUDGEON", Name = "Bludgeon", Cost = 3,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 32, Props = ValueProp.Move } },
            UpgradedEffects = new List<IEffect> { new DealDamage { BaseAmount = 42, Props = ValueProp.Move } }
        };
    }

    public static SimCard Bully()
    {
        return new SimCard
        {
            Id = "BULLY", Name = "Bully", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = BullyDamage }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, CalculatedAmount = BullyDamageUpgraded }
            }
        };
    }
    private static decimal BullyDamage(SimEngine e, SimCreature s)
    {
        var target = e.State.Enemies.FirstOrDefault(en => en.IsAlive);
        int vuln = target?.GetPowerAmount<VulnerablePower>() ?? 0;
        return 4 + vuln * 2;
    }
    private static decimal BullyDamageUpgraded(SimEngine e, SimCreature s)
    {
        var target = e.State.Enemies.FirstOrDefault(en => en.IsAlive);
        int vuln = target?.GetPowerAmount<VulnerablePower>() ?? 0;
        return 4 + vuln * 3;
    }

    public static SimCard BurningPact()
    {
        return new SimCard
        {
            Id = "BURNING_PACT", Name = "Burning Pact", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ExhaustFromHand { Amount = 1 },
                new DrawCards { Amount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ExhaustFromHand { Amount = 1 },
                new DrawCards { Amount = 3 }
            }
        };
    }

    public static SimCard Colossus()
    {
        return new SimCard
        {
            Id = "COLOSSUS", Name = "Colossus", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 5, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new ColossusPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 8, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new ColossusPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard DemonicShield()
    {
        return new SimCard
        {
            Id = "DEMONIC_SHIELD", Name = "Demonic Shield", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 1 },
                new GainBlock { BaseAmount = 0, Props = ValueProp.Move, CalculatedBlock = (e, s) => s.Block }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 1 },
                new GainBlock { BaseAmount = 0, Props = ValueProp.Move, CalculatedBlock = (e, s) => s.Block }
            },
            UpgradedKeywordsOverrides = true
        };
    }

    public static SimCard Dismantle()
    {
        return new SimCard
        {
            Id = "DISMANTLE", Name = "Dismantle", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, BaseAmount = 8, HitCount = 1, HitCountFunc = DismantleHitCount }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { Props = ValueProp.Move, BaseAmount = 10, HitCount = 1, HitCountFunc = DismantleHitCount }
            }
        };
    }
    private static int DismantleHitCount(SimEngine e, SimCreature target)
    {
        return target.GetPowerAmount<VulnerablePower>() > 0 ? 2 : 1;
    }

    public static SimCard Dominate()
    {
        return new SimCard
        {
            Id = "DOMINATE", Name = "Dominate", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 1, TargetType = TargetType.SingleEnemy },
                new DominateStrength()
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 2, TargetType = TargetType.SingleEnemy },
                new DominateStrength()
            }
        };
    }

    public static SimCard DrumOfBattle()
    {
        return new SimCard
        {
            Id = "DRUM_OF_BATTLE", Name = "Drum of Battle", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DrawCards { Amount = 2 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DrawCards { Amount = 2 }
            }
        };
    }

    public static SimCard EvilEye()
    {
        return new SimCard
        {
            Id = "EVIL_EYE", Name = "Evil Eye", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new EvilEyeEffect { BaseBlock = 8 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new EvilEyeEffect { BaseBlock = 11 }
            }
        };
    }

    public static SimCard ExpectAFight()
    {
        return new SimCard
        {
            Id = "EXPECT_A_FIGHT", Name = "Expect a Fight", Cost = 2, UpgradedCost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainEnergyPerAttackInHand(),
                new ApplyPower { PowerFactory = () => new NoEnergyGainPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainEnergyPerAttackInHand(),
                new ApplyPower { PowerFactory = () => new NoEnergyGainPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard FeelNoPain()
    {
        return new SimCard
        {
            Id = "FEEL_NO_PAIN", Name = "Feel No Pain", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new FeelNoPainPower(), Amount = 3, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new FeelNoPainPower(), Amount = 4, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard FlameBarrier()
    {
        return new SimCard
        {
            Id = "FLAME_BARRIER", Name = "Flame Barrier", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 12, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new FlameBarrierPower(), Amount = 4, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 16, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new FlameBarrierPower(), Amount = 6, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard ForgottenRitual()
    {
        return new SimCard
        {
            Id = "FORGOTTEN_RITUAL", Name = "Forgotten Ritual", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new ConditionalGainEnergy { Amount = 3, Condition = (e) => e.CardsExhaustedThisTurn > 0 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ConditionalGainEnergy { Amount = 4, Condition = (e) => e.CardsExhaustedThisTurn > 0 }
            }
        };
    }

    public static SimCard Hemokinesis()
    {
        return new SimCard
        {
            Id = "HEMOKINESIS", Name = "Hemokinesis", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 2 },
                new DealDamage { BaseAmount = 15, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 2 },
                new DealDamage { BaseAmount = 20, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard HowlFromBeyond()
    {
        return new SimCard
        {
            Id = "HOWL_FROM_BEYOND", Name = "Howl From Beyond", Cost = 3,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.AllEnemies,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 16, Props = ValueProp.Move },
                new HowlFromBeyondEffect()
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 21, Props = ValueProp.Move },
                new HowlFromBeyondEffect()
            }
        };
    }

    public static SimCard InfernalBlade()
    {
        return new SimCard
        {
            Id = "INFERNAL_BLADE", Name = "Infernal Blade", Cost = 1, UpgradedCost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new GenerateRandomAttackInHand { FreeToPlay = true }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GenerateRandomAttackInHand { FreeToPlay = true }
            }
        };
    }

    public static SimCard Inferno()
    {
        return new SimCard
        {
            Id = "INFERNO", Name = "Inferno", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new InfernoPower(), Amount = 6, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new InfernoPower(), Amount = 9, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Inflame()
    {
        return new SimCard
        {
            Id = "INFLAME", Name = "Inflame", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 2, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 3, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Juggling()
    {
        return new SimCard
        {
            Id = "JUGGLING", Name = "Juggling", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedKeywords = new HashSet<string> { "Innate" }
        };
    }

    public static SimCard Pillage()
    {
        return new SimCard
        {
            Id = "PILLAGE", Name = "Pillage", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 6, Props = ValueProp.Move },
                new DrawUntilCondition { Condition = (c) => c.Type != CardType.Attack }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 9, Props = ValueProp.Move },
                new DrawUntilCondition { Condition = (c) => c.Type != CardType.Attack }
            }
        };
    }

    public static SimCard Rage()
    {
        return new SimCard
        {
            Id = "RAGE", Name = "Rage", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new RagePower(), Amount = 3, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new RagePower(), Amount = 5, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Rampage()
    {
        return new SimCard
        {
            Id = "RAMPAGE", Name = "Rampage", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new RampageDamage { BaseDamage = 9, IncreasePerPlay = 5 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new RampageDamage { BaseDamage = 9, IncreasePerPlay = 9 }
            }
        };
    }

    public static SimCard Rupture()
    {
        return new SimCard
        {
            Id = "RUPTURE", Name = "Rupture", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new RupturePower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new RupturePower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard SecondWind()
    {
        return new SimCard
        {
            Id = "SECOND_WIND", Name = "Second Wind", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new SecondWindEffect { BlockPerCard = 5 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SecondWindEffect { BlockPerCard = 7 }
            }
        };
    }

    public static SimCard Spite()
    {
        return new SimCard
        {
            Id = "SPITE", Name = "Spite", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move, HitCountFunc = SpiteHitCount }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move, HitCountFunc = SpiteHitCountUpgraded }
            }
        };
    }
    private static int SpiteHitCount(SimEngine e, SimCreature t) => e.LostHpThisTurn ? 2 : 1;
    private static int SpiteHitCountUpgraded(SimEngine e, SimCreature t) => e.LostHpThisTurn ? 3 : 1;

    public static SimCard Stampede()
    {
        return new SimCard
        {
            Id = "STAMPEDE", Name = "Stampede", Cost = 2, UpgradedCost = 1,
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

    public static SimCard Stomp()
    {
        return new SimCard
        {
            Id = "STOMP", Name = "Stomp", Cost = 3,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.AllEnemies,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 12, Props = ValueProp.Move }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 15, Props = ValueProp.Move }
            }
        };
    }

    public static SimCard StoneArmor()
    {
        return new SimCard
        {
            Id = "STONE_ARMOR", Name = "Stone Armor", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlatingPower(), Amount = 4, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlatingPower(), Amount = 6, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Taunt()
    {
        return new SimCard
        {
            Id = "TAUNT", Name = "Taunt", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 7, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 1, TargetType = TargetType.SingleEnemy }
            },
            UpgradedEffects = new List<IEffect>
            {
                new GainBlock { BaseAmount = 8, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 2, TargetType = TargetType.SingleEnemy }
            }
        };
    }

    public static SimCard Unrelenting()
    {
        return new SimCard
        {
            Id = "UNRELENTING", Name = "Unrelenting", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 14, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new FreeAttackPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 20, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new FreeAttackPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Uppercut()
    {
        return new SimCard
        {
            Id = "UPPERCUT", Name = "Uppercut", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 13, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new WeakPower(), Amount = 1, TargetType = TargetType.SingleEnemy },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 1, TargetType = TargetType.SingleEnemy }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 13, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new WeakPower(), Amount = 2, TargetType = TargetType.SingleEnemy },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 2, TargetType = TargetType.SingleEnemy }
            }
        };
    }

    public static SimCard Vicious()
    {
        return new SimCard
        {
            Id = "VICIOUS", Name = "Vicious", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new ViciousPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new ViciousPower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    // ==================== RARE (21) ====================

    public static SimCard Aggression()
    {
        return new SimCard
        {
            Id = "AGGRESSION", Name = "Aggression", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedKeywords = new HashSet<string> { "Innate" }
        };
    }

    public static SimCard Barricade()
    {
        return new SimCard
        {
            Id = "BARRICADE", Name = "Barricade", Cost = 3, UpgradedCost = 2,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new BarricadePower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new BarricadePower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Brand()
    {
        return new SimCard
        {
            Id = "BRAND", Name = "Brand", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 1 },
                new ExhaustFromHand { Amount = 1 },
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 1 },
                new ExhaustFromHand { Amount = 1 },
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Cascade()
    {
        return new SimCard
        {
            Id = "CASCADE", Name = "Cascade", Cost = -1, HasXCost = true,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new CascadeEffect { BonusCount = 0 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new CascadeEffect { BonusCount = 1 }
            }
        };
    }

    public static SimCard Conflagration()
    {
        return new SimCard
        {
            Id = "CONFLAGRATION", Name = "Conflagration", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.AllEnemies,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 2, Props = ValueProp.Move, HitCount = 4 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 2, Props = ValueProp.Move, HitCount = 5 }
            }
        };
    }

    public static SimCard CrimsonMantle()
    {
        return new SimCard
        {
            Id = "CRIMSON_MANTLE", Name = "Crimson Mantle", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new CrimsonMantlePower(), Amount = 8, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new CrimsonMantlePower(), Amount = 10, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Cruelty()
    {
        return new SimCard
        {
            Id = "CRUELTY", Name = "Cruelty", Cost = 1,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new CrueltyPower(), Amount = 25, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new CrueltyPower(), Amount = 50, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard DarkEmbrace()
    {
        return new SimCard
        {
            Id = "DARK_EMBRACE", Name = "Dark Embrace", Cost = 2, UpgradedCost = 1,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new DarkEmbracePower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new DarkEmbracePower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard DemonForm()
    {
        return new SimCard
        {
            Id = "DEMON_FORM", Name = "Demon Form", Cost = 3,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new DemonFormPower(), Amount = 2, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new DemonFormPower(), Amount = 3, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Feed()
    {
        return new SimCard
        {
            Id = "FEED", Name = "Feed", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new FeedEffect { DamageAmount = 10, MaxHpGain = 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new FeedEffect { DamageAmount = 12, MaxHpGain = 4 }
            }
        };
    }

    public static SimCard FiendFire()
    {
        return new SimCard
        {
            Id = "FIEND_FIRE", Name = "Fiend Fire", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new FiendFireEffect { DamagePerCard = 7 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new FiendFireEffect { DamagePerCard = 10 }
            }
        };
    }

    public static SimCard Hellraiser()
    {
        return new SimCard
        {
            Id = "HELLRAISER", Name = "Hellraiser", Cost = 2, UpgradedCost = 1,
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

    public static SimCard Impervious()
    {
        return new SimCard
        {
            Id = "IMPERVIOUS", Name = "Impervious", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect> { new GainBlock { BaseAmount = 30, Props = ValueProp.Move } },
            UpgradedEffects = new List<IEffect> { new GainBlock { BaseAmount = 40, Props = ValueProp.Move } }
        };
    }

    public static SimCard Juggernaut()
    {
        return new SimCard
        {
            Id = "JUGGERNAUT", Name = "Juggernaut", Cost = 2,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new JuggernautPower(), Amount = 5, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new JuggernautPower(), Amount = 7, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard NotYet()
    {
        return new SimCard
        {
            Id = "NOT_YET", Name = "Not Yet", Cost = 2,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect> { new Heal { Amount = 10 } },
            UpgradedEffects = new List<IEffect> { new Heal { Amount = 13 } }
        };
    }

    public static SimCard Offering()
    {
        return new SimCard
        {
            Id = "OFFERING", Name = "Offering", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect>
            {
                new SelfDamage { Amount = 6 },
                new GainEnergy { Amount = 2 },
                new DrawCards { Amount = 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new SelfDamage { Amount = 6 },
                new GainEnergy { Amount = 2 },
                new DrawCards { Amount = 5 }
            }
        };
    }

    public static SimCard OneTwoPunch()
    {
        return new SimCard
        {
            Id = "ONE_TWO_PUNCH", Name = "One-Two Punch", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PlaceholderPower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard PactsEnd()
    {
        return new SimCard
        {
            Id = "PACTS_END", Name = "Pact's End", Cost = 0,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.AllEnemies,
            Effects = new List<IEffect>
            {
                new ConditionalDealDamage { BaseAmount = 17, Props = ValueProp.Move, Condition = (e) => e.State.ExhaustPile.Count >= 3 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ConditionalDealDamage { BaseAmount = 23, Props = ValueProp.Move, Condition = (e) => e.State.ExhaustPile.Count >= 3 }
            }
        };
    }

    public static SimCard PrimalForce()
    {
        return new SimCard
        {
            Id = "PRIMAL_FORCE", Name = "Primal Force", Cost = 0,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect> { new PrimalForceEffect() }
        };
    }

    public static SimCard Pyre()
    {
        return new SimCard
        {
            Id = "PYRE", Name = "Pyre", Cost = 2,
            Type = CardType.Power, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PyrePower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new PyrePower(), Amount = 2, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Stoke()
    {
        return new SimCard
        {
            Id = "STOKE", Name = "Stoke", Cost = 1,
            Type = CardType.Skill, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect> { new StokeEffect() },
            UpgradedEffects = new List<IEffect> { new StokeEffect() }
        };
    }

    public static SimCard Tank()
    {
        return new SimCard
        {
            Id = "TANK", Name = "Tank", Cost = 1, UpgradedCost = 0,
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

    public static SimCard TearAsunder()
    {
        return new SimCard
        {
            Id = "TEAR_ASUNDER", Name = "Tear Asunder", Cost = 2,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 5, Props = ValueProp.Move, HitCountFunc = TearAsunderHitCount }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 7, Props = ValueProp.Move, HitCountFunc = TearAsunderHitCount }
            }
        };
    }
    private static int TearAsunderHitCount(SimEngine e, SimCreature t) => 1 + e.UnblockedDamageReceivedThisTurn;

    public static SimCard Thrash()
    {
        return new SimCard
        {
            Id = "THRASH", Name = "Thrash", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Rare, DefaultTargetType = TargetType.SingleEnemy,
            Keywords = new HashSet<string> { "Exhaust" },
            Effects = new List<IEffect> { new ThrashEffect { BaseDamage = 4 } },
            UpgradedEffects = new List<IEffect> { new ThrashEffect { BaseDamage = 6 } }
        };
    }

    public static SimCard Unmovable()
    {
        return new SimCard
        {
            Id = "UNMOVABLE", Name = "Unmovable", Cost = 2, UpgradedCost = 1,
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

    // ==================== ANCIENT (2) ====================

    public static SimCard Break()
    {
        return new SimCard
        {
            Id = "BREAK", Name = "Break", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Ancient, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 20, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 5, TargetType = TargetType.SingleEnemy }
            },
            UpgradedEffects = new List<IEffect>
            {
                new DealDamage { BaseAmount = 30, Props = ValueProp.Move },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 7, TargetType = TargetType.SingleEnemy }
            }
        };
    }

    public static SimCard Corruption()
    {
        return new SimCard
        {
            Id = "CORRUPTION", Name = "Corruption", Cost = 3, UpgradedCost = 2,
            Type = CardType.Power, Rarity = CardRarity.Ancient, DefaultTargetType = TargetType.Self,
            Effects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new CorruptionPower(), Amount = 1, TargetType = TargetType.Self }
            },
            UpgradedEffects = new List<IEffect>
            {
                new ApplyPower { PowerFactory = () => new CorruptionPower(), Amount = 1, TargetType = TargetType.Self }
            }
        };
    }

    public static SimCard Whirlwind()
    {
        return new SimCard
        {
            Id = "WHIRLWIND", Name = "Whirlwind", Cost = 0, HasXCost = true,
            Type = CardType.Attack, Rarity = CardRarity.Uncommon, DefaultTargetType = TargetType.AllEnemies,
            Effects = new List<IEffect>
            {
                new WhirlwindDamage { DamagePerHit = 5 }
            },
            UpgradedEffects = new List<IEffect>
            {
                new WhirlwindDamage { DamagePerHit = 8 }
            }
        };
    }
}
