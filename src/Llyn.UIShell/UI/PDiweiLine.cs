using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PDiweiLine
{
    private readonly List<string> _pDiweiLineCharacters = [];

    internal PDiweiLine(string kind, LFanqieRow row, LHypothesis? hypothesis)
    {
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(row);

        bool rime = kind == LDiwei.LDiweiRime;
        string? part = rime ? hypothesis?.LHypothesisInitialFind(row) : hypothesis?.LHypothesisFinalFind(row);
        PDiweiLineReading = part is null ? string.Empty : '/' + part + '/';
        PDiweiLineLabel = rime ? row.LFanqieRowInitial : LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime);
        PDiweiLineRounded = !rime && row.LFanqieRowRounded;
        PDiweiLineRank = rime ? hypothesis?.LHypothesisRankRead(row.LFanqieRowInitial) ?? -1 : -1;
    }

    public string PDiweiLineReading { get; }

    public string PDiweiLineLabel { get; }

    public bool PDiweiLineRounded { get; }

    public int PDiweiLineRank { get; }

    public IReadOnlyList<string> PDiweiLineCharacters => _pDiweiLineCharacters;

    internal static void PDiweiLineSort(List<PDiweiLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        lines.Sort((left, right) =>
        {
            int order = LDiwei.LDiweiRankNormalize(left.PDiweiLineRank)
                .CompareTo(LDiwei.LDiweiRankNormalize(right.PDiweiLineRank));
            return order != 0 ? order : string.CompareOrdinal(left.PDiweiLineLabel, right.PDiweiLineLabel);
        });
    }

    internal void PDiweiLineAdd(string character)
    {
        if (character.Length > 0 && !_pDiweiLineCharacters.Contains(character))
        {
            _pDiweiLineCharacters.Add(character);
        }
    }
}
