using System;
using Llyn.Core;

namespace Llyn.Application;

public static class LCatalogClerk
{
    public static string LCatalogClerkFormat(LCatalogFilter? filter)
    {
        return LCatalog.LCatalogFilterFormat(filter ?? LCatalogFilter.LCatalogFilterEmpty);
    }
}
