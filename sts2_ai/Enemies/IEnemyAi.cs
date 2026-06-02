using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Enemies;

public enum IntentType
{
    Attack, Buff, Debuff, DebuffStrong, Defend, Escape, Heal,
    Hidden, Sleep, Stun, StatusCard, CardDebuff, Summon, DeathBlow, Unknown
}

public class EnemyTurnPlan
{
    public IntentType Intent { get; set; }
    public int Damage { get; set; }
    public int Block { get; set; }
    public int Hits { get; set; } = 1;
    public int BuffAmount { get; set; }
    public int DebuffAmount { get; set; }
    public int HealAmount { get; set; }
    public string? StatusCardId { get; set; }
    public int StatusCardCount { get; set; }
    public ValueProp DamageProps { get; set; } = ValueProp.Move;
}

public interface IEnemyAi
{
    string EnemyId { get; }
    int MinHp { get; }
    int MaxHp { get; }
    void OnCombatStart(SimCreature enemy);
    EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy);
}
