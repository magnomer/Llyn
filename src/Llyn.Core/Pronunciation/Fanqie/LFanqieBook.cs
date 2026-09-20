using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LFanqieBook(
    string LFanqieBookName,
    string LFanqieBookUrl,
    IReadOnlyDictionary<string, string> LFanqieBookForm,
    string LFanqieBookPattern,
    string? LFanqieBookBusy = null,
    int LFanqieBookInterval = 0,
    string? LFanqieBookSplit = null,
    string? LFanqieBookHead = null,
    string? LFanqieBookColumn = null,
    string? LFanqieBookRounded = null,
    string? LFanqieBookSource = null,
    string? LFanqieBookLine = null,
    string? LFanqieBookSpelling = null)
{
    public bool LFanqieBookMatch(string name)
    {
        return string.Equals(LFanqieBookName, name, StringComparison.Ordinal);
    }

    public string LFanqieBookSource { get; init; } = LFanqieBookSource ?? LFanqieBookName;
}
