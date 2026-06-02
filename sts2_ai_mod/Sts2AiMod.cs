using System.Reflection;
using System.Runtime.Loader;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Mcts;

namespace Sts2AiMod;

[ModInitializer("Init")]
public static class Sts2AiMod
{
    private static readonly CombatAi _ai = new();
    private static List<AiAction> _lastActions = new();
    private static int _lastTurn;

    public static void Init()
    {
        var modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (modDir != null)
        {
            AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!.Resolving += (ctx, name) =>
            {
                if (name.Name == "sts2_ai_core")
                {
                    var path = Path.Combine(modDir, "sts2_ai_core.dll");
                    return File.Exists(path) ? ctx.LoadFromAssemblyPath(path) : null;
                }
                return null;
            };
        }

        CardMapper.Initialize();
        ConsoleDisplay.Initialize();

        CombatManager.Instance.CombatSetUp += OnCombatSetUp;
        CombatManager.Instance.TurnStarted += OnTurnStarted;
        CombatManager.Instance.CreaturesChanged += OnStateChanged;  // 状态变化就刷新

        ConsoleDisplay.Log("Mod loaded. Events hooked: CombatSetUp, TurnStarted, CreaturesChanged");
    }

    private static void OnCombatSetUp(CombatState state)
    {
        ConsoleDisplay.Log("Combat started");
        UpdateRecommendation(state, "战斗开始");
    }

    private static void OnTurnStarted(CombatState state)
    {
        UpdateRecommendation(state, $"第{state.RoundNumber}回合");
    }

    private static void OnStateChanged(CombatState state)
    {
        // CreaturesChanged fires frequently; debounce by checking turn
        if (state.RoundNumber != _lastTurn)
        {
            _lastTurn = state.RoundNumber;
            return; // Let TurnStarted handle new turns
        }
        UpdateRecommendation(state, null); // Silent update
    }

    private static void UpdateRecommendation(CombatState combatState, string? label)
    {
        try
        {
            var simState = StateConverter.Convert(combatState);

            // Run MCTS to get ALL recommended actions
            _lastActions = _ai.ChooseActions(simState);

            ConsoleDisplay.Update(simState, _lastActions, label);

            // Update the run count for next time
            _lastTurn = combatState.RoundNumber;
        }
        catch (System.Exception ex)
        {
            ConsoleDisplay.Log($"MCTS error: {ex.Message}");
        }
    }
}
