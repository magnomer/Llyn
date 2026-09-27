using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogFilter(IReadOnlyList<string> LCatalogFilterHidden)
{
    public static readonly LCatalogFilter LCatalogFilterEmpty = new(Array.Empty<string>());

    public bool LCatalogFilterActive => LCatalogFilterHidden.Count > 0;

    public static LCatalogFilter LCatalogFilterCreate(IReadOnlyList<string> hidden)
    {
        ArgumentNullException.ThrowIfNull(hidden);

        return hidden.Count == 0 ? LCatalogFilterEmpty : new LCatalogFilter(hidden);
    }

    public bool LCatalogFilterMatch(string? language)
    {
        return !LCatalogFilterHidden.Contains(language ?? string.Empty, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<LCatalogRow> LCatalogFilterApply<LCatalogRow>(
        IReadOnlyList<LCatalogRow> rows,
        Func<LCatalogRow, string?> language)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(language);

        return LCatalogFilterActive
            ? [.. rows.Where(row => LCatalogFilterMatch(language(row)))]
            : rows;
    }
}
