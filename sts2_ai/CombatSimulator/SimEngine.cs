using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator;

public interface ICombatDecisionMaker
{
    List<SimCard> ChooseCardsToPlay(SimState state);
}

public class SimEngine
{
    public SimState State { get; private set; }
    public ICombatDecisionMaker? PlayerAi { get; set; }
    public SimCard? CurrentPlayingCard { get; set; }

    public SimEngine(SimState state, ICombatDecisionMaker? playerAi = null)
    {
        State = state;
        PlayerAi = playerAi;
    }

    public CombatResult RunFullCombat(int maxTurns = 100)
    {
        for (int turn = 0; turn < maxTurns; turn++)
        {
            State.TurnNumber++;
            State.RoundNumber++;
            if (State.IsCombatOver) break;

            State.CurrentSide = CombatSide.Player;
            BeginPlayerTurn();
            if (State.IsCombatOver) break;

            PlayerDrawCards(5);
            CheckEtherealOnDraw();

            if (PlayerAi != null)
            {
                var cardsToPlay = PlayerAi.ChooseCardsToPlay(State);
                foreach (var card in cardsToPlay)
                {
                    if (!State.Hand.Cards.Contains(card)) continue;
                    PlayCard(card);
                    if (State.IsCombatOver) break;
                }
            }

            EndPlayerTurn();
            if (State.IsCombatOver) break;

            State.CurrentSide = CombatSide.Enemy;
            BeginEnemyTurn();

            foreach (var enemy in State.Enemies.Where(e => e.IsAlive))
            {
                EnemyTakeTurn(enemy);
                if (State.IsCombatOver) break;
            }

            EndEnemyTurn();
        }

        return new CombatResult
        {
            PlayersAlive = State.Players.Any(p => p.IsAlive),
            EnemiesAlive = State.Enemies.Any(e => e.IsAlive),
            TurnNumber = State.TurnNumber,
            FinalState = State
        };
    }

    private void BeginPlayerTurn()
    {
        State.Energy = State.MaxEnergy;
        var participants = State.Players.Where(p => p.IsAlive).ToList() as IReadOnlyList<SimCreature>;
        foreach (var player in State.Players.Where(p => p.IsAlive))
        {
            foreach (var power in player.Powers.ToList())
                power.AfterSideTurnStart(CombatSide.Player, participants);
        }
    }

    public int PlayerDrawCards(int count, bool fromHandDraw = false)
    {
        int drawn = 0;
        for (int i = 0; i < count; i++)
        {
            bool noDraw = State.Players.Any(p =>
                p.Powers.Any(pow => !pow.ShouldDraw(fromHandDraw)));
            if (noDraw) break;

            if (State.DrawPile.Count == 0)
            {
                ReshuffleDiscardToDraw();
                if (State.DrawPile.Count == 0) break;
            }

            var card = State.DrawPile.Draw();
            if (card != null)
            {
                State.Hand.Add(card);
                drawn++;
            }
        }
        return drawn;
    }

    public void ReshuffleDiscardToDraw()
    {
        State.DiscardPile.MoveTo(State.DrawPile);
        State.DrawPile.Shuffle(State.Rng);
    }

    private void CheckEtherealOnDraw()
    {
        var toExhaust = State.Hand.Cards
            .Where(c => c.HasKeyword("Ethereal") && c.HasKeyword("Unplayable"))
            .ToList();
        foreach (var card in toExhaust)
        {
            State.Hand.Remove(card);
            State.ExhaustPile.Add(card);
        }
    }

    public void PlayCard(SimCard card)
    {
        if (!State.Hand.Cards.Contains(card)) return;
        if (card.HasKeyword("Unplayable")) return;

        int modifiedCost = ApplyCardCostHooks(card.CurrentCost, card);
        if (card.HasXCost) modifiedCost = State.Energy;
        if (State.Energy < modifiedCost) return;
        State.Energy -= modifiedCost;

        var source = State.Players[0];
        var target = SelectTarget(card);

        foreach (var power in source.Powers.ToList())
            power.BeforeCardPlayed(source, card);

        CurrentPlayingCard = card;
        foreach (var effect in card.CurrentEffects)
        {
            effect.Execute(this, source, target);
            if (State.IsCombatOver) { CurrentPlayingCard = null; return; }
        }
        CurrentPlayingCard = null;

        foreach (var power in source.Powers.ToList())
            power.AfterCardPlayed(source, card);

        HandleCardDisposition(card);
    }

    private int ApplyCardCostHooks(int baseCost, SimCard card)
    {
        int cost = baseCost;
        foreach (var power in State.Players.SelectMany(p => p.Powers))
            cost = power.ModifyCardCost(cost, card);
        return Math.Max(0, cost);
    }

    private SimCreature? SelectTarget(SimCard card)
    {
        if (card.DefaultTargetType == TargetType.Self || card.DefaultTargetType == TargetType.None)
            return State.Players.FirstOrDefault();
        if (card.DefaultTargetType == TargetType.SingleEnemy)
            return State.Enemies.FirstOrDefault(e => e.IsAlive);
        return null;
    }

    private void HandleCardDisposition(SimCard card)
    {
        State.Hand.Remove(card);

        bool shouldExhaust = card.HasKeyword("Exhaust");
        if (!shouldExhaust)
        {
            foreach (var power in State.Players.SelectMany(p => p.Powers))
            {
                if (power.ShouldCardExhaust(card))
                {
                    shouldExhaust = true;
                    break;
                }
            }
        }

        if (shouldExhaust)
        {
            State.ExhaustPile.Add(card);
            foreach (var power in State.Players.SelectMany(p => p.Powers))
                power.AfterCardExhausted(State.Players[0], card);
        }
        else
        {
            State.DiscardPile.Add(card);
        }
    }

    private void EndPlayerTurn()
    {
        var participants = State.Players.Where(p => p.IsAlive).ToList();

        foreach (var player in participants)
        {
            foreach (var power in player.Powers.ToList())
                power.BeforeSideTurnEndEarly(CombatSide.Player, participants);
        }

        foreach (var player in participants)
        {
            foreach (var power in player.Powers.ToList())
                power.AfterSideTurnEnd(CombatSide.Player, participants);
        }

        bool clearBlock = State.Players.All(p =>
            p.Powers.All(pow => pow.ShouldClearBlock(p)));
        if (clearBlock)
        {
            foreach (var player in State.Players)
                player.Block = 0;
        }

        State.Hand.MoveTo(State.DiscardPile);
    }

    private void BeginEnemyTurn()
    {
        var participants = State.Enemies.Where(e => e.IsAlive).ToList() as IReadOnlyList<SimCreature>;
        foreach (var enemy in State.Enemies.Where(e => e.IsAlive))
        {
            foreach (var power in enemy.Powers.ToList())
                power.AfterSideTurnStart(CombatSide.Enemy, participants);
        }
    }

    private void EnemyTakeTurn(SimCreature enemy)
    {
        var player = State.Players.FirstOrDefault(p => p.IsAlive);
        if (player == null) return;

        int damage = 8;
        damage = (int)CalculateDamage(enemy, player, damage, 1, ValueProp.Move, null);
        player.LoseHp(damage);
    }

    private void EndEnemyTurn()
    {
        var participants = State.Enemies.Where(e => e.IsAlive).ToList();

        foreach (var enemy in State.Enemies.Where(e => e.IsAlive))
        {
            foreach (var power in enemy.Powers.ToList())
                power.BeforeSideTurnEndEarly(CombatSide.Enemy, participants);
        }

        foreach (var enemy in State.Enemies.Where(e => e.IsAlive))
        {
            foreach (var power in enemy.Powers.ToList())
                power.AfterSideTurnEnd(CombatSide.Enemy, participants);
        }

        foreach (var enemy in State.Enemies)
            enemy.Block = 0;
    }

    // ============ 伤害计算流水线 ============

    public decimal CalculateDamage(SimCreature source, SimCreature target, decimal baseAmount, int strengthMultiplier, ValueProp props, SimCard? card)
    {
        decimal damage;

        if (props.IsPoweredAttack())
        {
            decimal additiveBonus = 0m;
            foreach (var power in source.Powers)
                additiveBonus += power.ModifyDamageAdditive(target, baseAmount, props, source, card);
            foreach (var power in target.Powers)
                additiveBonus += power.ModifyDamageAdditive(target, baseAmount, props, source, card);

            damage = baseAmount + additiveBonus;

            foreach (var power in source.Powers)
                damage *= power.ModifyDamageMultiplicative(target, damage, props, source, card);
            foreach (var power in target.Powers)
                damage *= power.ModifyDamageMultiplicative(target, damage, props, source, card);

            decimal cap = decimal.MaxValue;
            foreach (var power in source.Powers)
                cap = Math.Min(cap, power.ModifyDamageCap(target, props, source, card));
            foreach (var power in target.Powers)
                cap = Math.Min(cap, power.ModifyDamageCap(target, props, source, card));
            damage = Math.Min(damage, cap);
        }
        else
        {
            damage = baseAmount;
        }

        foreach (var power in target.Powers.ToList())
            power.BeforeDamageReceived(target, ref damage, source, props);

        if (!props.HasFlag(ValueProp.Unblockable) && target.Block > 0)
        {
            decimal blocked = Math.Min(target.Block, damage);
            target.Block -= blocked;
            damage -= blocked;
        }

        if (damage > 0)
        {
            foreach (var power in target.Powers.ToList())
                damage = power.ModifyHpLostAfterOsty(target, damage, props, source, card);
            foreach (var power in target.Powers.ToList())
                damage = power.ModifyHpLostAfterOstyLate(target, damage, props, source, card);
        }

        damage = Math.Max(damage, 0);
        return damage;
    }

    // ============ 格挡计算流水线 ============

    public decimal CalculateBlock(SimCreature creature, decimal baseAmount, ValueProp props, SimCard? card)
    {
        decimal block;

        if (props.IsPoweredCardOrMonsterMoveBlock())
        {
            decimal additiveBonus = 0m;
            foreach (var power in creature.Powers)
                additiveBonus += power.ModifyBlockAdditive(creature, baseAmount, props, card);

            block = baseAmount + additiveBonus;

            foreach (var power in creature.Powers)
                block *= power.ModifyBlockMultiplicative(creature, block, props, card);

            block = Math.Max(block, 0);
        }
        else
        {
            block = baseAmount;
        }

        creature.GainBlock(block);

        foreach (var power in creature.Powers.ToList())
            power.AfterBlockGained(creature, block, props, card);

        return block;
    }

    // ============ Power 应用（含 Artifact 拦截） ============

    public bool TryApplyPower(SimCreature target, SimPower powerTemplate, decimal amount, SimCreature? applier)
    {
        foreach (var existingPower in target.Powers.ToList())
        {
            decimal modifiedAmount = amount;
            if (existingPower.TryModifyPowerAmountReceived(powerTemplate, target, amount, out modifiedAmount))
            {
                if (modifiedAmount <= 0)
                    return false;
            }
        }

        var existing = target.Powers.FirstOrDefault(p => p.GetType() == powerTemplate.GetType());
        if (existing != null)
        {
            if (existing.StackType == PowerStackType.Counter)
                existing.Amount += (int)amount;
        }
        else
        {
            var clone = powerTemplate.Clone();
            clone.Amount = (int)amount;
            clone.Owner = target;
            target.ApplyPower(clone);
        }

        return true;
    }

    // ============ 能量（含钩子） ============

    public int ApplyEnergyGain(int amount)
    {
        decimal finalAmount = amount;
        foreach (var power in State.Players.SelectMany(p => p.Powers))
            finalAmount = power.ModifyEnergyGain(finalAmount);
        int result = Math.Max(0, (int)finalAmount);
        State.Energy += result;
        return result;
    }
}

public class CombatResult
{
    public bool PlayersAlive { get; set; }
    public bool EnemiesAlive { get; set; }
    public int TurnNumber { get; set; }
    public SimState? FinalState { get; set; }
}
