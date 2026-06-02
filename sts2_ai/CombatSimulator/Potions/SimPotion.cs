using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator;

public enum PotionRarity { Common, Uncommon, Rare }

public class SimPotion
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PotionRarity Rarity { get; set; }
    public List<IEffect> Effects { get; set; } = new();

    public void Use(SimEngine engine, SimCreature source)
    {
        foreach (var e in Effects)
            e.Execute(engine, source, null);
    }
}
