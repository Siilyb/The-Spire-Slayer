using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public abstract class SimPower
{
    public int Amount { get; set; }
    public virtual PowerType Type { get; set; }
    public virtual PowerStackType StackType { get; set; } = PowerStackType.Counter;
    public virtual bool AllowNegative { get; set; }
    public SimCreature? Owner { get; set; }

    // === 伤害修改 ===
    // 返回要添加的伤害加值（不是修改后的总数），默认 0
    public virtual decimal ModifyDamageAdditive(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
        => 0m;

    // 返回伤害乘数，默认 1（无变化）
    public virtual decimal ModifyDamageMultiplicative(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
        => 1m;

    // 返回伤害上限，默认 MaxValue（无上限）
    public virtual decimal ModifyDamageCap(SimCreature? target, ValueProp props, SimCreature? dealer, SimCard? card)
        => decimal.MaxValue;

    // === 格挡修改 ===
    public virtual decimal ModifyBlockAdditive(SimCreature target, decimal amount, ValueProp props, SimCard? card)
        => 0m;

    public virtual decimal ModifyBlockMultiplicative(SimCreature target, decimal amount, ValueProp props, SimCard? card)
        => 1m;

    // === HP 损失修改（Buffer/Intangible 用） ===
    public virtual decimal ModifyHpLostAfterOsty(SimCreature target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
        => amount;

    public virtual decimal ModifyHpLostAfterOstyLate(SimCreature target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
        => amount;

    // === 伤害事件 ===
    public virtual void BeforeDamageReceived(SimCreature target, ref decimal amount, SimCreature? dealer, ValueProp props) { }

    // === 格挡事件 ===
    public virtual void AfterBlockGained(SimCreature creature, decimal amount, ValueProp props, SimCard? card) { }

    // === 卡牌事件 ===
    public virtual void BeforeCardPlayed(SimCreature owner, SimCard card) { }
    public virtual void AfterCardPlayed(SimCreature owner, SimCard card) { }
    public virtual void AfterCardExhausted(SimCreature owner, SimCard card) { }

    // === 回合事件 ===
    public virtual void AfterSideTurnStart(CombatSide side, IReadOnlyList<SimCreature> participants) { }
    public virtual void AfterSideTurnEnd(CombatSide side, IEnumerable<SimCreature> participants) { }
    public virtual void BeforeSideTurnEndEarly(CombatSide side, IEnumerable<SimCreature> participants) { }

    // === 抽牌 ===
    public virtual bool ShouldDraw(bool fromHandDraw) => true;

    // === 格挡清除 ===
    public virtual bool ShouldClearBlock(SimCreature creature) => true;

    // === Power 拦截 ===
    public virtual bool TryModifyPowerAmountReceived(SimPower incomingPower, SimCreature target, decimal amount, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        return false;
    }

    // === 能量 ===
    public virtual decimal ModifyEnergyGain(decimal amount) => amount;

    // === 消耗 ===
    public virtual bool ShouldCardExhaust(SimCard card) => false;

    // === 费用 ===
    public virtual int ModifyCardCost(int originalCost, SimCard card) => originalCost;

    // === 生命周期 ===
    public virtual void TickDown() { }
    public abstract SimPower Clone();
}
