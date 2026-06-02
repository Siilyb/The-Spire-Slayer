using Sts2Ai.CombatSimulator.Effects;

namespace Sts2Ai.CombatSimulator.State;

public class SimCard
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Cost { get; set; }
    public int UpgradedCost { get; set; } = -1;
    public CardType Type { get; set; }
    public CardRarity Rarity { get; set; }
    public TargetType DefaultTargetType { get; set; }
    public bool HasXCost { get; set; }
    public bool IsUpgraded { get; set; }

    public List<IEffect> Effects { get; set; } = new();
    public List<IEffect> UpgradedEffects { get; set; } = new();

    public HashSet<string> Keywords { get; set; } = new();
    public HashSet<string> UpgradedKeywords { get; set; } = new();
    public HashSet<string> Tags { get; set; } = new();

    public int CurrentCost => IsUpgraded && UpgradedCost >= 0 ? UpgradedCost : Cost;
    public List<IEffect> CurrentEffects => IsUpgraded && UpgradedEffects.Count > 0 ? UpgradedEffects : Effects;
    public HashSet<string> CurrentKeywords => IsUpgraded && UpgradedKeywords.Count > 0 ? UpgradedKeywords : Keywords;

    public bool HasKeyword(string keyword) => CurrentKeywords.Contains(keyword);

    public SimCard Clone()
    {
        return new SimCard
        {
            Id = Id,
            Name = Name,
            Cost = Cost,
            UpgradedCost = UpgradedCost,
            Type = Type,
            Rarity = Rarity,
            DefaultTargetType = DefaultTargetType,
            HasXCost = HasXCost,
            IsUpgraded = IsUpgraded,
            Effects = Effects.Select(e => e.Clone()).ToList(),
            UpgradedEffects = UpgradedEffects.Select(e => e.Clone()).ToList(),
            Keywords = new HashSet<string>(Keywords),
            UpgradedKeywords = new HashSet<string>(UpgradedKeywords),
            Tags = new HashSet<string>(Tags)
        };
    }
}
