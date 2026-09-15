using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PDiweiLine
{
    private readonly List<string> _pDiweiLineCharacters = [];

    internal PDiweiLine(LFanqieRow row, LHypothesis? hypothesis)
    {
        ArgumentNullException.ThrowIfNull(row);

        string? final = hypothesis?.LHypothesisFinalFind(row);
        PDiweiLineReading = final is null ? string.Empty : '/' + final + '/';
        PDiweiLineRime = LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime);
        PDiweiLineRounded = row.LFanqieRowRounded;
    }

    public string PDiweiLineReading { get; }

    public string PDiweiLineRime { get; }

    public bool PDiweiLineRounded { get; }

    public IReadOnlyList<string> PDiweiLineCharacters => _pDiweiLineCharacters;

    internal void PDiweiLineAdd(string character)
    {
        if (character.Length > 0 && !_pDiweiLineCharacters.Contains(character))
        {
            _pDiweiLineCharacters.Add(character);
        }
    }
}
