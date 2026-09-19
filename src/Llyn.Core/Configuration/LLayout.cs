using System;
using System.Text.Json.Serialization;

namespace Llyn.Core;

public sealed record LLayout(
    string LLayoutTab,
    double? LLayoutLeft = null,
    double? LLayoutMiddle = null,
    LCatalogOrder? LLayoutOrder = null,
    LCatalogFilter? LLayoutFilter = null)
{
    public bool LLayoutTabMatch(string tab)
    {
        return string.Equals(LLayoutTab, tab, StringComparison.Ordinal);
    }
}
