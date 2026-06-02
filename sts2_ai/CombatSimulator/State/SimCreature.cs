using Sts2Ai.CombatSimulator.Powers;

namespace Sts2Ai.CombatSimulator.State;

public class SimCreature
{
    public string Name { get; set; } = string.Empty;
    public CombatSide Side { get; set; }
    public decimal CurrentHp { get; set; }
    public decimal MaxHp { get; set; }
    public decimal Block { get; set; }
    public bool IsAlive => CurrentHp > 0;
    public Dictionary<string, object> CustomState { get; set; } = new();

    private readonly List<SimPower> _powers = new();
    public IReadOnlyList<SimPower> Powers => _powers;

    public void GainBlock(decimal amount)
    {
        if (amount > 0)
            Block += amount;
    }

    public void LoseHp(decimal amount)
    {
        if (amount > 0)
        {
            CurrentHp -= Math.Min(amount, CurrentHp);
        }
    }

    public void Heal(decimal amount)
    {
        CurrentHp = Math.Min(CurrentHp + amount, MaxHp);
    }

    public void ApplyPower(SimPower power)
    {
        var existing = _powers.FirstOrDefault(p => p.GetType() == power.GetType());
        if (existing != null)
        {
            if (power.StackType == PowerStackType.Counter)
                existing.Amount += power.Amount;
        }
        else
        {
            power.Owner = this;
            _powers.Add(power);
        }
    }

    public void RemovePower(SimPower power) => _powers.Remove(power);

    public T? GetPower<T>() where T : SimPower
        => (T?)_powers.FirstOrDefault(p => p is T);

    public int GetPowerAmount<T>() where T : SimPower
        => GetPower<T>()?.Amount ?? 0;

    public void TickDownPowers()
    {
        var toRemove = new List<SimPower>();
        foreach (var power in _powers)
        {
            power.TickDown();
            if (power.Amount <= 0)
                toRemove.Add(power);
        }
        foreach (var power in toRemove)
            _powers.Remove(power);
    }

    public SimCreature Clone()
    {
        var clone = new SimCreature
        {
            Name = Name,
            Side = Side,
            CurrentHp = CurrentHp,
            MaxHp = MaxHp,
            Block = Block,
            CustomState = new Dictionary<string, object>(CustomState)
        };
        foreach (var power in _powers)
        {
            var pc = power.Clone();
            pc.Owner = clone;
            clone._powers.Add(pc);
        }
        return clone;
    }
}
