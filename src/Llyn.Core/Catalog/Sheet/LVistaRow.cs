using System;

namespace Llyn.Core;

public sealed record LVistaRow(
    long LVistaRowId,
    string LVistaRowHeadword,
    string LVistaRowLanguage,
    string? LVistaRowEpithet,
    string LVistaRowName,
    bool LVistaRowChosen)
{
    public bool LVistaRowMatch(LVistaRow other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return LVistaRowId == other.LVistaRowId
            && string.Equals(LVistaRowHeadword, other.LVistaRowHeadword, StringComparison.Ordinal)
            && string.Equals(LVistaRowEpithet, other.LVistaRowEpithet, StringComparison.Ordinal)
            && string.Equals(LVistaRowName, other.LVistaRowName, StringComparison.Ordinal);
    }
}
