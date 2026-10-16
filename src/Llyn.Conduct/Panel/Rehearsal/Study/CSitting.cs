using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Conduct;

public sealed class CSitting
{
    private readonly IReadOnlyList<(CSittingSide, CSittingSide)> _cSittingCards;

    private readonly Func<int, int> _cSittingChance;

    private readonly Action _cSittingMoved;

    private int[] _cSittingOrder;

    private int _cSittingPosition;

    private bool _cSittingTurned;

    private bool _cSittingShuffled;

    internal CSitting(IReadOnlyList<(CSittingSide, CSittingSide)> cards, Func<int, int> chance, Action moved)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(chance);
        ArgumentNullException.ThrowIfNull(moved);

        _cSittingCards = cards;
        _cSittingChance = chance;
        _cSittingMoved = moved;
        _cSittingOrder = LSittingOrderBuild();
    }

    public CSittingState CSittingRead()
    {
        int count = _cSittingCards.Count;
        bool end = _cSittingPosition >= count;
        CSittingSide blank = new(string.Empty, []);
        (CSittingSide front, CSittingSide back) =
            end ? (blank, blank) : _cSittingCards[_cSittingOrder[_cSittingPosition]];
        string position = string.Format(
            CultureInfo.InvariantCulture, "{0} / {1}", Math.Min(_cSittingPosition + 1, count), count);
        bool empty = back.CSittingSideText.Length == 0 && back.CSittingSidePictures.Count == 0
            && !back.CSittingSideAudible;
        return new CSittingState(
            position, front, back, _cSittingTurned, end, _cSittingShuffled,
            _cSittingTurned && empty ? "Study.BackEmpty" : null);
    }

    public CSittingState CSittingSideToggle()
    {
        if (_cSittingPosition < _cSittingCards.Count)
        {
            _cSittingTurned = !_cSittingTurned;
        }

        return CSittingRead();
    }

    public CSittingState CSittingShuffleToggle()
    {
        _cSittingShuffled = !_cSittingShuffled;
        _cSittingOrder = LSittingOrderBuild();
        _cSittingPosition = 0;
        _cSittingTurned = false;
        _cSittingMoved();
        return CSittingRead();
    }

    public CSittingState CSittingAdvance()
    {
        if (_cSittingPosition < _cSittingCards.Count)
        {
            _cSittingPosition++;
            _cSittingTurned = false;
            _cSittingMoved();
        }

        return CSittingRead();
    }

    public CSittingState CSittingRetreat()
    {
        if (_cSittingPosition > 0)
        {
            _cSittingPosition--;
            _cSittingTurned = false;
            _cSittingMoved();
        }

        return CSittingRead();
    }

    private int[] LSittingOrderBuild()
    {
        int[] order = Enumerable.Range(0, _cSittingCards.Count).ToArray();
        if (!_cSittingShuffled)
        {
            return order;
        }

        for (int last = order.Length - 1; last > 0; last--)
        {
            int drawn = _cSittingChance(last + 1);
            (order[last], order[drawn]) = (order[drawn], order[last]);
        }

        return order;
    }
}
