using System;

namespace Llyn.Core;

public sealed record LParadigmShown(string LParadigmShownText, string? LParadigmShownTip)
{
    public static LParadigmShown LParadigmShownResolve(LParadigmStatus status, string text, bool held)
    {
        ArgumentNullException.ThrowIfNull(text);

        return status switch
        {
            LParadigmStatus.LParadigmStatusText => new LParadigmShown(text, null),
            LParadigmStatus.LParadigmStatusUnknown => new LParadigmShown("—", "Paradigm.Unknown"),
            LParadigmStatus.LParadigmStatusPending => new LParadigmShown("…", "Paradigm.Pending"),
            LParadigmStatus.LParadigmStatusLost => new LParadigmShown("…", held ? "Paradigm.Held" : "Paradigm.Lost"),
            LParadigmStatus.LParadigmStatusAbsent => new LParadigmShown("…", "Paradigm.Absent"),
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }
}
