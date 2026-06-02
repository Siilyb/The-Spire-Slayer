using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2AiTests;

public class EffectAndPowerTests
{
    // ===== DealDamage =====

    [Fact]
    public void DealDamage_ReducesEnemyHp()
    {
        var (engine, player, enemy) = CreateCombat();
        var effect = new DealDamage { BaseAmount = 10 };
        effect.Execute(engine, player, enemy);
        Assert.Equal(50, enemy.CurrentHp);
    }

    [Fact]
    public void DealDamage_HonorsBlock()
    {
        var (engine, player, enemy) = CreateCombat();
        enemy.GainBlock(8);
        var effect = new DealDamage { BaseAmount = 10 };
        effect.Execute(engine, player, enemy);
        Assert.Equal(58, enemy.CurrentHp);
    }

    // ===== GainBlock =====

    [Fact]
    public void GainBlock_AddsBlock()
    {
        var (engine, player, _) = CreateCombat();
        var effect = new GainBlock { BaseAmount = 8 };
        effect.Execute(engine, player, null);
        Assert.Equal(8, player.Block);
    }

    // ===== DrawCards =====

    [Fact]
    public void DrawCards_DrawsFromPile()
    {
        var (engine, player, _) = CreateCombat();
        AddCards(engine.State, 5);
        var effect = new DrawCards { Amount = 3 };
        effect.Execute(engine, player, null);
        Assert.Equal(3, engine.State.Hand.Count);
        Assert.Equal(2, engine.State.DrawPile.Count);
    }

    // ===== GainEnergy =====

    [Fact]
    public void GainEnergy_AddsEnergy()
    {
        var (engine, player, _) = CreateCombat();
        engine.State.Energy = 0;
        var effect = new GainEnergy { Amount = 2 };
        effect.Execute(engine, player, null);
        Assert.Equal(2, engine.State.Energy);
    }

    // ===== SelfDamage =====

    [Fact]
    public void SelfDamage_ReducesPlayerHp()
    {
        var (engine, player, _) = CreateCombat();
        var effect = new SelfDamage { Amount = 6 };
        effect.Execute(engine, player, null);
        Assert.Equal(74, player.CurrentHp);
    }

    // ===== Heal =====

    [Fact]
    public void Heal_RestoresHp()
    {
        var (engine, player, _) = CreateCombat();
        player.CurrentHp = 50;
        var effect = new Heal { Amount = 20 };
        effect.Execute(engine, player, null);
        Assert.Equal(70, player.CurrentHp);
    }

    // ===== GainMaxHp =====

    [Fact]
    public void GainMaxHp_IncreasesMaxAndHeals()
    {
        var (engine, player, _) = CreateCombat();
        player.CurrentHp = 70;
        var effect = new GainMaxHp { Amount = 10 };
        effect.Execute(engine, player, null);
        Assert.Equal(90, player.MaxHp);
        Assert.Equal(80, player.CurrentHp);
    }

    // ===== StrengthPower =====

    [Fact]
    public void StrengthPower_AddsDamage()
    {
        var (engine, player, enemy) = CreateCombat();
        player.ApplyPower(new StrengthPower { Amount = 3 });
        decimal dmg = engine.CalculateDamage(player, enemy, 6, 1, ValueProp.Move, null);
        Assert.Equal(9, dmg);
    }

    [Fact]
    public void StrengthPower_DoesNotAffectUnpowered()
    {
        var (engine, player, enemy) = CreateCombat();
        player.ApplyPower(new StrengthPower { Amount = 3 });
        decimal dmg = engine.CalculateDamage(player, enemy, 6, 1, ValueProp.Unpowered, null);
        Assert.Equal(6, dmg);
    }

    // ===== VulnerablePower =====

    [Fact]
    public void VulnerablePower_MultipliesDamage()
    {
        var (engine, player, enemy) = CreateCombat();
        enemy.ApplyPower(new VulnerablePower { Amount = 2 });
        decimal dmg = engine.CalculateDamage(player, enemy, 10, 1, ValueProp.Move, null);
        Assert.Equal(15, dmg);
    }

    // ===== WeakPower =====

    [Fact]
    public void WeakPower_ReducesDamage()
    {
        var (engine, player, enemy) = CreateCombat();
        player.ApplyPower(new WeakPower { Amount = 2 });
        decimal dmg = engine.CalculateDamage(player, enemy, 10, 1, ValueProp.Move, null);
        Assert.Equal(7.5m, dmg);
    }

    // ===== FrailPower =====

    [Fact]
    public void FrailPower_ReducesBlock()
    {
        var (engine, player, _) = CreateCombat();
        player.ApplyPower(new FrailPower { Amount = 2 });
        decimal block = engine.CalculateBlock(player, 10, ValueProp.Move, null);
        Assert.Equal(7.5m, block);
    }

    // ===== Strength + Vulnerable =====

    [Fact]
    public void StrengthAndVulnerable_Stack()
    {
        var (engine, player, enemy) = CreateCombat();
        player.ApplyPower(new StrengthPower { Amount = 3 });
        enemy.ApplyPower(new VulnerablePower { Amount = 2 });
        decimal dmg = engine.CalculateDamage(player, enemy, 6, 1, ValueProp.Move, null);
        Assert.Equal(13.5m, dmg); // (6 + 3) * 1.5 = 13.5
    }

    // ===== DexterityPower =====

    [Fact]
    public void DexterityPower_IncreasesBlock()
    {
        var (engine, player, _) = CreateCombat();
        player.ApplyPower(new DexterityPower { Amount = 2 });
        decimal block = engine.CalculateBlock(player, 5, ValueProp.Move, null);
        Assert.Equal(7, block);
    }

    // ===== BarricadePower =====

    [Fact]
    public void BarricadePower_PreservesBlock()
    {
        var (engine, player, _) = CreateCombat();
        player.ApplyPower(new BarricadePower());
        Assert.False(player.GetPower<BarricadePower>()!.ShouldClearBlock(player));
    }

    // ===== IntangiblePower =====

    [Fact]
    public void IntangiblePower_CapsDamage()
    {
        var (engine, player, enemy) = CreateCombat();
        player.ApplyPower(new IntangiblePower { Amount = 1 });
        decimal dmg = engine.CalculateDamage(enemy, player, 20, 1, ValueProp.Move, null);
        Assert.Equal(1, dmg);
    }

    // ===== BufferPower =====

    [Fact]
    public void BufferPower_AbsorbsFatalHit()
    {
        var (engine, player, enemy) = CreateCombat();
        player.ApplyPower(new BufferPower { Amount = 1 });
        decimal dmg = engine.CalculateDamage(enemy, player, 999, 1, ValueProp.Move, null);
        player.LoseHp(dmg);
        Assert.Equal(80, player.CurrentHp);
        Assert.Equal(0, player.GetPowerAmount<BufferPower>());
    }

    // ===== PlatingPower =====

    [Fact]
    public void PlatingPower_GrantsBlockBeforeTurnEnd()
    {
        var (engine, player, _) = CreateCombat();
        player.ApplyPower(new PlatingPower { Amount = 4 });
        engine.State.CurrentSide = CombatSide.Player;
        foreach (var power in player.Powers.ToList())
            power.BeforeSideTurnEndEarly(CombatSide.Player, new List<SimCreature> { player });
        Assert.Equal(4, player.Block);
    }

    // ===== RegenPower =====

    [Fact]
    public void RegenPower_HealsAtTurnEnd()
    {
        var (engine, player, _) = CreateCombat();
        player.CurrentHp = 40;
        player.ApplyPower(new RegenPower { Amount = 5 });
        foreach (var power in player.Powers.ToList())
            power.AfterSideTurnEnd(CombatSide.Player, new List<SimCreature> { player });
        Assert.Equal(45, player.CurrentHp);
    }

    // ===== ThornsPower =====

    [Fact]
    public void ThornsPower_ReflectsDamage()
    {
        var (engine, player, enemy) = CreateCombat();
        enemy.ApplyPower(new ThornsPower { Amount = 3 });
        decimal damage = engine.CalculateDamage(player, enemy, 10, 1, ValueProp.Move, null);
        enemy.LoseHp(damage);
        Assert.Equal(77, player.CurrentHp); // 80 - 3 thorns
    }

    // ===== PoisonPower =====

    [Fact]
    public void PoisonPower_DamagesAtTurnStart()
    {
        var (engine, _, enemy) = CreateCombat();
        enemy.ApplyPower(new PoisonPower { Amount = 5 });
        foreach (var power in enemy.Powers.ToList())
            power.AfterSideTurnStart(CombatSide.Enemy, new List<SimCreature> { enemy });
        Assert.Equal(55, enemy.CurrentHp);
    }

    // ===== ArtifactPower =====

    [Fact]
    public void ArtifactPower_BlocksDebuff()
    {
        var (engine, player, _) = CreateCombat();
        player.ApplyPower(new ArtifactPower { Amount = 1 });

        var incoming = new VulnerablePower { Amount = 2, Type = PowerType.Debuff };
        decimal modified;
        bool blocked = player.GetPower<ArtifactPower>()!.TryModifyPowerAmountReceived(incoming, player, 2, out modified);

        Assert.True(blocked);
        Assert.Equal(0, modified);
        Assert.Equal(0, player.GetPowerAmount<ArtifactPower>());
    }

    [Fact]
    public void ArtifactPower_DoesNotBlockBuff()
    {
        var (engine, player, _) = CreateCombat();
        player.ApplyPower(new ArtifactPower { Amount = 1 });

        var incoming = new StrengthPower { Amount = 2, Type = PowerType.Buff };
        decimal modified;
        bool blocked = player.GetPower<ArtifactPower>()!.TryModifyPowerAmountReceived(incoming, player, 2, out modified);

        Assert.False(blocked);
        Assert.Equal(2, modified);
    }

    // ===== CorruptionPower =====

    [Fact]
    public void Corruption_MakesSkillsZeroCost()
    {
        var power = new CorruptionPower();
        var skill = new SimCard { Id = "S", Cost = 2, Type = CardType.Skill };
        var attack = new SimCard { Id = "A", Cost = 2, Type = CardType.Attack };

        Assert.Equal(0, power.ModifyCardCost(2, skill));
        Assert.Equal(2, power.ModifyCardCost(2, attack));
        Assert.True(power.ShouldCardExhaust(skill));
        Assert.False(power.ShouldCardExhaust(attack));
    }

    // ===== NoDrawPower =====

    [Fact]
    public void NoDrawPower_BlocksNormalDraw()
    {
        var power = new NoDrawPower();
        Assert.False(power.ShouldDraw(false));
        Assert.True(power.ShouldDraw(true));
    }

    // ===== Clone =====

    [Fact]
    public void Power_Clone_IsIndependent()
    {
        var creature = new SimCreature { CurrentHp = 50, MaxHp = 50 };
        creature.ApplyPower(new StrengthPower { Amount = 3 });
        var clone = creature.Clone();
        creature.GetPower<StrengthPower>()!.Amount = 5;
        Assert.Equal(3, clone.GetPowerAmount<StrengthPower>());
    }

    // ===== TickDown =====

    [Fact]
    public void VulnerablePower_TicksDown()
    {
        var creature = new SimCreature { CurrentHp = 50, MaxHp = 50 };
        creature.ApplyPower(new VulnerablePower { Amount = 2 });
        creature.TickDownPowers();
        Assert.Equal(1, creature.GetPowerAmount<VulnerablePower>());
        creature.TickDownPowers();
        Assert.Null(creature.GetPower<VulnerablePower>());
    }

    // ===== DiscardHand =====

    [Fact]
    public void DiscardHand_All_MovesToDiscard()
    {
        var (engine, player, _) = CreateCombat();
        AddCards(engine.State, 3);
        engine.PlayerDrawCards(3);
        Assert.Equal(3, engine.State.Hand.Count);

        var effect = new DiscardHand { Amount = 0 };
        effect.Execute(engine, player, null);
        Assert.Equal(0, engine.State.Hand.Count);
        Assert.Equal(3, engine.State.DiscardPile.Count);
    }

    // ===== UpgradeFromHand =====

    [Fact]
    public void UpgradeFromHand_UpgradesCards()
    {
        var (engine, player, _) = CreateCombat();
        AddCards(engine.State, 2);
        engine.PlayerDrawCards(2);

        var effect = new UpgradeFromHand { Amount = 0 };
        effect.Execute(engine, player, null);

        Assert.True(engine.State.Hand.Cards.All(c => c.IsUpgraded));
    }

    // ===== DamageWithBlock =====

    [Fact]
    public void DamageWithBlock_UsesBlockAsBonus()
    {
        var (engine, player, enemy) = CreateCombat();
        player.GainBlock(10);

        decimal totalDamage = 5 + 10 * 2;
        var effect = new DamageWithBlock { BaseAmount = 5, Multiplier = 2 };
        effect.Execute(engine, player, enemy);

        Assert.Equal(60 - totalDamage, enemy.CurrentHp);
    }

    private static (SimEngine engine, SimCreature player, SimCreature enemy) CreateCombat()
    {
        var state = new SimState();
        var player = new SimCreature { Name = "Player", Side = CombatSide.Player, CurrentHp = 80, MaxHp = 80 };
        var enemy = new SimCreature { Name = "Enemy", Side = CombatSide.Enemy, CurrentHp = 60, MaxHp = 60 };
        state.Players.Add(player);
        state.Enemies.Add(enemy);
        return (new SimEngine(state), player, enemy);
    }

    private static void AddCards(SimState state, int count)
    {
        for (int i = 0; i < count; i++)
            state.DrawPile.Add(new SimCard
            {
                Id = $"C{i}", Name = $"Card{i}", Cost = 1,
                Type = CardType.Attack, Rarity = CardRarity.Basic,
                DefaultTargetType = TargetType.SingleEnemy
            });
    }
}
