using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Potions;

public static class PotionDb
{
    public static Dictionary<string, Func<SimPotion>> All { get; } = new();

    public static void RegisterAll()
    {
        // ============================================================
        // COMMON - 15
        // ============================================================

        // AttackPotion: choose 1 of 3 random attack cards, set to 0-cost this turn
        Register("ATTACK_POTION", () => new SimPotion
        {
            Id = "ATTACK_POTION", Name = "Attack Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new PotionChooseCardEffect { CardTypeFilter = CardType.Attack } }
        });

        // BlockPotion: gain 12 block (Unpowered)
        Register("BLOCK_POTION", () => new SimPotion
        {
            Id = "BLOCK_POTION", Name = "Block Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new GainBlock { BaseAmount = 12, Props = ValueProp.Unpowered | ValueProp.Move } }
        });

        // ColorlessPotion: choose 1 of 3 random colorless cards, 0-cost this turn
        Register("COLORLESS_POTION", () => new SimPotion
        {
            Id = "COLORLESS_POTION", Name = "Colorless Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new PotionChooseCardEffect { CardPoolFilter = (c) => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Curse } }
        });

        // DexterityPotion: gain 2 dexterity
        Register("DEXTERITY_POTION", () => new SimPotion
        {
            Id = "DEXTERITY_POTION", Name = "Dexterity Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new DexterityPower(), Amount = 2, TargetType = TargetType.Self } }
        });

        // EnergyPotion: gain 2 energy
        Register("ENERGY_POTION", () => new SimPotion
        {
            Id = "ENERGY_POTION", Name = "Energy Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new GainEnergy { Amount = 2 } }
        });

        // ExplosiveAmpoule: deal 10 damage to ALL enemies (Unpowered)
        Register("EXPLOSIVE_AMPOULE", () => new SimPotion
        {
            Id = "EXPLOSIVE_AMPOULE", Name = "Explosive Ampoule", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 10, TargetType = TargetType.AllEnemies, Props = ValueProp.Unpowered | ValueProp.Move } }
        });

        // FirePotion: deal 20 damage to a single enemy (Unpowered)
        Register("FIRE_POTION", () => new SimPotion
        {
            Id = "FIRE_POTION", Name = "Fire Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 20, Props = ValueProp.Unpowered | ValueProp.Move } }
        });

        // FlexPotion: gain 5 temporary strength (FlexPotionPower, decays at end of turn)
        Register("FLEX_POTION", () => new SimPotion
        {
            Id = "FLEX_POTION", Name = "Flex Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 5, TargetType = TargetType.Self } }
        });

        // PowerPotion: choose 1 of 3 random power cards, 0-cost this turn
        Register("POWER_POTION", () => new SimPotion
        {
            Id = "POWER_POTION", Name = "Power Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new PotionChooseCardEffect { CardTypeFilter = CardType.Power } }
        });

        // SkillPotion: choose 1 of 3 random skill cards, 0-cost this turn
        Register("SKILL_POTION", () => new SimPotion
        {
            Id = "SKILL_POTION", Name = "Skill Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new PotionChooseCardEffect { CardTypeFilter = CardType.Skill } }
        });

        // SpeedPotion: gain 5 temporary dexterity (SpeedPotionPower, decays at end of turn)
        Register("SPEED_POTION", () => new SimPotion
        {
            Id = "SPEED_POTION", Name = "Speed Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new TemporaryDexterityPower(), Amount = 5, TargetType = TargetType.Self } }
        });

        // StrengthPotion: gain 2 strength
        Register("STRENGTH_POTION", () => new SimPotion
        {
            Id = "STRENGTH_POTION", Name = "Strength Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 2, TargetType = TargetType.Self } }
        });

        // SwiftPotion: draw 3 cards
        Register("SWIFT_POTION", () => new SimPotion
        {
            Id = "SWIFT_POTION", Name = "Swift Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new DrawCards { Amount = 3 } }
        });

        // VulnerablePotion: apply 3 vulnerable to a single enemy
        Register("VULNERABLE_POTION", () => new SimPotion
        {
            Id = "VULNERABLE_POTION", Name = "Vulnerable Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 3, TargetType = TargetType.SingleEnemy } }
        });

        // WeakPotion: apply 3 weak to a single enemy
        Register("WEAK_POTION", () => new SimPotion
        {
            Id = "WEAK_POTION", Name = "Weak Potion", Rarity = PotionRarity.Common,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new WeakPower(), Amount = 3, TargetType = TargetType.SingleEnemy } }
        });

        // ============================================================
        // UNCOMMON - 15
        // ============================================================

        // BlessingOfTheForge: upgrade ALL cards in hand
        Register("BLESSING_OF_THE_FORGE", () => new SimPotion
        {
            Id = "BLESSING_OF_THE_FORGE", Name = "Blessing of the Forge", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new UpgradeAllInHand() }
        });

        // Clarity: draw 1 card + gain 3 ClarityPower (draw 1 extra card per turn for 3 turns)
        Register("CLARITY", () => new SimPotion
        {
            Id = "CLARITY", Name = "Clarity", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new DrawCards { Amount = 1 }, new ApplyPower { PowerFactory = () => new ClarityPower(), Amount = 3, TargetType = TargetType.Self } }
        });

        // CureAll: gain 1 energy + draw 2
        Register("CURE_ALL", () => new SimPotion
        {
            Id = "CURE_ALL", Name = "Cure-All", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new GainEnergy { Amount = 1 }, new DrawCards { Amount = 2 } }
        });

        // Duplicator: next card played this turn is played twice
        Register("DUPLICATOR", () => new SimPotion
        {
            Id = "DUPLICATOR", Name = "Duplicator", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new DuplicationPower(), Amount = 1, TargetType = TargetType.Self } }
        });

        // Fortifier: double your current block
        Register("FORTIFIER", () => new SimPotion
        {
            Id = "FORTIFIER", Name = "Fortifier", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new FortifierEffect() }
        });

        // FyshOil: gain 1 strength + 1 dexterity
        Register("FYSH_OIL", () => new SimPotion
        {
            Id = "FYSH_OIL", Name = "Fysh Oil", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> {
                new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = 1, TargetType = TargetType.Self },
                new ApplyPower { PowerFactory = () => new DexterityPower(), Amount = 1, TargetType = TargetType.Self }
            }
        });

        // GamblersBrew: discard your entire hand, draw that many cards
        Register("GAMBLERS_BREW", () => new SimPotion
        {
            Id = "GAMBLERS_BREW", Name = "Gambler's Brew", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new GamblersBrewEffect() }
        });

        // HeartOfIron: gain 7 Plating (block each turn)
        Register("HEART_OF_IRON", () => new SimPotion
        {
            Id = "HEART_OF_IRON", Name = "Heart of Iron", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new PlatingPower(), Amount = 7, TargetType = TargetType.Self } }
        });

        // LiquidBronze: gain 3 thorns
        Register("LIQUID_BRONZE", () => new SimPotion
        {
            Id = "LIQUID_BRONZE", Name = "Liquid Bronze", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new ThornsPower(), Amount = 3, TargetType = TargetType.Self } }
        });

        // PotionOfBinding: apply 1 weak + 1 vulnerable to ALL enemies
        Register("POTION_OF_BINDING", () => new SimPotion
        {
            Id = "POTION_OF_BINDING", Name = "Potion of Binding", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> {
                new ApplyPower { PowerFactory = () => new WeakPower(), Amount = 1, TargetType = TargetType.AllEnemies },
                new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 1, TargetType = TargetType.AllEnemies }
            }
        });

        // PowderedDemise: apply 9 Demise to an enemy (deals 9 unblockable damage at end of turn)
        Register("POWDERED_DEMISE", () => new SimPotion
        {
            Id = "POWDERED_DEMISE", Name = "Powdered Demise", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new DemisePower(), Amount = 9, TargetType = TargetType.SingleEnemy } }
        });

        // RadiantTincture: gain 1 energy + gain 3 RadiancePower (1 extra energy each turn for 3 turns)
        Register("RADIANT_TINCTURE", () => new SimPotion
        {
            Id = "RADIANT_TINCTURE", Name = "Radiant Tincture", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new GainEnergy { Amount = 1 }, new ApplyPower { PowerFactory = () => new RadiancePower(), Amount = 3, TargetType = TargetType.Self } }
        });

        // RegenPotion: gain 5 regen (heal at end of turn)
        Register("REGEN_POTION", () => new SimPotion
        {
            Id = "REGEN_POTION", Name = "Regen Potion", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new RegenPower(), Amount = 5, TargetType = TargetType.Self } }
        });

        // StableSerum: retain your hand for 2 turns
        Register("STABLE_SERUM", () => new SimPotion
        {
            Id = "STABLE_SERUM", Name = "Stable Serum", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new RetainHandPower(), Amount = 2, TargetType = TargetType.Self } }
        });

        // TouchOfInsanity: choose a card in hand, set it to 0-cost this combat
        Register("TOUCH_OF_INSANITY", () => new SimPotion
        {
            Id = "TOUCH_OF_INSANITY", Name = "Touch of Insanity", Rarity = PotionRarity.Uncommon,
            Effects = new List<IEffect> { new TouchOfInsanityEffect() }
        });

        // ============================================================
        // RARE - 15
        // ============================================================

        // BeetleJuice: apply 4 ShrinkPower (reduces enemy damage by 70%)
        Register("BEETLE_JUICE", () => new SimPotion
        {
            Id = "BEETLE_JUICE", Name = "Beetle Juice", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new ShrinkPower(), Amount = 4, TargetType = TargetType.SingleEnemy } }
        });

        // BottledPotential: return your hand to draw pile, shuffle, draw 5 cards
        Register("BOTTLED_POTENTIAL", () => new SimPotion
        {
            Id = "BOTTLED_POTENTIAL", Name = "Bottled Potential", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new BottledPotentialEffect() }
        });

        // DistilledChaos: auto-play the top 3 cards of your draw pile
        Register("DISTILLED_CHAOS", () => new SimPotion
        {
            Id = "DISTILLED_CHAOS", Name = "Distilled Chaos", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new DistilledChaosEffect() }
        });

        // DropletOfPrecognition: look at the top of your draw pile, choose a card to add to hand
        Register("DROPLET_OF_PRECOGNITION", () => new SimPotion
        {
            Id = "DROPLET_OF_PRECOGNITION", Name = "Droplet of Precognition", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new SecretTechniqueEffect { CardType = CardType.Attack } }
        });

        // EntropicBrew: fill empty potion slots (in-combat: draw 2 as simplification)
        Register("ENTROPIC_BREW", () => new SimPotion
        {
            Id = "ENTROPIC_BREW", Name = "Entropic Brew", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new DrawCards { Amount = 2 } }
        });

        // FairyInABottle: heal 30% max HP (auto-use on death)
        Register("FAIRY_IN_A_BOTTLE", () => new SimPotion
        {
            Id = "FAIRY_IN_A_BOTTLE", Name = "Fairy in a Bottle", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new Heal { Amount = 30 } }
        });

        // FruitJuice: gain 5 max HP
        Register("FRUIT_JUICE", () => new SimPotion
        {
            Id = "FRUIT_JUICE", Name = "Fruit Juice", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new GainMaxHp { Amount = 5 } }
        });

        // GigantificationPotion: gain GigantificationPower (next attack deals 3x damage)
        Register("GIGANTIFICATION_POTION", () => new SimPotion
        {
            Id = "GIGANTIFICATION_POTION", Name = "Gigantification Potion", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new GigantificationPower(), Amount = 1, TargetType = TargetType.Self } }
        });

        // LiquidMemories: choose a card from your discard pile, add it to hand, make it 0-cost this turn
        Register("LIQUID_MEMORIES", () => new SimPotion
        {
            Id = "LIQUID_MEMORIES", Name = "Liquid Memories", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new SecretTechniqueEffect { CardType = CardType.Attack } }
        });

        // LuckyTonic: gain 1 buffer (absorbs lethal damage once)
        Register("LUCKY_TONIC", () => new SimPotion
        {
            Id = "LUCKY_TONIC", Name = "Lucky Tonic", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new BufferPower(), Amount = 1, TargetType = TargetType.Self } }
        });

        // MazalethsGift: gain 1 RitualPower (gain 1 strength at end of each turn)
        Register("MAZALETHS_GIFT", () => new SimPotion
        {
            Id = "MAZALETHS_GIFT", Name = "Mazaleth's Gift", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new RitualPower(), Amount = 1, TargetType = TargetType.Self } }
        });

        // OrobicAcid: generate 1 random attack + 1 skill + 1 power, all 0-cost
        Register("OROBIC_ACID", () => new SimPotion
        {
            Id = "OROBIC_ACID", Name = "Orobic Acid", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> {
                new GenerateCardOfType { CardType = CardType.Attack },
                new GenerateCardOfType { CardType = CardType.Skill },
                new GenerateCardOfType { CardType = CardType.Power }
            }
        });

        // ShacklingPotion: reduce ALL enemies' strength by 7 this turn
        Register("SHACKLING_POTION", () => new SimPotion
        {
            Id = "SHACKLING_POTION", Name = "Shackling Potion", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new ApplyPower { PowerFactory = () => new StrengthPower(), Amount = -7, TargetType = TargetType.AllEnemies } }
        });

        // ShipInABottle: gain 10 block now, AND 10 block at start of next turn
        Register("SHIP_IN_A_BOTTLE", () => new SimPotion
        {
            Id = "SHIP_IN_A_BOTTLE", Name = "Ship in a Bottle", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new GainBlock { BaseAmount = 10, Props = ValueProp.Unpowered | ValueProp.Move } }
        });

        // SneckoOil: draw 7 cards, randomize their costs (0-3)
        Register("SNECKO_OIL", () => new SimPotion
        {
            Id = "SNECKO_OIL", Name = "Snecko Oil", Rarity = PotionRarity.Rare,
            Effects = new List<IEffect> { new SneckoOilEffect() }
        });
    }

    private static void Register(string id, Func<SimPotion> factory) => All[id] = factory;
}
