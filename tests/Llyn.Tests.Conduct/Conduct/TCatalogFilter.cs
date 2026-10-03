using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogFilter
{
    [Fact]
    public void CatalogFilterMatch_HiddenLanguage_IsFalseInAnyCase()
    {
        CCatalogFilter filter = new(["Latin"]);

        Assert.False(filter.TCatalogFilterMatch("Latin"));
        Assert.False(filter.TCatalogFilterMatch("latin"));
    }

    [Fact]
    public void CatalogFilterMatch_ShownLanguage_IsTrue()
    {
        CCatalogFilter filter = new(["Latin"]);

        Assert.True(filter.TCatalogFilterMatch("Greek"));
        Assert.True(filter.TCatalogFilterMatch(null));
    }
}
