namespace Sts2Ai.CombatSimulator;

public interface IRandom
{
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
    double NextDouble();
}

public class DefaultRandom : IRandom
{
    private readonly Random _rng = new();

    public int Next(int maxValue) => _rng.Next(maxValue);
    public int Next(int minValue, int maxValue) => _rng.Next(minValue, maxValue);
    public double NextDouble() => _rng.NextDouble();
}

public class SeedRandom : IRandom
{
    private readonly Random _rng;

    public SeedRandom(int seed) => _rng = new Random(seed);

    public int Next(int maxValue) => _rng.Next(maxValue);
    public int Next(int minValue, int maxValue) => _rng.Next(minValue, maxValue);
    public double NextDouble() => _rng.NextDouble();
}
