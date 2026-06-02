using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Mcts;

public enum CardChoiceType
{
    None,
    UpgradeFromHand,
    ExhaustFromHand,
    DuplicateInHand,
}

public class ChoiceData
{
    public CardChoiceType Type { get; set; }
    public SimCard? ChosenCard { get; set; }
}
