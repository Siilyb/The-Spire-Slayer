using System.Runtime.InteropServices;
using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Effects;
using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Enemies;
using Sts2Ai.Mcts;

namespace Sts2AiMod;

public static class ConsoleDisplay
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
    [DllImport("kernel32.dll")]
    private static extern nint GetConsoleWindow();
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);
    private const int SW_SHOW = 5;

    private static int _step;
    private static string _logPath = "";

    public static void Initialize()
    {
        _logPath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".",
            "sts2_ai_debug.log");

        try
        {
            bool ok = AllocConsole();
            var hWnd = GetConsoleWindow();
            if (hWnd != nint.Zero) ShowWindow(hWnd, SW_SHOW);
            Log($"AllocConsole={ok}, HWND={hWnd}");
            if (ok || hWnd != nint.Zero)
            {
                Console.WriteLine("=== STS2 AI 战斗辅助 ===");
                Console.WriteLine("进入战斗后自动显示推荐。\n");
            }
        }
        catch (Exception ex) { Log($"Init: {ex.Message}"); }
    }

    public static void Log(string msg)
    {
        try { File.AppendAllText(_logPath, $"[{DateTime.Now:T}] {msg}\n"); } catch { }
    }

    public static void Update(SimState state, List<AiAction> actions, string? label)
    {
        _step++;
        Log($"Update #{_step}: label={label}, hand={state.Hand.Count}, actions={actions.Count}");

        try { if (GetConsoleWindow() == nint.Zero) return; } catch { return; }
        try { Console.Clear(); } catch { }

        var sep = new string('=', 56);
        Console.WriteLine(sep);
        Console.WriteLine($"  [{_step}] {(label ?? "")}");
        Console.WriteLine(sep);

        // Player
        foreach (var p in state.Players)
            Console.WriteLine($"  玩家 HP:{p.CurrentHp}/{p.MaxHp}  Block:{p.Block}  能量:{state.Energy}/{state.MaxEnergy}");

        // Enemies with numbering + intent
        Console.WriteLine();
        int ei = 0;
        foreach (var e in state.Enemies.Where(x => x.IsAlive))
        {
            ei++;
            // Compute intent
            var ai = e.CustomState.TryGetValue("__ai", out var a) ? a as IEnemyAi : null;
            var plan = ai?.PlanNextTurn(state, e);
            string intent = plan?.Intent.ToString() ?? "?";
            string dmg = plan?.Damage > 0 ? $" ({plan.Damage}×{plan.Hits})" : "";
            Console.WriteLine($"  #{ei} {e.Name} HP:{e.CurrentHp}/{e.MaxHp}  Block:{e.Block}  [{intent}]{dmg}");
        }

        // MCTS recommendation sequence
        Console.WriteLine($"\n  ▶ MCTS 推荐:");
        int idx = 0;
        foreach (var act in actions)
        {
            if (act.Card == null) { Console.WriteLine($"    {++idx}. 结束回合"); break; }
            string t = "";
            if (act.Target != null)
            {
                int ti = 1;
                foreach (var te in state.Enemies.Where(x => x.IsAlive))
                {
                    if (te.Name == act.Target.Name) break;
                    ti++;
                }
                t = $" → #{ti} {act.Target.Name}";
            }
            string note = act.ChoiceCard != null ? $" (升级{act.ChoiceCard.Name})" : "";
            Console.WriteLine($"    {++idx}. [{act.Card.Cost}] {act.Card.Name}{t}{note}");
        }

        // Hand
        Console.WriteLine($"\n  手牌({state.Hand.Count}):");
        foreach (var c in state.Hand.Cards)
        {
            string note = "";
            if (RequiresChoice(c)) note = " ⚠需选牌";
            Console.WriteLine($"    [{c.Cost}] {c.Name}{note}");
        }

        // Upcoming draws
        Console.WriteLine($"\n  接下来抽到:");
        for (int i = 0; i < Math.Min(5, state.DrawPile.Count); i++)
        {
            var c = state.DrawPile.Cards[i];
            Console.WriteLine($"    [{c.Cost}] {c.Name}");
        }

        Console.WriteLine($"\n  抽:{state.DrawPile.Count}  弃:{state.DiscardPile.Count}  耗:{state.ExhaustPile.Count}");
        Console.WriteLine();
    }

    private static bool RequiresChoice(SimCard card) =>
        card.CurrentEffects.Any(e =>
        e is UpgradeFromHand ||
        e is ExhaustFromHand ||
        e is DuplicateCardInHand);
}
