namespace Llyn.Core;

public sealed record LLayout(
    string LLayoutTab,
    double? LLayoutLeft = null,
    double? LLayoutMiddle = null,
    LCatalogOrder? LLayoutOrder = null,
    LCatalogFilter? LLayoutFilter = null);
