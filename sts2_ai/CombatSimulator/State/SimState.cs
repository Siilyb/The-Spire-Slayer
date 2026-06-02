namespace Sts2Ai.CombatSimulator.State;

public class SimState
{
    public List<SimCreature> Players { get; set; } = new();
    public List<SimCreature> Enemies { get; set; } = new();
    public SimCardPile DrawPile { get; set; } = new();
    public SimCardPile Hand { get; set; } = new();
    public SimCardPile DiscardPile { get; set; } = new();
    public SimCardPile ExhaustPile { get; set; } = new();
    public int Energy { get; set; }
    public int MaxEnergy { get; set; } = 3;
    public int TurnNumber { get; set; }
    public int RoundNumber { get; set; }
    public CombatSide CurrentSide { get; set; } = CombatSide.Player;
    public IRandom Rng { get; set; } = new GameRng();

    public bool IsCombatOver => !Players.Any(p => p.IsAlive) || !Enemies.Any(e => e.IsAlive);

    public SimState Clone()
    {
        return new SimState
        {
            Players = Players.Select(p => p.Clone()).ToList(),
            Enemies = Enemies.Select(e => e.Clone()).ToList(),
            DrawPile = DrawPile.Clone(),
            Hand = Hand.Clone(),
            DiscardPile = DiscardPile.Clone(),
            ExhaustPile = ExhaustPile.Clone(),
            Energy = Energy,
            MaxEnergy = MaxEnergy,
            TurnNumber = TurnNumber,
            RoundNumber = RoundNumber,
            CurrentSide = CurrentSide,
            Rng = Rng.Clone()
        };
    }
}
