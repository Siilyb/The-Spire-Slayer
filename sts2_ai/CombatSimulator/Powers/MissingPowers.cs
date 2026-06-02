using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

/// DemisePower: at end of turn, deal Amount unblockable damage to owner
public class DemisePower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterSideTurnEnd(CombatSide side, IEnumerable<SimCreature> participants)
    {
        if (participants.Contains(Owner) && Owner.IsAlive)
            Owner.LoseHp(Amount);
    }

    public override SimPower Clone() => new DemisePower { Amount = Amount };
}

/// RitualPower: at end of turn, gain Amount strength
public class RitualPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterSideTurnEnd(CombatSide side, IEnumerable<SimCreature> participants)
    {
        if (participants.Contains(Owner))
        {
            var str = Owner.GetPower<StrengthPower>();
            if (str != null) str.Amount += Amount;
            else Owner.ApplyPower(new StrengthPower { Amount = Amount });
        }
    }

    public override SimPower Clone() => new RitualPower { Amount = Amount };
}

/// RadiancePower: grants 1 energy at start of each turn, then decrements
public class RadiancePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private bool _usedThisTurn;

    public override void AfterSideTurnStart(CombatSide side, IReadOnlyList<SimCreature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(Owner) && Amount > 0)
        {
            _usedThisTurn = true;
        }
    }

    /// Called by engine after resetting energy to provide the bonus
    public bool TryGrantEnergy(SimEngine engine)
    {
        if (_usedThisTurn && Amount > 0)
        {
            _usedThisTurn = false;
            engine.State.Energy++;
            Amount--;
            return true;
        }
        return false;
    }

    public override SimPower Clone() => new RadiancePower { Amount = Amount, _usedThisTurn = _usedThisTurn };
}

/// ClarityPower: draw 1 extra card each turn
public class ClarityPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new ClarityPower { Amount = Amount };
}

/// RetainHandPower: prevents hand discard at end of turn
public class RetainHandPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new RetainHandPower { Amount = Amount };
}

/// ShrinkPower: reduces damage dealt by the owner by 70%
public class ShrinkPower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override bool AllowNegative => true;

    public override decimal ModifyDamageMultiplicative(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer == Owner && props.IsPoweredAttack())
            return 0.3m;
        return 1m;
    }

    public override SimPower Clone() => new ShrinkPower { Amount = Amount };
}

/// GigantificationPower: next Attack deals 3x damage
public class GigantificationPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private bool _used;

    public override decimal ModifyDamageMultiplicative(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer == Owner && !_used && props.IsPoweredAttack() && card?.Type == CardType.Attack)
        {
            _used = true;
            return 3m;
        }
        return 1m;
    }

    public override SimPower Clone() => new GigantificationPower { Amount = Amount, _used = _used };
}

/// TemporaryStrengthPower: strength that decays (flex potion)
public class TemporaryStrengthPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (dealer == Owner && props.IsPoweredAttack())
            return Amount;
        return 0m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new TemporaryStrengthPower { Amount = Amount };
}

/// TemporaryDexterityPower: dexterity that decays (speed potion)
public class TemporaryDexterityPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyBlockAdditive(SimCreature target, decimal amount, ValueProp props, SimCard? card)
    {
        if (target == Owner && props.IsPoweredCardOrMonsterMoveBlock())
            return Amount;
        return 0m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new TemporaryDexterityPower { Amount = Amount };
}

/// DuplicationPower: next card played by owner gets duplicated (played twice)
public class DuplicationPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public bool Used { get; set; }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new DuplicationPower { Amount = Amount, Used = Used };
}
