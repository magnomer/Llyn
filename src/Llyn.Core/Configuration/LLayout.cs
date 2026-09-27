using System;

namespace Llyn.Core;

public sealed record LLayout(
    string LLayoutTab,
    LCatalogOrder? LLayoutOrder = null,
    LCatalogFilter? LLayoutFilter = null)
{
    public bool LLayoutTabMatch(string tab)
    {
        return string.Equals(LLayoutTab, tab, StringComparison.Ordinal);
    }
}
