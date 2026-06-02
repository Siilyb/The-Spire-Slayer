using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Cards.Quest;

public static class QuestCards
{
    public static SimCard ByrdonisEgg() => new() { Id = "BYRDONIS_EGG", Name = "Byrdonis Egg", Cost = -1, Type = CardType.Quest, Rarity = CardRarity.Quest, DefaultTargetType = TargetType.None, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard LanternKey() => new() { Id = "LANTERN_KEY", Name = "Lantern Key", Cost = -1, Type = CardType.Quest, Rarity = CardRarity.Quest, DefaultTargetType = TargetType.Self, Keywords = new HashSet<string> { "Unplayable" } };
    public static SimCard SpoilsMap() => new() { Id = "SPOILS_MAP", Name = "Spoils Map", Cost = -1, Type = CardType.Quest, Rarity = CardRarity.Quest, DefaultTargetType = TargetType.Self, Keywords = new HashSet<string> { "Unplayable" } };
}
