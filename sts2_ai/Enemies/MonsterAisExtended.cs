using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Enemies;

public class AssassinRubyRaiderAi : SimpleEnemyAi
{
    public override string EnemyId => "ASSASSIN_RUBY_RAIDER";
    public override int MinHp => 18; public override int MaxHp => 23;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 };
}

public class AxeRubyRaiderAi : SimpleEnemyAi
{
    public override string EnemyId => "AXE_RUBY_RAIDER";
    public override int MinHp => 20; public override int MaxHp => 22;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch
        {
            0 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 1, Block = 5 },
            1 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 1, Block = 5 },
            _ => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }
        };
    }
}

public class BruteRubyRaiderAi : SimpleEnemyAi
{
    public override string EnemyId => "BRUTE_RUBY_RAIDER";
    public override int MinHp => 30; public override int MaxHp => 33;
    private bool _nextIsRoar = true;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsRoar = !_nextIsRoar;
        return _nextIsRoar ? new() { Intent = IntentType.Attack, Damage = 7, Hits = 1 } : new() { Intent = IntentType.Buff, BuffAmount = 3 };
    }
}

public class CrossbowRubyRaiderAi : SimpleEnemyAi
{
    public override string EnemyId => "CROSSBOW_RUBY_RAIDER";
    public override int MinHp => 18; public override int MaxHp => 21;
    private bool _nextIsFire = false;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsFire = !_nextIsFire;
        return _nextIsFire ? new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 } : new() { Intent = IntentType.Defend, Block = 3 };
    }
}

public class TrackerRubyRaiderAi : SimpleEnemyAi
{
    public override string EnemyId => "TRACKER_RUBY_RAIDER";
    public override int MinHp => 22; public override int MaxHp => 26;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 1, DebuffAmount = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 } };
    }
}

public class BowlbugEggAi : SimpleEnemyAi
{
    public override string EnemyId => "BOWLBUG_EGG";
    public override int MinHp => 21; public override int MaxHp => 22;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1, Block = 7 };
}

public class BowlbugNectarAi : SimpleEnemyAi
{
    public override string EnemyId => "BOWLBUG_NECTAR";
    public override int MinHp => 35; public override int MaxHp => 38;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (_p < 2) { _p++; return new() { Intent = IntentType.Attack, Damage = 3, Hits = 1 }; }
        if (_p == 2) { _p++; return new() { Intent = IntentType.Buff, BuffAmount = 15 }; }
        return new() { Intent = IntentType.Attack, Damage = 3, Hits = 1 };
    }
}

public class BowlbugRockAi : SimpleEnemyAi
{
    public override string EnemyId => "BOWLBUG_ROCK";
    public override int MinHp => 45; public override int MaxHp => 48;
    private bool _offBalance;
    public override void OnCombatStart(SimCreature enemy) { _offBalance = true; }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (!_offBalance) { _offBalance = true; return new() { Intent = IntentType.Stun }; }
        _offBalance = false; return new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 };
    }
}

public class BowlbugSilkAi : SimpleEnemyAi
{
    public override string EnemyId => "BOWLBUG_SILK";
    public override int MinHp => 40; public override int MaxHp => 43;
    private bool _nextIsSpit = true;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsSpit = !_nextIsSpit;
        return _nextIsSpit ? new() { Intent = IntentType.Debuff, DebuffAmount = 1 } : new() { Intent = IntentType.Attack, Damage = 4, Hits = 2 };
    }
}

public class CalcifiedCultistAi : SimpleEnemyAi
{
    public override string EnemyId => "CALCIFIED_CULTIST";
    public override int MinHp => 38; public override int MaxHp => 41;
    private bool _hasIncanted;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (!_hasIncanted) { _hasIncanted = true; return new() { Intent = IntentType.Buff, BuffAmount = 2 }; }
        return new() { Intent = IntentType.Attack, Damage = 9, Hits = 1 };
    }
}

public class DampCultistAi : SimpleEnemyAi
{
    public override string EnemyId => "DAMP_CULTIST";
    public override int MinHp => 35; public override int MaxHp => 38;
    private bool _hasIncanted;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (!_hasIncanted) { _hasIncanted = true; return new() { Intent = IntentType.Buff, BuffAmount = 3 }; }
        return new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 };
    }
}

public class GuardbotAi : SimpleEnemyAi
{
    public override string EnemyId => "GUARDBOT";
    public override int MinHp => 16; public override int MaxHp => 20;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Defend, Block = 15 };
}

public class NoisebotAi : SimpleEnemyAi
{
    public override string EnemyId => "NOISEBOT";
    public override int MinHp => 18; public override int MaxHp => 23;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.StatusCard, StatusCardId = "DAZED", StatusCardCount = 2 };
}

public class StabbotAi : SimpleEnemyAi
{
    public override string EnemyId => "STABBOT";
    public override int MinHp => 18; public override int MaxHp => 23;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 11, Hits = 1, DebuffAmount = 1 };
}

public class ParafrightAi : SimpleEnemyAi
{
    public override string EnemyId => "PARAFRIGHT";
    public override int MinHp => 21; public override int MaxHp => 21;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1 };
}

public class PunchConstructAi : SimpleEnemyAi
{
    public override string EnemyId => "PUNCH_CONSTRUCT";
    public override int MinHp => 55; public override int MaxHp => 60;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new ArtifactPower { Amount = 1 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Defend, Block = 10 }, 1 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 2, DebuffAmount = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 } };
    }
}

public class MyteAi : SimpleEnemyAi
{
    public override string EnemyId => "MYTE";
    public override int MinHp => 61; public override int MaxHp => 67;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.StatusCard, StatusCardId = "TOXIC", StatusCardCount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 4, Hits = 1, BuffAmount = 2 } };
    }
}

public class SewerClamAi : SimpleEnemyAi
{
    public override string EnemyId => "SEWER_CLAM";
    public override int MinHp => 56; public override int MaxHp => 58;
    private bool _nextIsJet = true;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 8 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsJet = !_nextIsJet;
        return _nextIsJet ? new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 } : new() { Intent = IntentType.Buff, BuffAmount = 4 };
    }
}

public class ShrinkerBeetleAi : SimpleEnemyAi
{
    public override string EnemyId => "SHRINKER_BEETLE";
    public override int MinHp => 38; public override int MaxHp => 40;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.DebuffStrong, DebuffAmount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 } };
    }
}

public class SpinyToadAi : SimpleEnemyAi
{
    public override string EnemyId => "SPINY_TOAD";
    public override int MinHp => 116; public override int MaxHp => 119;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Buff }, 1 => new() { Intent = IntentType.Attack, Damage = 23, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 17, Hits = 1 } };
    }
}

public class AxebotAi : SimpleEnemyAi
{
    public override string EnemyId => "AXEBOT";
    public override int MinHp => 70; public override int MaxHp => 78;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 2 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1, DebuffAmount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 2 }, _ => new() { Intent = IntentType.Defend, Block = 10, BuffAmount = 3 } };
    }
}

public class CubexConstructAi : SimpleEnemyAi
{
    public override string EnemyId => "CUBEX_CONSTRUCT";
    public override int MinHp => 65; public override int MaxHp => 70;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new ArtifactPower { Amount = 1 }); enemy.GainBlock(13); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.Buff, BuffAmount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1, BuffAmount = 2 }, 2 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1, BuffAmount = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 5, Hits = 2 } };
    }
}

public class FogmogAi : SimpleEnemyAi
{
    public override string EnemyId => "FOGMOG";
    public override int MinHp => 74; public override int MaxHp => 78;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch
        {
            0 => new() { Intent = IntentType.Summon },
            1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, BuffAmount = 1 },
            _ => state.Rng.Next(2) == 0 ? new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, BuffAmount = 1 } : new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 }
        };
    }
}

public class HauntedShipAi : SimpleEnemyAi
{
    public override string EnemyId => "HAUNTED_SHIP";
    public override int MinHp => 63; public override int MaxHp => 67;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 3 }, _ => new() { Intent = IntentType.Debuff, DebuffAmount = 3, StatusCardId = "DAZED", StatusCardCount = 5 } };
    }
}

public class MawlerAi : SimpleEnemyAi
{
    public override string EnemyId => "MAWLER";
    public override int MinHp => 80; public override int MaxHp => 86;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 9, Hits = 1, DebuffAmount = 2 } };
    }
}

public class FossilStalkerAi : SimpleEnemyAi
{
    public override string EnemyId => "FOSSIL_STALKER";
    public override int MinHp => 51; public override int MaxHp => 53;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 3 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 1, DebuffAmount = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 3, Hits = 2 } };
    }
}

public class SeapunkAi : SimpleEnemyAi
{
    public override string EnemyId => "SEAPUNK";
    public override int MinHp => 44; public override int MaxHp => 46;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 11, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 2, Hits = 4 }, _ => new() { Intent = IntentType.Defend, Block = 7, BuffAmount = 1 } };
    }
}

public class EyeWithTeethAi : SimpleEnemyAi
{
    public override string EnemyId => "EYE_WITH_TEETH";
    public override int MinHp => 15; public override int MaxHp => 20;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1 };
}

public class ScrollOfBitingAi : SimpleEnemyAi
{
    public override string EnemyId => "SCROLL_OF_BITING";
    public override int MinHp => 30; public override int MaxHp => 37;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 2 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 2 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 2 } };
    }
}

public class InkletAi : SimpleEnemyAi
{
    public override string EnemyId => "INKLET";
    public override int MinHp => 11; public override int MaxHp => 17;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        int r = state.Rng.Next(3);
        return r switch { 0 => new() { Intent = IntentType.Attack, Damage = 3, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 2, Hits = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 } };
    }
}

public class GlobeHeadAi : SimpleEnemyAi
{
    public override string EnemyId => "GLOBE_HEAD";
    public override int MinHp => 148; public override int MaxHp => 158;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 6 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1, DebuffAmount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1, BuffAmount = 2 } };
    }
}

public class TwoTailedRatAi : SimpleEnemyAi
{
    public override string EnemyId => "TWO_TAILED_RAT";
    public override int MinHp => 50; public override int MaxHp => 56;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1, BuffAmount = 2 }, 2 => new() { Intent = IntentType.Defend, Block = 10 }, _ => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, DebuffAmount = 1 } };
    }
}

public class SlitheringStranglerAi : SimpleEnemyAi
{
    public override string EnemyId => "SLITHERING_STRANGLER";
    public override int MinHp => 55; public override int MaxHp => 62;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, DebuffAmount = 2 } };
    }
}

public class SludgeSpinnerAi : SimpleEnemyAi
{
    public override string EnemyId => "SLUDGE_SPINNER";
    public override int MinHp => 48; public override int MaxHp => 54;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1, DebuffAmount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 2 }, _ => new() { Intent = IntentType.Defend, Block = 8, BuffAmount = 3 } };
    }
}

public class SlimedBerserkerAi : SimpleEnemyAi
{
    public override string EnemyId => "SLIMED_BERSERKER";
    public override int MinHp => 60; public override int MaxHp => 68;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.StatusCard, StatusCardId = "SLIMED", StatusCardCount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 2 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 3 } };
    }
}

public class SlumberingBeetleAi : SimpleEnemyAi
{
    public override string EnemyId => "SLUMBERING_BEETLE";
    public override int MinHp => 40; public override int MaxHp => 46;
    private bool _nextIsBite = false;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsBite = !_nextIsBite;
        return _nextIsBite ? new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 } : new() { Intent = IntentType.Defend, Block = 10, BuffAmount = 4 };
    }
}

public class TheLostAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_LOST";
    public override int MinHp => 93; public override int MaxHp => 99;
    private bool _nextIsLaser = true;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsLaser = !_nextIsLaser;
        return _nextIsLaser ? new() { Intent = IntentType.Attack, Damage = 4, Hits = 2 } : new() { Intent = IntentType.Debuff, DebuffAmount = 2 };
    }
}

public class TheForgottenAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_FORGOTTEN";
    public override int MinHp => 106; public override int MaxHp => 111;
    private bool _nextIsDread = true;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsDread = !_nextIsDread;
        return _nextIsDread ? new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 } : new() { Intent = IntentType.Debuff, DebuffAmount = 2, Block = 8 };
    }
}

public class VineShamblerAi : SimpleEnemyAi
{
    public override string EnemyId => "VINE_SHAMBLER";
    public override int MinHp => 45; public override int MaxHp => 52;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2 }, 1 => new() { Intent = IntentType.Defend, Block = 8, BuffAmount = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, DebuffAmount = 1 } };
    }
}

public class OvicopterAi : SimpleEnemyAi
{
    public override string EnemyId => "OVICOPTER";
    public override int MinHp => 124; public override int MaxHp => 130;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.Summon }, 1 => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1, DebuffAmount = 2 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 3 } };
    }
}

public class ToughEggAi : SimpleEnemyAi
{
    public override string EnemyId => "TOUGH_EGG";
    public override int MinHp => 18; public override int MaxHp => 24;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1 };
}

public class PhrogParasiteAi : SimpleEnemyAi
{
    public override string EnemyId => "PHROG_PARASITE";
    public override int MinHp => 61; public override int MaxHp => 64;
    private bool _nextIsLash = true;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 4 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _nextIsLash = !_nextIsLash;
        return _nextIsLash ? new() { Intent = IntentType.Attack, Damage = 4, Hits = 4 } : new() { Intent = IntentType.StatusCard, StatusCardId = "INFECTION", StatusCardCount = 3 };
    }
}

public class TheInsatiableAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_INSATIABLE";
    public override int MinHp => 321; public override int MaxHp => 341;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 5;
        return _p switch
        {
            0 => new() { Intent = IntentType.Buff, BuffAmount = 2, StatusCardId = "FRANTIC_ESCAPE", StatusCardCount = 6 },
            1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2 },
            2 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2 },
            3 => new() { Intent = IntentType.Attack, Damage = 28, Hits = 1 },
            _ => new() { Intent = IntentType.Buff, BuffAmount = 2 }
        };
    }
}

public class SoulFyshAi : SimpleEnemyAi
{
    public override string EnemyId => "SOUL_FYSH";
    public override int MinHp => 211; public override int MaxHp => 221;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 5;
        return _p switch
        {
            0 => new() { Intent = IntentType.StatusCard, StatusCardId = "BECKON", StatusCardCount = 2 },
            1 => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1 },
            2 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1, StatusCardId = "BECKON", StatusCardCount = 1 },
            3 => new() { Intent = IntentType.Buff },
            _ => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1, DebuffAmount = 3 }
        };
    }
}

public class TerrorEelAi : SimpleEnemyAi
{
    public override string EnemyId => "TERROR_EEL";
    public override int MinHp => 140; public override int MaxHp => 150;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 70 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 3, Hits = 3, BuffAmount = 6 }, 2 => new() { Intent = IntentType.Stun }, _ => new() { Intent = IntentType.Debuff, DebuffAmount = 99 } };
    }
}

public class SoulNexusAi : SimpleEnemyAi
{
    public override string EnemyId => "SOUL_NEXUS";
    public override int MinHp => 234; public override int MaxHp => 254;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        int r = state.Rng.Next(3);
        return r switch { 0 => new() { Intent = IntentType.Attack, Damage = 29, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 4 }, _ => new() { Intent = IntentType.Attack, Damage = 18, Hits = 1, DebuffAmount = 2 } };
    }
}

public class InfestedPrismAi : SimpleEnemyAi
{
    public override string EnemyId => "INFESTED_PRISM";
    public override int MinHp => 161; public override int MaxHp => 171;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 2 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 11, Hits = 1, Block = 11 }, 2 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, BuffAmount = 2, Block = 20 } };
    }
}

public class FlailKnightAi : SimpleEnemyAi
{
    public override string EnemyId => "FLAIL_KNIGHT";
    public override int MinHp => 101; public override int MaxHp => 108;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Buff, BuffAmount = 3 }, 1 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 } };
    }
}

public class KinFollowerAi : SimpleEnemyAi
{
    public override string EnemyId => "KIN_FOLLOWER";
    public override int MinHp => 58; public override int MaxHp => 59;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 2, Hits = 2 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 2 } };
    }
}

public class KinPriestAi : SimpleEnemyAi
{
    public override string EnemyId => "KIN_PRIEST";
    public override int MinHp => 190; public override int MaxHp => 199;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 4;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, DebuffAmount = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, DebuffAmount = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 3, Hits = 3 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 2 } };
    }
}

public class HunterKillerAi : SimpleEnemyAi
{
    public override string EnemyId => "HUNTER_KILLER";
    public override int MinHp => 121; public override int MaxHp => 126;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Debuff }, 1 => new() { Intent = IntentType.Attack, Damage = 17, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 7, Hits = 3 } };
    }
}

public class MysteriousKnightAi : FlailKnightAi
{
    public override string EnemyId => "MYSTERIOUS_KNIGHT";
    public override int MinHp => 101; public override int MaxHp => 108;
    public override void OnCombatStart(SimCreature enemy)
    {
        enemy.ApplyPower(new StrengthPower { Amount = 6 });
        enemy.ApplyPower(new PlatingPower { Amount = 6 });
    }
}

public class FrogKnightAi : SimpleEnemyAi
{
    public override string EnemyId => "FROG_KNIGHT";
    public override int MinHp => 191; public override int MaxHp => 199;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlatingPower { Amount = 15 }); }
    private int _p;
    private bool _hasBeetleCharged;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    {
        if (enemy.CurrentHp < enemy.MaxHp / 2 && !_hasBeetleCharged)
        {
            _hasBeetleCharged = true;
            return new() { Intent = IntentType.Attack, Damage = 35, Hits = 1 };
        }
        _p = (_p + 1) % 3;
        return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1, DebuffAmount = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 21, Hits = 1 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 5 } };
    }
}
