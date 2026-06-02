using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Enemies;
using Sts2Ai.Mcts;

EnemyDb.RegisterAll();

int step = 0;
int lastTurn = 0;
var sep = new string('=', 60);

Console.WriteLine("=== STS2 AI 战斗模拟器 ===\n");

var state = new SimState();
state.Players.Add(new SimCreature { Name = "铁血战士", Side = CombatSide.Player, CurrentHp = 80, MaxHp = 80 });
state.Enemies.Add(new SimCreature { Name = "CHOMPER", Side = CombatSide.Enemy, CurrentHp = 64, MaxHp = 64 });

for (int i = 0; i < 5; i++)
{
    state.DrawPile.Add(new SimCard
    {
        Id = "STRIKE_IRONCLAD", Name = "打击", Cost = 1,
        Type = CardType.Attack, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.SingleEnemy,
        Tags = new HashSet<string> { "Strike" },
        Effects = new List<IEffect> { new DealDamage { BaseAmount = 6, Props = ValueProp.Move } }
    });
    state.DrawPile.Add(new SimCard
    {
        Id = "DEFEND_IRONCLAD", Name = "防御", Cost = 1,
        Type = CardType.Skill, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.Self,
        Tags = new HashSet<string> { "Defend" },
        Effects = new List<IEffect> { new GainBlock { BaseAmount = 5, Props = ValueProp.Move } }
    });
}
state.DrawPile.Add(new SimCard
{
    Id = "BASH", Name = "痛击", Cost = 2,
    Type = CardType.Attack, Rarity = CardRarity.Basic, DefaultTargetType = TargetType.SingleEnemy,
    Effects = new List<IEffect>
    {
        new DealDamage { BaseAmount = 8, Props = ValueProp.Move },
        new ApplyPower { PowerFactory = () => new VulnerablePower(), Amount = 2, TargetType = TargetType.SingleEnemy }
    }
});

state.DrawPile.Shuffle(state.Rng);

string CardStr(SimCard c) => c.HasKeyword("Ethereal") ? $"[-]{c.Name}*" : $"[{c.Cost}]{c.Name}";

void Display(SimState s, string? msg)
{
    step++;
    Console.WriteLine(sep);
    if (msg != null) Console.WriteLine($"  [{step}] {msg}");
    if (s.TurnNumber != lastTurn)
    {
        Console.WriteLine($"  ═══ 第 {s.TurnNumber} 回合 {(s.CurrentSide == CombatSide.Player ? "开始" : "结束")} ═══");
        lastTurn = s.TurnNumber;
    }
    Console.WriteLine(sep);

    var p = s.Players[0];
    var e = s.Enemies[0];
    var ai = e.CustomState.TryGetValue("__ai", out var a) ? a as IEnemyAi : null;
    var plan = ai?.PlanNextTurn(s, e);

    Console.WriteLine($"  玩家 HP:{p.CurrentHp}/{p.MaxHp}  Block:{p.Block}  能量:{s.Energy}/{s.MaxEnergy}");
    if (p.Powers.Any()) Console.WriteLine($"  Buff: {string.Join(", ", p.Powers.Select(pw => $"{pw.GetType().Name}({pw.Amount})"))}");
    Console.WriteLine($"  敌人 HP:{e.CurrentHp}/{e.MaxHp}  Block:{e.Block}  意图:{plan?.Intent}");
    if (e.Powers.Any()) Console.WriteLine($"  Buff: {string.Join(", ", e.Powers.Select(pw => $"{pw.GetType().Name}({pw.Amount})"))}");

    Console.WriteLine($"\n  手牌({s.Hand.Count}):");
    foreach (var c in s.Hand.Cards)
        Console.WriteLine($"    {CardStr(c)}");

    Console.WriteLine($"\n  抽牌堆({s.DrawPile.Count}):");
    foreach (var c in s.DrawPile.Cards)
        Console.WriteLine($"    {CardStr(c)}");

    Console.WriteLine($"\n  弃牌堆({s.DiscardPile.Count}):");
    foreach (var c in s.DiscardPile.Cards.TakeLast(8))
        Console.WriteLine($"    {CardStr(c)}");
    if (s.DiscardPile.Count > 8) Console.WriteLine($"    ... 还有 {s.DiscardPile.Count - 8} 张");

    if (s.ExhaustPile.Count > 0)
    {
        Console.WriteLine($"\n  消耗堆({s.ExhaustPile.Count}):");
        foreach (var c in s.ExhaustPile.Cards.TakeLast(8))
            Console.WriteLine($"    {CardStr(c)}");
        if (s.ExhaustPile.Count > 8) Console.WriteLine($"    ... 还有 {s.ExhaustPile.Count - 8} 张");
    }

    Console.WriteLine();
}

Display(state, "战斗开始");

var engine = new SimEngine(state);
var result = engine.RunFullCombat(maxTurns: 15, mctsAi: new CombatAi(), onCardPlayed: (s, m) => Display(s, m));

Display(state, $"战斗结束! {(result.PlayersAlive ? "玩家胜利" : "玩家失败")}");
Console.WriteLine($"共 {result.TurnNumber} 回合  玩家剩余HP: {state.Players[0].CurrentHp}/{state.Players[0].MaxHp}");
