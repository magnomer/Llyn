namespace Llyn.Core;

public sealed record LLayout(
    string LLayoutTab,
    LCatalogOrder? LLayoutOrder = null,
    LCatalogFilter? LLayoutFilter = null);
