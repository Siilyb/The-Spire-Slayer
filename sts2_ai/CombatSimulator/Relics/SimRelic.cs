namespace Sts2Ai.CombatSimulator;

/// Relics modify combat through hooks, similar to Powers.
/// Most combat-relevant relics can be simulated via existing Power effects.
public class SimRelic
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// Called at combat start to apply any permanent effects (e.g., BurningBlood)
    public virtual void OnCombatStart(SimEngine engine) { }
}
