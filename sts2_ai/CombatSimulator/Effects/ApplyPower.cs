using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class ApplyPower : IEffect
{
    public Func<SimPower> PowerFactory { get; set; } = () => new PlaceholderPower();
    public int Amount { get; set; } = 1;
    public TargetType TargetType { get; set; } = TargetType.Self;

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var targets = ResolveTargets(engine, source);
        var template = PowerFactory();
        foreach (var t in targets)
            engine.TryApplyPower(t, template, Amount, source);
    }

    private List<SimCreature> ResolveTargets(SimEngine engine, SimCreature source)
    {
        if (TargetType == TargetType.Self) return new List<SimCreature> { source };
        if (TargetType == TargetType.SingleEnemy)
        {
            var e = engine.State.Enemies.FirstOrDefault(e => e.IsAlive);
            if (e != null) return new List<SimCreature> { e };
        }
        if (TargetType == TargetType.AllEnemies)
            return engine.State.Enemies.Where(e => e.IsAlive).ToList();
        if (TargetType == TargetType.AllAllies)
            return engine.State.Players.Where(p => p.IsAlive).ToList();
        return new List<SimCreature> { source };
    }

    public IEffect Clone() => new ApplyPower { PowerFactory = PowerFactory, Amount = Amount, TargetType = TargetType };
}
