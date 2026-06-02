using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Mcts;

namespace Sts2AiTests;

public class MctsTests
{
    [Fact]
    public void Mcts_CanSearch_EmptyHand()
    {
        var state = new SimState();
        state.Players.Add(new SimCreature { Name = "Player", Side = CombatSide.Player, CurrentHp = 80, MaxHp = 80 });
        state.Enemies.Add(new SimCreature { Name = "CHOMPER", Side = CombatSide.Enemy, CurrentHp = 60, MaxHp = 64 });

        var engine = new MctsEngine { Iterations = 50 };
        var root = engine.Search(state);

        Assert.NotNull(root);
        Assert.True(root.Visits > 0);
    }

    [Fact]
    public void Mcts_FindsBestAction_WithStrike()
    {
        var state = new SimState();
        state.Players.Add(new SimCreature { Name = "Player", Side = CombatSide.Player, CurrentHp = 80, MaxHp = 80 });
        state.Enemies.Add(new SimCreature { Name = "CHOMPER", Side = CombatSide.Enemy, CurrentHp = 10, MaxHp = 64 });

        state.Hand.Add(new SimCard
        {
            Id = "STRIKE_IRONCLAD", Name = "Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } }
        });
        state.Energy = 3;

        var engine = new MctsEngine { Iterations = 100 };
        var root = engine.Search(state);
        var best = engine.FindBestAction(root);

        Assert.NotNull(best.card);
        Assert.Equal("STRIKE_IRONCLAD", best.card.Id);
    }

    [Fact]
    public void CombatAi_PlaysStrike_WhenEnemyLowHp()
    {
        var state = new SimState();
        state.Players.Add(new SimCreature { Name = "Player", Side = CombatSide.Player, CurrentHp = 80, MaxHp = 80 });
        state.Enemies.Add(new SimCreature { Name = "CHOMPER", Side = CombatSide.Enemy, CurrentHp = 10, MaxHp = 64 });

        state.Hand.Add(new SimCard
        {
            Id = "STRIKE_IRONCLAD", Name = "Strike", Cost = 1,
            Type = CardType.Attack, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.SingleEnemy,
            Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } }
        });
        state.Energy = 3;

        var ai = new CombatAi();
        var actions = ai.ChooseActions(state);

        Assert.NotEmpty(actions);
        Assert.Equal("STRIKE_IRONCLAD", actions[0].Card.Id);
    }
}
