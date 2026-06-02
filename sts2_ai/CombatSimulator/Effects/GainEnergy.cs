using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public class GainEnergy : IEffect
{
    public int Amount { get; set; }

    public void Execute(SimEngine engine, SimCreature source, SimCreature? target)
    {
        engine.ApplyEnergyGain(Amount);
    }

    public IEffect Clone() => new GainEnergy { Amount = Amount };
}
