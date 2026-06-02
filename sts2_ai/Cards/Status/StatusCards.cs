using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Status;

public static class StatusCards
{
    public static SimCard Beckon() => new() { Id = "BECKON", Name = "Beckon", Cost = 1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 6 } } };
    public static SimCard Burn() => new() { Id = "BURN", Name = "Burn", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 2 } } };
    public static SimCard Dazed() => new() { Id = "DAZED", Name = "Dazed", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Ethereal", "Unplayable" } };
    public static SimCard Debris() => new() { Id = "DEBRIS", Name = "Debris", Cost = 1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Exhaust" } };
    public static SimCard FranticEscape() => new() { Id = "FRANTIC_ESCAPE", Name = "Frantic Escape", Cost = 1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.Self };
    public static SimCard Infection() => new() { Id = "INFECTION", Name = "Infection", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 3 } } };
    public static SimCard Wither() => new() { Id = "WITHER", Name = "Wither", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" }, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 3 } } };
    public static SimCard Slimed() => new() { Id = "SLIMED", Name = "Slimed", Cost = 1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Exhaust" }, Effects = new List<IEffect> { new DrawCards { Amount = 1 } } };
    public static SimCard Soot() => new() { Id = "SOOT", Name = "Soot", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard Toxic() => new() { Id = "TOXIC", Name = "Toxic", Cost = 1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Exhaust" }, Effects = new List<IEffect> { new CurseTurnEndDamage { Amount = 5 } } };
    public static SimCard Void() => new() { Id = "VOID", Name = "Void", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable", "Ethereal" }, Effects = new List<IEffect> { new VoidEnergyLoss() } };
    public static SimCard Wound() => new() { Id = "WOUND", Name = "Wound", Cost = -1, Type = CardType.Status, Rarity = CardRarity.Status, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
}
