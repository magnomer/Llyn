using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed class LDiweiLine
{
    private readonly List<string> _lDiweiLineCharacters = [];

    internal LDiweiLine(bool rime, LFanqieRow row, LHypothesis? hypothesis)
    {
        ArgumentNullException.ThrowIfNull(row);

        string? part = rime ? hypothesis?.LHypothesisInitialFind(row) : hypothesis?.LHypothesisFinalFind(row);
        LDiweiLineReading = part is null ? string.Empty : '/' + part + '/';
        LDiweiLineLabel = rime ? row.LFanqieRowInitial : LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime);
        LDiweiLineRounded = !rime && row.LFanqieRowRounded;
        LDiweiLineRank = rime ? hypothesis?.LHypothesisRankRead(row.LFanqieRowInitial) ?? -1 : -1;
    }

    public string LDiweiLineReading { get; }

    public string LDiweiLineLabel { get; }

    public bool LDiweiLineRounded { get; }

    public int LDiweiLineRank { get; }

    public IReadOnlyList<string> LDiweiLineCharacters => _lDiweiLineCharacters;

    internal static void LDiweiLineSort(List<LDiweiLine> lines)
    {
        lines.Sort((left, right) =>
        {
            int order = LDiwei.LDiweiRankNormalize(left.LDiweiLineRank)
                .CompareTo(LDiwei.LDiweiRankNormalize(right.LDiweiLineRank));
            return order != 0 ? order : string.CompareOrdinal(left.LDiweiLineLabel, right.LDiweiLineLabel);
        });
    }

    internal void LDiweiLineAdd(string character)
    {
        if (character.Length > 0 && !_lDiweiLineCharacters.Contains(character))
        {
            _lDiweiLineCharacters.Add(character);
        }
    }
}
