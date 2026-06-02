using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Curse;

public static class CurseCards
{
    public static SimCard AscendersBane() => new() { Id = "ASCENDERS_BANE", Name = "Ascender's Bane", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Eternal", "Unplayable", "Ethereal" } };
    public static SimCard BadLuck() => new() { Id = "BAD_LUCK", Name = "Bad Luck", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Eternal", "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 13 } } };
    public static SimCard Clumsy() => new() { Id = "CLUMSY", Name = "Clumsy", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable", "Ethereal" } };
    public static SimCard CurseOfTheBell() => new() { Id = "CURSE_OF_THE_BELL", Name = "Curse of the Bell", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Eternal", "Unplayable" } };
    public static SimCard Debt() => new() { Id = "DEBT", Name = "Debt", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard Decay() => new() { Id = "DECAY", Name = "Decay", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 2 } } };
    public static SimCard Doubt() => new() { Id = "DOUBT", Name = "Doubt", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndApplyPower { PowerFactory = () => new WeakPower(), Amount = 1 } } };
    public static SimCard Enthralled() => new() { Id = "ENTHRALLED", Name = "Enthralled", Cost = 2, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Eternal" } };
    public static SimCard Folly() => new() { Id = "FOLLY", Name = "Folly", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable", "Eternal", "Innate", "Ethereal" } };
    public static SimCard Greed() => new() { Id = "GREED", Name = "Greed", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Eternal", "Unplayable" } };
    public static SimCard Guilty() => new() { Id = "GUILTY", Name = "Guilty", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard Injury() => new() { Id = "INJURY", Name = "Injury", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard Normality() => new() { Id = "NORMALITY", Name = "Normality", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard PoorSleep() => new() { Id = "POOR_SLEEP", Name = "Poor Sleep", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable", "Retain" } };
    public static SimCard Regret() => new() { Id = "REGRET", Name = "Regret", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndHandSizeDamage() } };
    public static SimCard Shame() => new() { Id = "SHAME", Name = "Shame", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndApplyPower { PowerFactory = () => new FrailPower(), Amount = 1 } } };
    public static SimCard SporeMind() => new() { Id = "SPORE_MIND", Name = "Spore Mind", Cost = 1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Exhaust" } };
    public static SimCard Writhe() => new() { Id = "WRITHE", Name = "Writhe", Cost = -1, Type = CardType.Curse, Rarity = CardRarity.Curse, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Innate", "Unplayable" } };
}
