using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Enemies;

namespace Sts2Ai.CombatSimulator;

public class AiAction
{
    public SimCard Card { get; set; } = null!;
    public SimCreature? Target { get; set; }
    public SimCard? ChoiceCard { get; set; } // For Armaments etc.
}

public interface ICombatDecisionMaker
{
    List<AiAction> ChooseActions(SimState state);
}

public class SimEngine
{
    public SimState State { get; set; }
    public ICombatDecisionMaker? PlayerAi { get; set; }
    public SimCard? CurrentPlayingCard { get; set; }
    public SimCard? CurrentChoiceCard { get; set; }  // For cards that choose from hand (Armaments etc.)

    // Per-turn tracking
    public int CardsExhaustedThisTurn { get; set; }
    public int UnblockedDamageReceivedThisTurn { get; set; }
    public bool LostHpThisTurn { get; set; }

    public SimEngine(SimState state, ICombatDecisionMaker? playerAi = null)
    {
        State = state;
        PlayerAi = playerAi;
    }

    public CombatResult RunFullCombat(int maxTurns = 100, ICombatDecisionMaker? mctsAi = null, Action<SimState, string>? onCardPlayed = null)
    {
        foreach (var enemy in State.Enemies)
        {
            var ai = EnemyDb.Create(enemy.Name.ToUpperInvariant());
            if (ai != null)
            {
                enemy.CustomState["__ai"] = ai;
                ai.OnCombatStart(enemy);
            }
        }

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

            var ai = mctsAi ?? PlayerAi;
            if (ai != null)
            {
                var actions = ai.ChooseActions(State);
                foreach (var act in actions)
                {
                    var realCard = State.Hand.Cards.FirstOrDefault(c => c.Id == act.Card.Id);
                    if (realCard == null) continue;
                    SimCreature? realTarget = null;
                    if (act.Target != null)
                        realTarget = State.Enemies.FirstOrDefault(e => e.Name == act.Target.Name);
                    if (act.ChoiceCard != null)
                        CurrentChoiceCard = State.Hand.Cards.FirstOrDefault(c => c.Id == act.ChoiceCard.Id);
                    PlayCard(realCard, realTarget);
                    CurrentChoiceCard = null;
                    onCardPlayed?.Invoke(State, $"打出: {realCard.Name}");
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

    public void BeginPlayerTurn()
    {
        CardsExhaustedThisTurn = 0;
        UnblockedDamageReceivedThisTurn = 0;
        LostHpThisTurn = false;
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

    public void CheckEtherealOnDraw()
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

    public void PlayCard(SimCard card, SimCreature? forcedTarget = null)
    {
        if (!State.Hand.Cards.Contains(card)) return;
        ExecuteCard(card, forcedTarget);
    }

    /// Play a card that may not be in hand (for HowlFromBeyond auto-replay)
    public void ExecuteCard(SimCard card, SimCreature? forcedTarget = null)
    {
        if (card.HasKeyword("Unplayable")) return;

        int modifiedCost = ApplyCardCostHooks(card.CurrentCost, card);
        if (card.HasXCost) modifiedCost = State.Energy;
        if (State.Energy < modifiedCost) return;
        State.Energy -= modifiedCost;

        var source = State.Players[0];
        var target = forcedTarget ?? SelectTarget(card);

        foreach (var power in source.Powers.ToList())
            power.BeforeCardPlayed(source, card);

        CurrentPlayingCard = card;
        foreach (var effect in card.CurrentEffects)
        {
            effect.Execute(this, source, target);
            if (State.IsCombatOver) { CurrentPlayingCard = null; CurrentChoiceCard = null; return; }
        }
        CurrentPlayingCard = null;
        CurrentChoiceCard = null;

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
        {
            var alive = State.Enemies.Where(e => e.IsAlive).ToList();
            if (alive.Count == 0) return null;
            return alive[State.Rng.Next(alive.Count)]; // MCTS 后续会替换为智能选择
        }
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
            CardsExhaustedThisTurn++;

            // DrumOfBattle: gain energy when exhausted
            if (card.Id == "DRUM_OF_BATTLE")
            {
                int energyAmt = card.IsUpgraded ? 3 : 2;
                State.Energy += energyAmt;
            }

            foreach (var power in State.Players.SelectMany(p => p.Powers))
                power.AfterCardExhausted(State.Players[0], card);

            // HowlFromBeyond: auto-replay from exhaust pile
            bool isHowl = card.CurrentEffects.Any(e => e is Effects.HowlFromBeyondEffect);
            if (isHowl && !State.IsCombatOver)
            {
                State.ExhaustPile.Remove(card);
                ExecuteCard(card);
            }
        }
        else
        {
            State.DiscardPile.Add(card);
        }
    }

    public void EndPlayerTurn()
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

        // Ethereal: exhaust cards with Ethereal keyword before discarding
        var etherealCards = State.Hand.Cards.Where(c => c.HasKeyword("Ethereal")).ToList();
        foreach (var card in etherealCards)
        {
            State.Hand.Remove(card);
            State.ExhaustPile.Add(card);
            foreach (var power in State.Players.SelectMany(p => p.Powers))
                power.AfterCardExhausted(State.Players[0], card);
        }

        State.Hand.MoveTo(State.DiscardPile);
    }

    public void BeginEnemyTurn()
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

        var ai = enemy.CustomState.TryGetValue("__ai", out var stored) ? stored as IEnemyAi : null;
        if (ai == null) return;

        var plan = ai.PlanNextTurn(State, enemy);

        if (plan.Intent == IntentType.Heal && plan.HealAmount > 0)
            enemy.Heal(plan.HealAmount);

        if (plan.Intent == IntentType.Attack || plan.Intent == IntentType.DeathBlow)
        {
            for (int i = 0; i < plan.Hits; i++)
            {
                decimal dmg = CalculateDamage(enemy, player, plan.Damage, 1, plan.DamageProps, null);
                player.LoseHp(dmg);
                if (!player.IsAlive) break;
            }
        }

        if (plan.Block > 0)
            enemy.GainBlock(plan.Block);

        if (plan.BuffAmount > 0)
        {
            var strPower = enemy.GetPower<StrengthPower>();
            if (strPower != null)
                strPower.Amount += plan.BuffAmount;
            else
                enemy.ApplyPower(new StrengthPower { Amount = plan.BuffAmount });
        }

        if (plan.DebuffAmount > 0 && player.IsAlive)
        {
            player.ApplyPower(new VulnerablePower { Amount = plan.DebuffAmount });
        }

        if (plan.StatusCardCount > 0 && !string.IsNullOrEmpty(plan.StatusCardId))
        {
            var kw = new HashSet<string> { "Unplayable" };
            if (plan.StatusCardId == "DAZED" || plan.StatusCardId == "VOID")
                kw.Add("Ethereal");
            if (plan.StatusCardId == "SLIMED" || plan.StatusCardId == "DEBRIS" || plan.StatusCardId == "SPORE_MIND")
                kw = new HashSet<string> { "Exhaust" };
            if (plan.StatusCardId == "BURN" || plan.StatusCardId == "INFECTION" || plan.StatusCardId == "WITHER")
                kw = new HashSet<string> { "Unplayable" };
            if (plan.StatusCardId == "TOXIC")
                kw = new HashSet<string> { "Exhaust" };

            for (int i = 0; i < plan.StatusCardCount; i++)
            {
                var statusCard = new SimCard
                {
                    Id = plan.StatusCardId,
                    Name = plan.StatusCardId,
                    Cost = plan.StatusCardId == "SLIMED" || plan.StatusCardId == "BECKON" || plan.StatusCardId == "DEBRIS" ? 1 : -1,
                    Type = CardType.Status,
                    Rarity = CardRarity.Status,
                    DefaultTargetType = TargetType.None,
                    Keywords = new HashSet<string>(kw)
                };
                State.DiscardPile.Add(statusCard);
            }
        }
    }

    public void EndEnemyTurn()
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
            if (target.Side == CombatSide.Player)
            {
                LostHpThisTurn = true;
                UnblockedDamageReceivedThisTurn++;
            }
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

    /// Direct enemy turn execution for MCTS rollout
    public void EnemyTakeTurnDirect(SimCreature enemy) => EnemyTakeTurn(enemy);

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
