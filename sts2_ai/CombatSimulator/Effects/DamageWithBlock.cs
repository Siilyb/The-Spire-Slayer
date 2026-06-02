using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class DamageWithBlock : IEffect
{
    public decimal BaseAmount { get; set; }
    public TargetType TargetType { get; set; } = TargetType.SingleEnemy;
    public decimal Multiplier { get; set; } = 1;

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        decimal totalDamage = BaseAmount + source.Block * Multiplier;
        var targets = TargetType == TargetType.SingleEnemy && target != null
            ? new List<SimCreature> { target }
            : engine.State.Enemies.Where(e => e.IsAlive).ToList();

        foreach (var t in targets)
        {
            decimal dmg = engine.CalculateDamage(source, t, totalDamage, 1, ValueProp.Move, null);
            t.LoseHp(dmg);
        }
    }

    public IEffect Clone() => new DamageWithBlock { BaseAmount = BaseAmount, TargetType = TargetType, Multiplier = Multiplier };
}
