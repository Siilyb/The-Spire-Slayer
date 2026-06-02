using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Enemies;

/// Simple fixed-cycle or alternating pattern monster AI
public abstract class SimpleEnemyAi : IEnemyAi
{
    public abstract string EnemyId { get; }
    public abstract int MinHp { get; }
    public abstract int MaxHp { get; }

    public virtual void OnCombatStart(SimCreature enemy) { }
    public abstract EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy);
}

// ===== 固定循环（始终同一招） =====
public class ZapbotAi : SimpleEnemyAi
{
    public override string EnemyId => "ZAPBOT";
    public override int MinHp => 18;
    public override int MaxHp => 23;

    public override void OnCombatStart(SimCreature enemy)
    {
        enemy.ApplyPower(new PlaceholderPower { Amount = 2 });
    }

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
        => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 };
}

public class ChomperAi : SimpleEnemyAi
{
    public override string EnemyId => "CHOMPER";
    public override int MinHp => 60;
    public override int MaxHp => 64;
    private bool _nextIsClamp = true;

    public override void OnCombatStart(SimCreature enemy)
    {
        enemy.ApplyPower(new ArtifactPower { Amount = 2 });
    }

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsClamp = !_nextIsClamp;
        if (_nextIsClamp)
            return new() { Intent = IntentType.Attack, Damage = 8, Hits = 2 };
        return new() { Intent = IntentType.StatusCard, StatusCardId = "DAZED", StatusCardCount = 3 };
    }
}

public class LeafSlimeMAi : SimpleEnemyAi
{
    public override string EnemyId => "LEAF_SLIME_M";
    public override int MinHp => 32;
    public override int MaxHp => 35;
    private bool _nextIsClump = false; // starts with StickyShot

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsClump = !_nextIsClump;
        if (_nextIsClump)
            return new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 };
        return new() { Intent = IntentType.StatusCard, StatusCardId = "SLIMED", StatusCardCount = 2 };
    }
}

public class TwigSlimeSAi : SimpleEnemyAi
{
    public override string EnemyId => "TWIG_SLIME_S";
    public override int MinHp => 10;
    public override int MaxHp => 14;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
        => new() { Intent = IntentType.Attack, Damage = 4, Hits = 1 };
}

public class CorpseSlugAi : SimpleEnemyAi
{
    public override string EnemyId => "CORPSE_SLUG";
    public override int MinHp => 30;
    public override int MaxHp => 36;
    private int _phase;

    public override void OnCombatStart(SimCreature enemy)
    {
        enemy.ApplyPower(new StrengthPower { Amount = 4 });
    }

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _phase = (_phase + 1) % 3;
        return _phase switch
        {
            0 => new() { Intent = IntentType.Attack, Damage = 3, Hits = 2 },
            1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 },
            _ => new() { Intent = IntentType.Debuff, DebuffAmount = 2 }
        };
    }
}

public class GremlinMercAi : SimpleEnemyAi
{
    public override string EnemyId => "GREMLIN_MERC";
    public override int MinHp => 20;
    public override int MaxHp => 26;
    private int _phase;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _phase = (_phase + 1) % 3;
        return _phase switch
        {
            0 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 2 },
            1 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 2, DebuffAmount = 1 },
            _ => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, BuffAmount = 2 }
        };
    }
}

public class SneakyGremlinAi : SimpleEnemyAi
{
    public override string EnemyId => "SNEAKY_GREMLIN";
    public override int MinHp => 16;
    public override int MaxHp => 22;
    private bool _nextIsBig;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsBig = !_nextIsBig;
        return new() { Intent = IntentType.Attack, Damage = _nextIsBig ? 7 : 4, Hits = 1 };
    }
}

public class FatGremlinAi : SimpleEnemyAi
{
    public override string EnemyId => "FAT_GREMLIN";
    public override int MinHp => 20;
    public override int MaxHp => 26;
    private bool _hasStunned;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (!_hasStunned) { _hasStunned = true; return new() { Intent = IntentType.Stun }; }
        return new() { Intent = IntentType.Escape };
    }
}

public class NibbitAi : SimpleEnemyAi
{
    public override string EnemyId => "NIBBIT";
    public override int MinHp => 24;
    public override int MaxHp => 30;
    private int _phase;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _phase = (_phase + 1) % 3;
        return _phase switch
        {
            0 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 },
            1 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, Block = 5 },
            _ => new() { Intent = IntentType.Buff, BuffAmount = 2 }
        };
    }
}

public class FlyconidAi : SimpleEnemyAi
{
    public override string EnemyId => "FLYCONID";
    public override int MinHp => 26;
    public override int MaxHp => 32;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        int r = state.Rng.Next(3);
        return r switch
        {
            0 => new() { Intent = IntentType.Debuff, DebuffAmount = 2 },
            1 => new() { Intent = IntentType.Debuff, DebuffAmount = 2, Block = 8 },
            _ => new() { Intent = IntentType.Attack, Damage = 11, Hits = 1 }
        };
    }
}

public class SnappingJaxfruitAi : SimpleEnemyAi
{
    public override string EnemyId => "SNAPPING_JAXFRUIT";
    public override int MinHp => 30;
    public override int MaxHp => 36;
    private int _strGained;

    public override void OnCombatStart(SimCreature enemy)
    {
        _strGained = 0;
    }

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _strGained += 2;
        return new() { Intent = IntentType.Attack, Damage = 3 + _strGained, Hits = 1, BuffAmount = 2 };
    }
}

public class LouseProgenitorAi : SimpleEnemyAi
{
    public override string EnemyId => "LOUSE_PROGENITOR";
    public override int MinHp => 40;
    public override int MaxHp => 48;
    private int _phase;

    public override void OnCombatStart(SimCreature enemy)
    {
        enemy.ApplyPower(new BufferPower { Amount = 14 });
    }

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _phase = (_phase + 1) % 3;
        return _phase switch
        {
            0 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 1, DebuffAmount = 2 },
            1 => new() { Intent = IntentType.Defend, Block = 14, BuffAmount = 5 },
            _ => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 }
        };
    }
}

public class FuzzyWurmCrawlerAi : SimpleEnemyAi
{
    public override string EnemyId => "FUZZY_WURM_CRAWLER";
    public override int MinHp => 28;
    public override int MaxHp => 34;
    private bool _nextIsAttack = true;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsAttack = !_nextIsAttack;
        if (_nextIsAttack)
            return new() { Intent = IntentType.Attack, Damage = 4, Hits = 1 };
        return new() { Intent = IntentType.Buff, BuffAmount = 7 };
    }
}

public class WrigglerAi : SimpleEnemyAi
{
    public override string EnemyId => "WRIGGLER";
    public override int MinHp => 32;
    public override int MaxHp => 38;
    private bool _nextIsBite = true;

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsBite = !_nextIsBite;
        if (_nextIsBite)
            return new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 };
        return new() { Intent = IntentType.Buff, BuffAmount = 1, StatusCardCount = 1, StatusCardId = "DAZED" };
    }
}

public class SpectralKnightAi : SimpleEnemyAi
{
    public override string EnemyId => "SPECTRAL_KNIGHT";
    public override int MinHp => 40;
    public override int MaxHp => 48;
    private int _cooldown;

    public override void OnCombatStart(SimCreature enemy) { _cooldown = 0; }

    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (_cooldown == 0)
        {
            _cooldown = 2;
            return new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 };
        }
        _cooldown--;
        return new() { Intent = IntentType.Attack, Damage = 7, Hits = 3 };
    }
}
