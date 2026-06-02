using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Enemies;

namespace Sts2Ai.Mcts;

public static class RolloutPolicy
{
    public static List<(SimCard? card, SimCreature? target)> GetSmartActions(SimState state, out EnemyTurnPlan? enemyPlan)
    {
        enemyPlan = null;
        var actions = new List<(SimCard? card, SimCreature? target)>();
        var aliveEnemies = state.Enemies.Where(e => e.IsAlive).ToList();
        if (aliveEnemies.Count == 0) { actions.Add((null, null)); return actions; }

        var player = state.Players[0];
        var rng = state.Rng;

        var firstEnemy = aliveEnemies[0];
        var ai = firstEnemy.CustomState.TryGetValue("__ai", out var a) ? a as IEnemyAi : null;
        enemyPlan = ai?.PlanNextTurn(state, firstEnemy);

        bool enemyIsAttacking = enemyPlan != null &&
            (enemyPlan.Intent == IntentType.Attack || enemyPlan.Intent == IntentType.DeathBlow);

        // Collect playable cards with neutral weights
        var playable = new List<(SimCard card, double weight, SimCreature? target, int cost)>();

        foreach (var card in state.Hand.Cards)
        {
            if (card.HasKeyword("Unplayable")) continue;
            int cost = card.HasXCost ? state.Energy : card.CurrentCost;
            if (state.Energy < cost) continue;

            double weight = 1.0;
            if (cost == 0) weight = 1.3;

            SimCreature? target = null;
            if (card.DefaultTargetType == TargetType.SingleEnemy && aliveEnemies.Count > 0)
                target = aliveEnemies.OrderBy(e => e.CurrentHp).First();
            else if (card.DefaultTargetType == TargetType.RandomEnemy && aliveEnemies.Count > 0)
                target = aliveEnemies[rng.Next(aliveEnemies.Count)];

            playable.Add((card, weight, target, cost));
        }

        var remaining = new List<(SimCard card, double weight, SimCreature? target, int cost)>(playable);
        int usedEnergy = 0;

        while (remaining.Count > 0 && usedEnergy < state.Energy)
        {
            double totalWeight = remaining.Sum(h => h.weight);
            double roll = rng.NextDouble() * totalWeight;
            double cumulative = 0;
            int idx = 0;
            for (int i = 0; i < remaining.Count; i++)
            {
                cumulative += remaining[i].weight;
                if (roll <= cumulative) { idx = i; break; }
            }

            var chosen = remaining[idx];
            actions.Add((chosen.card, chosen.target));
            usedEnergy += chosen.cost;
            remaining.RemoveAt(idx);
        }

        actions.Add((null, null));
        return actions;
    }
}
