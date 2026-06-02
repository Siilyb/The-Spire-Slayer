using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class DemonFormPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterSideTurnStart(CombatSide side, IReadOnlyList<SimCreature> participants)
    {
        if (participants.Contains(Owner))
        {
            var str = Owner.GetPower<StrengthPower>();
            if (str != null)
                str.Amount += Amount;
            else
                Owner.ApplyPower(new StrengthPower { Amount = Amount });
        }
    }

    public override SimPower Clone() => new DemonFormPower { Amount = Amount };
}

public class DarkEmbracePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _etherealCount;

    public override void AfterCardExhausted(SimCreature owner, SimCard card)
    {
        if (owner != Owner) return;
        if (card.HasKeyword("Ethereal"))
            _etherealCount++;
    }

    public override void AfterSideTurnEnd(CombatSide side, IEnumerable<SimCreature> participants)
    {
        if (participants.Contains(Owner) && _etherealCount > 0)
        {
            // Draw will be handled by the engine
            _etherealCount = 0;
        }
    }

    public override SimPower Clone() => new DarkEmbracePower { Amount = Amount };
}

public class FeelNoPainPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterCardExhausted(SimCreature owner, SimCard card)
    {
        if (owner == Owner)
            Owner.GainBlock(Amount);
    }

    public override SimPower Clone() => new FeelNoPainPower { Amount = Amount };
}

public class FlameBarrierPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void BeforeDamageReceived(SimCreature target, ref decimal amount, SimCreature? dealer, ValueProp props)
    {
        if (target == Owner && dealer != null && amount > 0)
        {
            dealer.LoseHp(Amount);
        }
    }

    public override SimPower Clone() => new FlameBarrierPower { Amount = Amount };
}

public class JuggernautPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void AfterBlockGained(SimCreature creature, decimal amount, ValueProp props, SimCard? card)
    {
        if (amount > 0 && creature == Owner)
        {
            var alive = creature.CustomState.ContainsKey("Enemies")
                ? new List<SimCreature>()
                : new List<SimCreature>();
            // Random enemy targeting handled via engine access
            if (alive.Count == 0) return;
            int idx = new Random().Next(alive.Count);
            alive[idx].LoseHp(Amount);
        }
    }

    public override SimPower Clone() => new JuggernautPower { Amount = Amount };
}

public class NoDrawPower : SimPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldDraw(bool fromHandDraw)
    {
        return fromHandDraw;
    }

    public override SimPower Clone() => new NoDrawPower { Amount = Amount };
}

public class RupturePower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override void BeforeDamageReceived(SimCreature target, ref decimal amount, SimCreature? dealer, ValueProp props)
    {
        if (target == Owner && amount > 0)
        {
            var str = target.GetPower<StrengthPower>();
            if (str != null)
                str.Amount += Amount;
            else
                target.ApplyPower(new StrengthPower { Amount = Amount });
        }
    }

    public override SimPower Clone() => new RupturePower { Amount = Amount };
}

public class CorruptionPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override int ModifyCardCost(int originalCost, SimCard card)
    {
        if (card.Type == CardType.Skill) return 0;
        return originalCost;
    }

    public override bool ShouldCardExhaust(SimCard card)
    {
        return card.Type == CardType.Skill;
    }

    public override SimPower Clone() => new CorruptionPower { Amount = Amount };
}
