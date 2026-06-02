namespace Sts2Ai.CombatSimulator.Powers;

public class PlaceholderPower : SimPower
{
    public override SimPower Clone() => new PlaceholderPower { Amount = Amount };
}
