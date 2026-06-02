using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator.Effects;

public interface IEffect
{
    void Execute(SimEngine engine, SimCreature source, SimCreature? target);
    IEffect Clone();
}
