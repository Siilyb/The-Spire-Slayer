using Sts2Ai.Cards;
using Sts2Ai.CombatSimulator.State;

namespace Sts2AiMod;

public static class CardMapper
{
    private static readonly Dictionary<string, Func<SimCard>> _map = new();

    public static void Initialize()
    {
        CardRegistry.RegisterAll();
        _map.Clear();
        foreach (var kv in CardRegistry.All)
            _map[kv.Key] = kv.Value;
    }

    public static SimCard? CreateSimCard(string gameCardId)
    {
        if (_map.TryGetValue(gameCardId, out var factory))
            return factory();
        return null;
    }
}
