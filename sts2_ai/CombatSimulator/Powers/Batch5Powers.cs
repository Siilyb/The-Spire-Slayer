using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class RagePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterCardPlayed(SimCreature owner, SimCard card)
    {
        if (owner == Owner && card.Type == CardType.Attack)
            Owner.GainBlock(Amount);
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new RagePower { Amount = Amount };
}

public class InfernoPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterCardPlayed(SimCreature owner, SimCard card)
    {
        // This needs engine access - handled via simulation context
    }

    public override SimPower Clone() => new InfernoPower { Amount = Amount };
}

public class CrimsonMantlePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterCardPlayed(SimCreature owner, SimCard card)
    {
        if (owner == Owner && card.Type == CardType.Attack)
            Owner.GainBlock(Amount);
    }

    public override SimPower Clone() => new CrimsonMantlePower { Amount = Amount };
}

public class CrueltyPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void BeforeCardPlayed(SimCreature owner, SimCard card)
    {
        if (owner == Owner && card.Type == CardType.Attack)
        {
            var vuln = Owner.GetPower<VulnerablePower>();
            if (vuln != null)
                vuln.Amount += Amount;
            else
                Owner.ApplyPower(new VulnerablePower { Amount = Amount });
        }
    }

    public override SimPower Clone() => new CrueltyPower { Amount = Amount };
}

public class FreeAttackPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _used;

    public override int ModifyCardCost(int originalCost, SimCard card)
    {
        if (!_used && card.Type == CardType.Attack && originalCost > 0)
        {
            _used = true;
            return 0;
        }
        return originalCost;
    }

    public override SimPower Clone() => new FreeAttackPower { Amount = Amount };
}

public class PyrePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterSideTurnStart(CombatSide side, IReadOnlyList<SimCreature> participants)
    {
        if (participants.Contains(Owner) && side == CombatSide.Player)
        {
            // Energy gain handled by engine
        }
    }

    public override SimPower Clone() => new PyrePower { Amount = Amount };
}

public class ViciousPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterCardPlayed(SimCreature owner, SimCard card)
    {
        // Triggers when debuff is applied - handled via engine
    }

    public override SimPower Clone() => new ViciousPower { Amount = Amount };
}

public class ManglePower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(SimCreature? target, decimal amount, ValueProp props, SimCreature? dealer, SimCard? card)
    {
        if (target == Owner && dealer != null)
        {
            var str = dealer.GetPower<StrengthPower>();
            if (str != null && str.Amount > 0)
                str.Amount = Math.Max(0, str.Amount - Amount);
        }
        return 0m;
    }

    public override void TickDown()
    {
        if (Amount > 0) Amount--;
    }

    public override SimPower Clone() => new ManglePower { Amount = Amount };
}

public class NoEnergyGainPower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyEnergyGain(decimal amount) => 0m;

    public override SimPower Clone() => new NoEnergyGainPower { Amount = Amount };
}
