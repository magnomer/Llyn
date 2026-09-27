using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Conduct;

public sealed record CCatalogFilter(IReadOnlyList<string> CCatalogFilterHidden)
{
    public bool CCatalogFilterMatch(string? language)
    {
        return !CCatalogFilterHidden.Contains(language ?? string.Empty, StringComparer.OrdinalIgnoreCase);
    }
}
