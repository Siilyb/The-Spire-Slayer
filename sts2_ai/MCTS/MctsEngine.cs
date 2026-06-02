using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Mcts;

public class MctsEngine
{
    public int Iterations { get; set; } = 1000;
    public double ExplorationC { get; set; } = 1.414;
    public int MaxRolloutDepth { get; set; } = 12;

    private readonly SimEngine _engine;

    public MctsEngine()
    {
        _engine = new SimEngine(new SimState());
    }

    public MctsNode Search(SimState rootState)
    {
        var root = new MctsNode(rootState.Clone());
        root.BuildLegalActions();

        for (int i = 0; i < Iterations; i++)
        {
            var node = TreePolicy(root);
            if (node == null) continue;

            double score = DefaultPolicy(node);
            node.Backpropagate(score);
        }

        return root;
    }

    private MctsNode? TreePolicy(MctsNode node)
    {
        while (!node.IsTerminal)
        {
            if (!node.IsFullyExpanded)
            {
                var child = node.GetNextChild();
                if (child != null)
                {
                    SimulateAction(child);
                    return child;
                }
            }

            var next = node.BestUcbChild(ExplorationC);
            if (next == null) return null;
            node = next;
        }
        return node;
    }

    private void SimulateAction(MctsNode node)
    {
        var (card, target) = node.Action;
        if (card == null || node.State.IsCombatOver) return;

        _engine.State = node.State;
        if (node.Choice?.ChosenCard != null)
            _engine.CurrentChoiceCard = node.Choice.ChosenCard;
        _engine.PlayCard(card, target);
        _engine.CurrentChoiceCard = null;
    }

    private double DefaultPolicy(MctsNode node)
    {
        var state = node.State.Clone();
        _engine.State = state;

        for (int turn = 0; turn < MaxRolloutDepth && !state.IsCombatOver; turn++)
        {
            // === Player turn ===
            state.CurrentSide = CombatSide.Player;
            _engine.BeginPlayerTurn();
            _engine.PlayerDrawCards(5);
            _engine.CheckEtherealOnDraw();

            var actions = RolloutPolicy.GetSmartActions(state, out var plan);
            foreach (var (card, target) in actions)
            {
                if (card == null) break;
                var realCard = state.Hand.Cards.FirstOrDefault(c => c.Id == card.Id);
                if (realCard == null) continue;
                SimCreature? realTarget = null;
                if (target != null)
                    realTarget = state.Enemies.FirstOrDefault(e => e.Name == target.Name);
                _engine.PlayCard(realCard, realTarget);
                if (state.IsCombatOver) break;
            }

            _engine.EndPlayerTurn();
            if (state.IsCombatOver) break;

            // === Enemy turn ===
            state.CurrentSide = CombatSide.Enemy;
            _engine.BeginEnemyTurn();
            foreach (var enemy in state.Enemies.Where(e => e.IsAlive))
            {
                _engine.EnemyTakeTurnDirect(enemy);
                if (state.IsCombatOver) break;
            }
            _engine.EndEnemyTurn();
        }

        return Evaluate(state);
    }

    private double Evaluate(SimState state)
    {
        bool playerAlive = state.Players.Any(p => p.IsAlive);
        bool enemiesAlive = state.Enemies.Any(e => e.IsAlive);

        if (playerAlive && !enemiesAlive)
        {
            var player = state.Players[0];
            return 100.0 + (double)player.CurrentHp / (double)player.MaxHp * 50.0;
        }

        if (!playerAlive)
            return -100.0 - state.TurnNumber * 5.0;

        var p = state.Players[0];
        double hpRatio = (double)p.CurrentHp / (double)p.MaxHp;
        double enemyHp = state.Enemies.Where(e => e.IsAlive).Sum(e => (double)e.CurrentHp);
        double enemyMax = state.Enemies.Where(e => e.IsAlive).Sum(e => (double)(e.MaxHp > 0 ? e.MaxHp : 1));
        return hpRatio * 50.0 - (enemyHp / enemyMax) * 30.0 - state.TurnNumber * 0.1;
    }

    public (SimCard? card, SimCreature? target) FindBestAction(MctsNode root)
    {
        var candidates = root.Children.Where(c => c.Visits > 5).ToList();
        if (candidates.Count == 0)
            candidates = root.Children.Where(c => c.Visits > 0).ToList();
        if (candidates.Count == 0)
            candidates = root.Children;

        return candidates
            .OrderByDescending(c => c.WinRate)
            .ThenByDescending(c => c.Visits)
            .FirstOrDefault()?.Action ?? (null, null);
    }
}
