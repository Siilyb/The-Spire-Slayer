using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class DealDamage : IEffect
{
    public decimal BaseAmount { get; set; }
    public TargetType TargetType { get; set; } = TargetType.SingleEnemy;
    public ValueProp Props { get; set; } = ValueProp.Move;
    public int HitCount { get; set; } = 1;
    public int StrengthMultiplier { get; set; } = 1;
    public Func<SimEngine, SimCreature, decimal>? CalculatedAmount { get; set; }
    public Func<SimEngine, SimCreature, int>? HitCountFunc { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        var targets = ResolveTargets(engine, source, target);
        foreach (var t in targets)
        {
            decimal baseAmt = CalculatedAmount != null ? CalculatedAmount(engine, source) : BaseAmount;
            int hits = HitCountFunc != null ? HitCountFunc(engine, t) : HitCount;
            for (int i = 0; i < hits; i++)
            {
                decimal dmg = engine.CalculateDamage(source, t, baseAmt, StrengthMultiplier, Props, null);
                t.LoseHp(dmg);
            }
        }
    }

    private List<SimCreature> ResolveTargets(SimEngine engine, SimCreature source, SimCreature? specific)
    {
        if (TargetType == TargetType.SingleEnemy && specific != null)
            return new List<SimCreature> { specific };
        if (TargetType == TargetType.AllEnemies)
            return engine.State.Enemies.Where(e => e.IsAlive).ToList();
        if (TargetType == TargetType.RandomEnemy)
        {
            var alive = engine.State.Enemies.Where(e => e.IsAlive).ToList();
            if (alive.Count == 0) return new List<SimCreature>();
            return new List<SimCreature> { alive[engine.State.Rng.Next(alive.Count)] };
        }
        if (TargetType == TargetType.Self)
            return new List<SimCreature> { source };
        if (specific != null)
            return new List<SimCreature> { specific };
        return new List<SimCreature>();
    }

    public IEffect Clone() => new DealDamage
    {
        BaseAmount = BaseAmount,
        TargetType = TargetType,
        Props = Props,
        HitCount = HitCount,
        StrengthMultiplier = StrengthMultiplier,
        CalculatedAmount = CalculatedAmount,
        HitCountFunc = HitCountFunc
    };
}
