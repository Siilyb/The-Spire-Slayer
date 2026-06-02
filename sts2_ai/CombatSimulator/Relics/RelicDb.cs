using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.CombatSimulator;

public static class RelicDb
{
    public static Dictionary<string, Func<SimRelic>> All { get; } = new();

    public static void RegisterAll()
    {
        // BurningBlood: heal 6 after each combat (implemented outside combat)
        Register("BURNING_BLOOD", () => new SimpleRelic("BURNING_BLOOD", "燃烧之血"));

        // StrikeDummy: +3 damage with Strike cards
        Register("STRIKE_DUMMY", () => new DamageBuffRelic("STRIKE_DUMMY", "打击木桩", 3,
            (c, _) => c.Tags.Contains("Strike")));

        // OddSmoothStone: +1 dexterity
        Register("ODD_SMOOTH_STONE", () => new StatRelic("ODD_SMOOTH_STONE", "滑石", "Dexterity", 1));

        // Vajra: +1 strength
        Register("VAJRA", () => new StatRelic("VAJRA", "金刚杵", "Strength", 1));

        // Du-Vu Doll: +1 strength per curse
        Register("DU_VU_DOLL", () => new StatRelic("DU_VU_DOLL", "杜瓦娃娃", "Strength", 1));

        // Girya: +1 strength (upgradeable at rest)
        Register("GIRYA", () => new StatRelic("GIRYA", "杠铃", "Strength", 1));

        // Red Skull: +3 strength when HP <= 50%
        Register("RED_SKULL", () => new ConditionalStatRelic("RED_SKULL", "红骷髅", "Strength", 3,
            (engine) => engine.State.Players[0].CurrentHp <= engine.State.Players[0].MaxHp / 2));

        // Bag of Preparation: draw 2 extra cards on first turn
        Register("BAG_OF_PREPARATION", () => new DrawRelic("BAG_OF_PREPARATION", "备战袋", 2));

        // Ancient Tea Set: first 2 energy each combat
        Register("ANCIENT_TEA_SET", () => new EnergyRelic("ANCIENT_TEA_SET", "古茶具", 2));

        // Happy Flower: every 3 turns, +1 energy
        Register("HAPPY_FLOWER", () => new HappyFlowerRelic());

        // Orichalcum: if no block at end of turn, gain 6 block
        Register("ORICHALCUM", () => new OrichalcumRelic());

        // Thread and Needle: +4 plating at combat start
        Register("THREAD_AND_NEEDLE", () => new StartBuffRelic("THREAD_AND_NEEDLE", "针与线", () => new PlatingPower(), 4));

        // Tungsten Rod: reduce all HP loss by 1
        Register("TUNGSTEN_ROD", () => new SimpleRelic("TUNGSTEN_ROD", "钨棒"));
    }

    public static void Register(string id, Func<SimRelic> factory) => All[id] = factory;
}

// ===== Concrete relic types =====
public class SimpleRelic : SimRelic
{
    public SimpleRelic(string id, string name) { Id = id; Name = name; }
}

public class StatRelic : SimRelic
{
    public string StatType { get; }
    public int Amount { get; }
    private bool _applied;

    public StatRelic(string id, string name, string statType, int amount) : base()
    {
        Id = id; Name = name; StatType = statType; Amount = amount;
    }

    public override void OnCombatStart(SimEngine engine)
    {
        if (_applied) return;
        _applied = true;
        var p = engine.State.Players[0];
        if (StatType == "Strength")
        {
            var s = p.GetPower<StrengthPower>();
            if (s != null) s.Amount += Amount;
            else p.ApplyPower(new StrengthPower { Amount = Amount });
        }
        else if (StatType == "Dexterity")
        {
            var d = p.GetPower<DexterityPower>();
            if (d != null) d.Amount += Amount;
            else p.ApplyPower(new DexterityPower { Amount = Amount });
        }
    }
}

public class ConditionalStatRelic : StatRelic
{
    private Func<SimEngine, bool> _condition;

    public ConditionalStatRelic(string id, string name, string statType, int amount, Func<SimEngine, bool> condition)
        : base(id, name, statType, amount) { _condition = condition; }

    public override void OnCombatStart(SimEngine engine)
    {
        if (_condition(engine)) base.OnCombatStart(engine);
    }
}

public class DamageBuffRelic : SimRelic
{
    public int Amount { get; }
    private Func<SimCard, SimEngine, bool> _filter;

    public DamageBuffRelic(string id, string name, int amount, Func<SimCard, SimEngine, bool> filter)
    { Id = id; Name = name; Amount = amount; _filter = filter; }
}

public class DrawRelic : SimRelic
{
    public int Amount { get; }
    private bool _used;

    public DrawRelic(string id, string name, int amount) { Id = id; Name = name; Amount = amount; }

    public override void OnCombatStart(SimEngine engine)
    {
        if (!_used) { _used = true; engine.PlayerDrawCards(Amount); }
    }
}

public class EnergyRelic : SimRelic
{
    public int Amount { get; }
    private bool _used;

    public EnergyRelic(string id, string name, int amount) { Id = id; Name = name; Amount = amount; }

    public override void OnCombatStart(SimEngine engine)
    {
        if (!_used) { _used = true; engine.State.MaxEnergy += Amount; }
    }
}

public class HappyFlowerRelic : SimRelic
{
    private int _counter;

    public HappyFlowerRelic() { Id = "HAPPY_FLOWER"; Name = "欢乐之花"; }

    public override void OnCombatStart(SimEngine engine) { _counter = 0; }
}

public class OrichalcumRelic : SimRelic
{
    public OrichalcumRelic() { Id = "ORICHALCUM"; Name = "山铜"; }
}

public class StartBuffRelic : SimRelic
{
    private Func<SimPower> _powerFactory;
    public int Amount { get; }
    private bool _applied;

    public StartBuffRelic(string id, string name, Func<SimPower> factory, int amount)
    { Id = id; Name = name; _powerFactory = factory; Amount = amount; }

    public override void OnCombatStart(SimEngine engine)
    {
        if (!_applied) { _applied = true; var p = _powerFactory(); p.Amount = Amount; engine.State.Players[0].ApplyPower(p); }
    }
}
