namespace Sts2Ai.CombatSimulator.State;

public class SimCardPile
{
    private readonly List<SimCard> _cards = new();

    public IReadOnlyList<SimCard> Cards => _cards;
    public int Count => _cards.Count;

    public void Add(SimCard card) => _cards.Add(card);
    public void AddRange(IEnumerable<SimCard> cards) => _cards.AddRange(cards);
    public bool Remove(SimCard card) => _cards.Remove(card);
    public void Clear() => _cards.Clear();

    public SimCard? Draw()
    {
        if (_cards.Count == 0) return null;
        var card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public List<SimCard> DrawMultiple(int count)
    {
        var drawn = new List<SimCard>();
        for (int i = 0; i < count; i++)
        {
            var card = Draw();
            if (card == null) break;
            drawn.Add(card);
        }
        return drawn;
    }

    public void Shuffle(IRandom rng)
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public void InsertAt(int index, SimCard card) => _cards.Insert(index, card);

    public void MoveTo(SimCardPile target, Func<SimCard, bool>? filter = null)
    {
        var cardsToMove = filter == null ? _cards.ToList() : _cards.Where(filter).ToList();
        foreach (var card in cardsToMove)
        {
            _cards.Remove(card);
            target.Add(card);
        }
    }

    public SimCardPile Clone()
    {
        var pile = new SimCardPile();
        foreach (var card in _cards)
            pile.Add(card.Clone());
        return pile;
    }
}
