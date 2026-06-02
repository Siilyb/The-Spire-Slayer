using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Powers;

public class ArtifactPower : SimPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyPowerAmountReceived(SimPower incomingPower, SimCreature target, decimal amount, out decimal modifiedAmount)
    {
        if (target != Owner)
        {
            modifiedAmount = amount;
            return false;
        }
        if (incomingPower.Type != PowerType.Debuff)
        {
            modifiedAmount = amount;
            return false;
        }
        if (Amount <= 0)
        {
            modifiedAmount = amount;
            return false;
        }
        modifiedAmount = 0;
        Amount--;
        return true;
    }

    public override SimPower Clone() => new ArtifactPower { Amount = Amount };
}
