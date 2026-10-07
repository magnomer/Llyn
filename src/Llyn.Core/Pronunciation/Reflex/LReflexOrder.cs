using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LReflexOrder(
    IReadOnlyList<string> LReflexOrderLanguages,
    IReadOnlyDictionary<string, IReadOnlyList<string>> LReflexOrderKinds)
{
    public static LReflexOrder LReflexOrderEmpty { get; } =
        new([], new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal));

    public IReadOnlyList<LReflexDraft> LReflexOrderSort(IReadOnlyList<LReflexDraft> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .OrderBy(row => LReflexOrderFind(LReflexOrderLanguages, row.LReflexDraftLanguage))
            .ThenBy(static row => row.LReflexDraftLanguage, StringComparer.Ordinal)
            .ThenBy(row => LReflexOrderFind(
                LReflexOrderKinds.GetValueOrDefault(row.LReflexDraftLanguage) ?? Array.Empty<string>(),
                row.LReflexDraftKind))
            .ThenBy(static row => row.LReflexDraftKind, StringComparer.Ordinal)
            .ThenBy(static row => row.LReflexDraftMain ? 0 : 1)
            .ThenBy(static row => row.LReflexDraftText, StringComparer.Ordinal)
            .ThenBy(static row => row.LReflexDraftId)
            .ToList();
    }

    internal static int LReflexOrderFind(IReadOnlyList<string> declared, string name)
    {
        for (int index = 0; index < declared.Count; index++)
        {
            if (string.Equals(declared[index], name, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return int.MaxValue;
    }
}
