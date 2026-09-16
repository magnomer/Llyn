using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LTallyMark(string LTallyMarkText, IReadOnlyList<string> LTallyMarkCharacters)
{
    public string LTallyMarkText { get; init; } = LTallyMarkText ?? string.Empty;

    public IReadOnlyList<string> LTallyMarkCharacters { get; init; } = LTallyMarkCharacters ?? [];

    public int LTallyMarkCount => LTallyMarkCharacters.Count;

    public static IReadOnlyList<LTallyMark> LTallyMarkScan(
        IReadOnlyDictionary<string, List<string>>? parts)
    {
        if (parts is null || parts.Count == 0)
        {
            return [];
        }

        return
        [
            .. parts
                .Select(part => new LTallyMark(part.Key, [.. part.Value]))
                .OrderByDescending(mark => mark.LTallyMarkCount)
                .ThenBy(mark => mark.LTallyMarkText, StringComparer.Ordinal),
        ];
    }
}
