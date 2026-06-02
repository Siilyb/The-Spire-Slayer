using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Mcts;

public class CombatAi : ICombatDecisionMaker
{
    private readonly MctsEngine _mcts = new();
    private SimEngine _engine = new(new SimState());
    private int _maxCardsPerTurn = 10;

    public List<AiAction> ChooseActions(SimState state)
    {
        var actions = new List<AiAction>();
        var workState = state.Clone();
        _engine.State = workState;

        for (int i = 0; i < _maxCardsPerTurn; i++)
        {
            var root = _mcts.Search(workState);
            var best = _mcts.FindBestAction(root);

            if (best.card == null) break;

            var realCard = workState.Hand.Cards.FirstOrDefault(c => c.Id == best.card.Id);
            if (realCard == null) break;

            SimCreature? realTarget = null;
            if (best.target != null)
                realTarget = workState.Enemies.FirstOrDefault(e => e.Name == best.target.Name);

            // Find the best node to get ChoiceData
            var bestNode = root.Children
                .FirstOrDefault(c => c.Action.card?.Id == best.card.Id && c.Visits > 0);

            var act = new AiAction { Card = realCard, Target = realTarget };

            if (bestNode?.Choice?.ChosenCard != null)
            {
                var choiceCard = workState.Hand.Cards
                    .FirstOrDefault(c => c.Id == bestNode.Choice.ChosenCard.Id);
                act.ChoiceCard = choiceCard;
            }

            actions.Add(act);
            _engine.CurrentChoiceCard = act.ChoiceCard;
            _engine.PlayCard(realCard, realTarget);
            _engine.CurrentChoiceCard = null;

            if (workState.IsCombatOver) break;
        }

        return actions;
    }
}
