using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Enemies;

// ===== Simple dummies / non-combat =====
public class BigDummyAi : SimpleEnemyAi
{
    public override string EnemyId => "BIG_DUMMY"; public override int MinHp => 9999; public override int MaxHp => 9999;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class ArchitectAi : SimpleEnemyAi
{
    public override string EnemyId => "ARCHITECT"; public override int MinHp => 9999; public override int MaxHp => 9999;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class OneHpMonsterAi : SimpleEnemyAi
{
    public override string EnemyId => "ONE_HP_MONSTER"; public override int MinHp => 1; public override int MaxHp => 1;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class TenHpMonsterAi : SimpleEnemyAi
{
    public override string EnemyId => "TEN_HP_MONSTER"; public override int MinHp => 10; public override int MaxHp => 10;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class DeprecatedMonsterAi : SimpleEnemyAi
{
    public override string EnemyId => "DEPRECATED_MONSTER"; public override int MinHp => 1; public override int MaxHp => 1;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class OstyAi : SimpleEnemyAi
{
    public override string EnemyId => "OSTY"; public override int MinHp => 1; public override int MaxHp => 1;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class ByrdpipAi : SimpleEnemyAi
{
    public override string EnemyId => "BYRDPIP"; public override int MinHp => 9999; public override int MaxHp => 9999;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class PaelsLegionAi : SimpleEnemyAi
{
    public override string EnemyId => "PAELS_LEGION"; public override int MinHp => 9999; public override int MaxHp => 9999;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class BattleFriendV1Ai : SimpleEnemyAi
{
    public override string EnemyId => "BATTLE_FRIEND_V1"; public override int MinHp => 75; public override int MaxHp => 75;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 3 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class BattleFriendV2Ai : SimpleEnemyAi
{
    public override string EnemyId => "BATTLE_FRIEND_V2"; public override int MinHp => 150; public override int MaxHp => 150;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 3 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class BattleFriendV3Ai : SimpleEnemyAi
{
    public override string EnemyId => "BATTLE_FRIEND_V3"; public override int MinHp => 300; public override int MaxHp => 300;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 3 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Hidden };
}
public class LeafSlimeSAi : SimpleEnemyAi
{
    public override string EnemyId => "LEAF_SLIME_S"; public override int MinHp => 8; public override int MaxHp => 12;
    private bool _nextIsAttack = true;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _nextIsAttack = !_nextIsAttack; return _nextIsAttack ? new() { Intent = IntentType.Attack, Damage = 3, Hits = 1 } : new() { Intent = IntentType.StatusCard, StatusCardId = "SLIMED", StatusCardCount = 1 }; }
}
public class TwigSlimeMAi : SimpleEnemyAi
{
    public override string EnemyId => "TWIG_SLIME_M"; public override int MinHp => 22; public override int MaxHp => 26;
    private int _cd; private bool _first = true;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (_cd > 0) _cd--; if (!_first && _cd == 0) { _first = false; _cd = 2; return new() { Intent = IntentType.Attack, Damage = 11, Hits = 1 }; } if (_first) _first = false; return new() { Intent = IntentType.StatusCard, StatusCardId = "SLIMED", StatusCardCount = 2 }; }
}
public class SingleAttackMoveMonsterAi : SimpleEnemyAi
{
    public override string EnemyId => "SINGLE_ATTACK_MOVE_MONSTER"; public override int MinHp => 20; public override int MaxHp => 30;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 7, Hits = 1 };
}
public class MultiAttackMoveMonsterAi : SimpleEnemyAi
{
    public override string EnemyId => "MULTI_ATTACK_MOVE_MONSTER"; public override int MinHp => 25; public override int MaxHp => 35;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.Attack, Damage = 4, Hits = 3 };
}
public class ExoskeletonAi : SimpleEnemyAi
{
    public override string EnemyId => "EXOSKELETON"; public override int MinHp => 40; public override int MaxHp => 46;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlatingPower { Amount = 6 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 2; return _p == 0 ? new() { Intent = IntentType.Defend, Block = 8 } : new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 }; }
}
public class LivingShieldAi : SimpleEnemyAi
{
    public override string EnemyId => "LIVING_SHIELD"; public override int MinHp => 60; public override int MaxHp => 68;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Defend, Block = 12 }, 1 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, DebuffAmount = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 } }; }
}
public class LivingFogAi : SimpleEnemyAi
{
    public override string EnemyId => "LIVING_FOG"; public override int MinHp => 45; public override int MaxHp => 52;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 1, DebuffAmount = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1 }, _ => new() { Intent = IntentType.Defend, Block = 8 } }; }
}
public class TunnelerAi : SimpleEnemyAi
{
    public override string EnemyId => "TUNNELER"; public override int MinHp => 48; public override int MaxHp => 54;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 1, DebuffAmount = 2 }, _ => new() { Intent = IntentType.Defend, Block = 10, BuffAmount = 3 } }; }
}
public class ToadpoleAi : SimpleEnemyAi
{
    public override string EnemyId => "TOADPOLE"; public override int MinHp => 30; public override int MaxHp => 36;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 3 } }; }
}
public class TurretOperatorAi : SimpleEnemyAi
{
    public override string EnemyId => "TURRET_OPERATOR"; public override int MinHp => 55; public override int MaxHp => 62;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Summon }, 1 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 2 }, _ => new() { Intent = IntentType.Defend, Block = 10 } }; }
}
public class GasBombAi : SimpleEnemyAi
{
    public override string EnemyId => "GAS_BOMB"; public override int MinHp => 7; public override int MaxHp => 8;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy) => new() { Intent = IntentType.DeathBlow, Damage = 8, Hits = 1 };
}
public class VantomAi : SimpleEnemyAi
{
    public override string EnemyId => "VANTOM"; public override int MinHp => 80; public override int MaxHp => 90;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 2; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, DebuffAmount = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 } }; }
}
public class SkulkingColonyAi : SimpleEnemyAi
{
    public override string EnemyId => "SKULKING_COLONY"; public override int MinHp => 80; public override int MaxHp => 88;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 4; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 2 }, 1 => new() { Intent = IntentType.Buff, BuffAmount = 3 }, 2 => new() { Intent = IntentType.Defend, Block = 12 }, _ => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 } }; }
}
public class ThievingHopperAi : SimpleEnemyAi
{
    public override string EnemyId => "THIEVING_HOPPER"; public override int MinHp => 79; public override int MaxHp => 84;
    private int _p;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 5 }); }
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 5; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 17, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 21, Hits = 1 }, 3 => new() { Intent = IntentType.Buff }, _ => new() { Intent = IntentType.Escape } }; }
}
public class EntomancerAi : SimpleEnemyAi
{
    public override string EnemyId => "ENTOMANCER"; public override int MinHp => 80; public override int MaxHp => 90;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Summon }, 1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2, DebuffAmount = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, BuffAmount = 3 } }; }
}
public class MagiKnightAi : SimpleEnemyAi
{
    public override string EnemyId => "MAGI_KNIGHT"; public override int MinHp => 80; public override int MaxHp => 88;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1, DebuffAmount = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 7, Hits = 2 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 3 } }; }
}
public class DevotedSculptorAi : SimpleEnemyAi
{
    public override string EnemyId => "DEVOTED_SCULPTOR"; public override int MinHp => 70; public override int MaxHp => 78;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }, 1 => new() { Intent = IntentType.Defend, Block = 10, BuffAmount = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2 } }; }
}
public class TheObscuraAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_OBSCURA"; public override int MinHp => 123; public override int MaxHp => 129;
    private bool _hasSummoned;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (!_hasSummoned) { _hasSummoned = true; return new() { Intent = IntentType.Summon }; } int r = state.Rng.Next(3); return r switch { 0 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 }, 1 => new() { Intent = IntentType.Buff, BuffAmount = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, Block = 6 } }; }
}

// ===== Elites =====
public class BygoneEffigyAi : SimpleEnemyAi
{
    public override string EnemyId => "BYGONE_EFFIGY"; public override int MinHp => 127; public override int MaxHp => 132;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p++; if (_p == 1) return new() { Intent = IntentType.Sleep }; if (_p == 2) return new() { Intent = IntentType.Buff, BuffAmount = 10 }; return new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 }; }
}
public class ByrdonisAi : SimpleEnemyAi
{
    public override string EnemyId => "BYRDONIS"; public override int MinHp => 81; public override int MaxHp => 84;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 1 }); }
    private bool _nextIsSwoop = true;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _nextIsSwoop = !_nextIsSwoop; return _nextIsSwoop ? new() { Intent = IntentType.Attack, Damage = 17, Hits = 1 } : new() { Intent = IntentType.Attack, Damage = 3, Hits = 3 }; }
}
public class MechaKnightAi : SimpleEnemyAi
{
    public override string EnemyId => "MECHA_KNIGHT"; public override int MinHp => 120; public override int MaxHp => 130;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 4; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, BuffAmount = 3 }, 1 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }, 2 => new() { Intent = IntentType.Defend, Block = 12 }, _ => new() { Intent = IntentType.Attack, Damage = 6, Hits = 3, DebuffAmount = 1 } }; }
}
public class FabricatorAi : SimpleEnemyAi
{
    public override string EnemyId => "FABRICATOR"; public override int MinHp => 150; public override int MaxHp => 155;
    private int _s;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (_s < 4) { if (_s % 2 == 0) { _s++; return new() { Intent = IntentType.Summon }; } _s++; return new() { Intent = IntentType.Attack, Damage = 18, Hits = 1 }; } return new() { Intent = IntentType.Attack, Damage = 11, Hits = 1 }; }
}
public class DecimillipedeSegmentAi : SimpleEnemyAi
{
    public override string EnemyId => "DECIMILLIPEDE_SEGMENT"; public override int MinHp => 40; public override int MaxHp => 46;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 25 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 5, Hits = 2 }, 1 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 1, DebuffAmount = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 6, Hits = 1, BuffAmount = 2 } }; }
}
public class DecimillipedeSegmentFrontAi : DecimillipedeSegmentAi { public override string EnemyId => "DECIMILLIPEDE_SEGMENT_FRONT"; }
public class DecimillipedeSegmentMiddleAi : DecimillipedeSegmentAi { public override string EnemyId => "DECIMILLIPEDE_SEGMENT_MIDDLE"; }
public class DecimillipedeSegmentBackAi : DecimillipedeSegmentAi { public override string EnemyId => "DECIMILLIPEDE_SEGMENT_BACK"; }

// ===== Bosses =====
public class LagavulinMatriarchAi : SimpleEnemyAi
{
    public override string EnemyId => "LAGAVULIN_MATRIARCH"; public override int MinHp => 222; public override int MaxHp => 233;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlatingPower { Amount = 12 }); }
    private bool _awake; private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (!_awake) return new() { Intent = IntentType.Sleep }; _p = (_p + 1) % 4; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 19, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 2 }, 2 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1, Block = 12 }, _ => new() { Intent = IntentType.Debuff, DebuffAmount = 1, BuffAmount = 2 } }; }
}
public class CeremonialBeastAi : SimpleEnemyAi
{
    public override string EnemyId => "CEREMONIAL_BEAST"; public override int MinHp => 252; public override int MaxHp => 262;
    private int _p; private bool _p2;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (!_p2) { _p++; if (_p >= 2) _p2 = true; return new() { Intent = IntentType.Buff, BuffAmount = 150 }; } _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 17, Hits = 1, BuffAmount = 3 }, _ => new() { Intent = IntentType.Debuff, DebuffAmount = 1 } }; }
}
public class WaterfallGiantAi : SimpleEnemyAi
{
    public override string EnemyId => "WATERFALL_GIANT"; public override int MinHp => 240; public override int MaxHp => 250;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 6; return _p switch { 0 => new() { Intent = IntentType.Buff, BuffAmount = 15 }, 1 => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 1 }, 3 => new() { Intent = IntentType.Heal, HealAmount = 10 }, 4 => new() { Intent = IntentType.Attack, Damage = 20, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 } }; }
}
public class QueenAi : SimpleEnemyAi
{
    public override string EnemyId => "QUEEN"; public override int MinHp => 400; public override int MaxHp => 419;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 5; return _p switch { 0 => new() { Intent = IntentType.CardDebuff, DebuffAmount = 3 }, 1 => new() { Intent = IntentType.Debuff, DebuffAmount = 99 }, 2 => new() { Intent = IntentType.Buff, BuffAmount = 1, Block = 20 }, 3 => new() { Intent = IntentType.Attack, Damage = 3, Hits = 5 }, _ => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 } }; }
}
public class KnowledgeDemonAi : SimpleEnemyAi
{
    public override string EnemyId => "KNOWLEDGE_DEMON"; public override int MinHp => 379; public override int MaxHp => 399;
    private int _p; private int _cc;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (_cc < 3 && _p % 4 == 0) { _cc++; _p++; return new() { Intent = IntentType.Debuff }; } _p = (_p + 1) % 4; return _p switch { 0 => new() { Intent = IntentType.Debuff }, 1 => new() { Intent = IntentType.Attack, Damage = 17, Hits = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 8, Hits = 3 }, _ => new() { Intent = IntentType.Attack, Damage = 11, Hits = 1, BuffAmount = 2, HealAmount = 30 } }; }
}
public class AeonglassAi : SimpleEnemyAi
{
    public override string EnemyId => "AEONGLASS"; public override int MinHp => 512; public override int MaxHp => 535;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new ArtifactPower { Amount = 3 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 26, Hits = 1, DebuffAmount = 3 }, 1 => new() { Intent = IntentType.Attack, Damage = 11, Hits = 2 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 3, Block = 33, StatusCardId = "WITHER", StatusCardCount = 1 } }; }
}
public class TestSubjectAi : SimpleEnemyAi
{
    public override string EnemyId => "TEST_SUBJECT"; public override int MinHp => 100; public override int MaxHp => 111;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new PlaceholderPower { Amount = 2 }); }
    private int _ph = 1; private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { if (!enemy.IsAlive && _ph == 1) { _ph = 2; return new() { Intent = IntentType.Heal, HealAmount = 200 }; } if (!enemy.IsAlive && _ph == 2) { _ph = 3; return new() { Intent = IntentType.Heal, HealAmount = 300 }; } if (_ph >= 3) { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 10, Hits = 3 }, 1 => new() { Intent = IntentType.Attack, Damage = 45, Hits = 1 }, _ => new() { Intent = IntentType.Buff, BuffAmount = 2, StatusCardId = "BURN", StatusCardCount = 3 } }; } _p = (_p + 1) % 2; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 20, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 14, Hits = 1, DebuffAmount = 1 } }; }
}
public class OwlMagistrateAi : SimpleEnemyAi
{
    public override string EnemyId => "OWL_MAGISTRATE"; public override int MinHp => 231; public override int MaxHp => 247;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 4; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 6 }, 2 => new() { Intent = IntentType.Buff }, _ => new() { Intent = IntentType.Attack, Damage = 33, Hits = 1, DebuffAmount = 4 } }; }
}
public class FakeMerchantMonsterAi : SimpleEnemyAi
{
    public override string EnemyId => "FAKE_MERCHANT_MONSTER"; public override int MinHp => 165; public override int MaxHp => 175;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 5; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 2, Hits = 8 }, 2 => new() { Intent = IntentType.Attack, Damage = 9, Hits = 1, DebuffAmount = 1 }, 3 => new() { Intent = IntentType.Buff, BuffAmount = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 } }; }
}

// ===== Kaiser Crab arms =====
public class CrusherAi : SimpleEnemyAi
{
    public override string EnemyId => "CRUSHER"; public override int MinHp => 209; public override int MaxHp => 219;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 5; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 4, Hits = 1 }, 2 => new() { Intent = IntentType.Attack, Damage = 6, Hits = 2, DebuffAmount = 2 }, 3 => new() { Intent = IntentType.Buff, BuffAmount = 2 }, _ => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1, Block = 18 } }; }
}
public class RocketAi : SimpleEnemyAi
{
    public override string EnemyId => "ROCKET"; public override int MinHp => 199; public override int MaxHp => 209;
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 5; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 3, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 18, Hits = 1 }, 2 => new() { Intent = IntentType.Buff, BuffAmount = 2 }, 3 => new() { Intent = IntentType.Attack, Damage = 31, Hits = 1 }, _ => new() { Intent = IntentType.Sleep } }; }
}

// ===== Adversary =====
public class TheAdversaryMkOneAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_ADVERSARY_MK_ONE"; public override int MinHp => 100; public override int MaxHp => 100;
    public override void OnCombatStart(SimCreature enemy) { }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 12, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 8, Hits = 2, BuffAmount = 2 } }; }
}
public class TheAdversaryMkTwoAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_ADVERSARY_MK_TWO"; public override int MinHp => 200; public override int MaxHp => 200;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new ArtifactPower { Amount = 1 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 13, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 16, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 9, Hits = 2, BuffAmount = 3 } }; }
}
public class TheAdversaryMkThreeAi : SimpleEnemyAi
{
    public override string EnemyId => "THE_ADVERSARY_MK_THREE"; public override int MinHp => 300; public override int MaxHp => 300;
    public override void OnCombatStart(SimCreature enemy) { enemy.ApplyPower(new ArtifactPower { Amount = 2 }); }
    private int _p;
    public override EnemyTurnPlan PlanNextTurn(SimState state, SimCreature enemy)
    { _p = (_p + 1) % 3; return _p switch { 0 => new() { Intent = IntentType.Attack, Damage = 15, Hits = 1 }, 1 => new() { Intent = IntentType.Attack, Damage = 18, Hits = 1 }, _ => new() { Intent = IntentType.Attack, Damage = 10, Hits = 2, BuffAmount = 4 } }; }
}
