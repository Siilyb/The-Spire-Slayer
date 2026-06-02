using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers; using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Mcts;

public class MctsNode
{
    public SimState State { get; }
    public int Visits { get; private set; }
    public double TotalScore { get; private set; }
    public double WinRate => Visits > 0 ? TotalScore / Visits : 0;
    public MctsNode? Parent { get; }
    public List<MctsNode> Children { get; } = new();
    public (SimCard? card, SimCreature? target) Action { get; }
    public ChoiceData? Choice { get; set; }

    public bool IsFullyExpanded => _nextActionIndex >= _legalActions.Count;
    public bool IsTerminal => State.IsCombatOver;

    private List<(SimCard? card, SimCreature? target, ChoiceData? choice, double priority)> _legalActions = new();
    private int _nextActionIndex;

    public MctsNode(SimState state, MctsNode? parent = null, (SimCard? card, SimCreature? target) action = default)
    {
        State = state;
        Parent = parent;
        Action = action;
    }

    public void BuildLegalActions()
    {
        _legalActions = new List<(SimCard? card, SimCreature? target, ChoiceData? choice, double priority)>();
        var aliveEnemies = State.Enemies.Where(e => e.IsAlive).ToList();
        int energy = State.Energy;

        foreach (var card in State.Hand.Cards.ToList())
        {
            if (card.HasKeyword("Unplayable")) continue;
            int cost = card.HasXCost ? energy : card.CurrentCost;
            if (energy < cost) continue;

            // Cards that need a choice from hand — expand sub-actions
            var choiceEffects = GetChoiceEffects(card);
            if (choiceEffects.Count > 0)
            {
                foreach (var effect in choiceEffects)
                    ExpandChoiceActions(card, effect, energy);
                continue;
            }

            double priority = cost == 0 ? 0.5 : 0;

            if (card.DefaultTargetType == TargetType.SingleEnemy && aliveEnemies.Count > 0)
            {
                foreach (var enemy in aliveEnemies)
                {
                    double p = priority + (enemy.CurrentHp > 0 ? 100.0 / (double)enemy.CurrentHp : 100);
                    _legalActions.Add((card, enemy, null, p));
                }
            }
            else
            {
                _legalActions.Add((card, null, null, priority));
            }
        }

        _legalActions = _legalActions.OrderByDescending(a => a.priority).ToList();
        _legalActions.Add((null, null, null, 0));
    }

    private List<IEffect> GetChoiceEffects(SimCard card)
    {
        var effects = new List<IEffect>();
        foreach (var e in card.CurrentEffects)
        {
            if (e is UpgradeFromHand u && (u.Filter == null || u.Amount == 1))
                effects.Add(e);
            // More choice types can be added here (ExhaustFromHand, etc.)
        }
        return effects;
    }

    private void ExpandChoiceActions(SimCard card, IEffect effect, int energy)
    {
        if (effect is UpgradeFromHand up)
        {
            var candidates = up.Filter != null
                ? State.Hand.Cards.Where(up.Filter).ToList()
                : State.Hand.Cards.Where(c => c.IsUpgraded == false).ToList();

            foreach (var targetCard in candidates)
            {
                _legalActions.Add((card, null, new ChoiceData
                {
                    Type = CardChoiceType.UpgradeFromHand,
                    ChosenCard = targetCard
                }, 10.0));
            }
        }
    }

    public MctsNode? GetNextChild()
    {
        if (_legalActions.Count == 0) BuildLegalActions();
        if (_nextActionIndex >= _legalActions.Count) return null;

        var (actionCard, actionTarget, choice, _) = _legalActions[_nextActionIndex++];
        var clonedState = State.Clone();
        var child = new MctsNode(clonedState, this, (actionCard, actionTarget));
        child.Choice = choice;
        Children.Add(child);
        return child;
    }

    public MctsNode? BestUcbChild(double explorationC)
    {
        MctsNode? best = null;
        double bestValue = double.MinValue;

        foreach (var child in Children)
        {
            if (child.Visits == 0) return child;

            double ucb = child.WinRate + explorationC * Math.Sqrt(Math.Log(Visits) / child.Visits);
            if (ucb > bestValue)
            {
                bestValue = ucb;
                best = child;
            }
        }
        return best;
    }

    public MctsNode MostVisitedChild =>
        Children.OrderByDescending(c => c.Visits).FirstOrDefault()!;

    public void Backpropagate(double score)
    {
        Visits++;
        TotalScore += score;
        Parent?.Backpropagate(score);
    }
}
