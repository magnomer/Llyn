using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSPathfinderMedium
{
    private readonly IReadOnlyList<CPortraitChoice> _bsPathfinderMediumChoices;
    private int _bsPathfinderMediumChosen;

    internal BSPathfinderMedium(IReadOnlyList<CPortraitChoice> choices)
    {
        ArgumentNullException.ThrowIfNull(choices);
        ArgumentOutOfRangeException.ThrowIfZero(choices.Count);

        _bsPathfinderMediumChoices = choices;
        _bsPathfinderMediumChosen =
            Math.Max(choices.ToList().FindIndex(static choice => choice.CPortraitChoiceChosen), 0);
    }

    internal CPortraitChoice BSPathfinderMediumChoice => _bsPathfinderMediumChoices[_bsPathfinderMediumChosen];

    internal void BSPathfinderMediumMove(int step)
    {
        _bsPathfinderMediumChosen =
            Math.Clamp(_bsPathfinderMediumChosen + step, 0, _bsPathfinderMediumChoices.Count - 1);
    }

    internal string BSPathfinderMediumRemove(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        string? known = _bsPathfinderMediumChoices
            .Select(static choice => choice.CPortraitChoiceSuffix)
            .Where(suffix => suffix.Length > 0 && text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            .MaxBy(static suffix => suffix.Length);
        return known is null ? text : text[..^known.Length];
    }
}
