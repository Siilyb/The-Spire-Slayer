namespace Sts2Ai.CombatSimulator;

public interface IRandom
{
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    double NextDouble();
    IRandom Clone();
}

public class GameRng : IRandom
{
    private readonly Random _rng;
    public uint Seed { get; }
    public int Counter { get; private set; }

    public GameRng(uint seed = 0, int counter = 0)
    {
        Seed = seed;
        Counter = 0;
        _rng = new Random((int)seed);
        FastForward(counter);
    }

    public GameRng(uint seed, string name)
        : this(seed + (uint)name.GetHashCode())
    {
    }

    public void FastForward(int targetCount)
    {
        while (Counter < targetCount)
        {
            Counter++;
            _rng.Next();
        }
    }

    public int Next(int maxValue)
    {
        Counter++;
        return _rng.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        Counter++;
        return _rng.Next(minValue, maxValue);
    }

    public double NextDouble()
    {
        Counter++;
        return _rng.NextDouble();
    }

    public IRandom Clone() => new GameRng(Seed, Counter);
}
