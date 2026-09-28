using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TCitationRow
{
    [Fact]
    public void CitationRowFind_WordInByline_SplitsAroundMatchIgnoringCase()
    {
        IReadOnlyList<CCitationRow> rows = TInterfaceConduct.TCitationRowFind(
            [TCitationSourceCreate(3, "Smith (1990)", 4)], "smi");

        CCitationRow row = Assert.Single(rows);
        Assert.Equal(3, row.CCitationRowId);
        Assert.Equal(string.Empty, row.CCitationRowLead);
        Assert.Equal("Smi", row.CCitationRowMark);
        Assert.Equal("th (1990)", row.CCitationRowTail);
        Assert.Equal("4", row.CCitationRowCount);
    }

    [Fact]
    public void CitationRowFind_WordOutsideByline_ReadsBylineWholeWithoutCount()
    {
        CCitationRow row = Assert.Single(TInterfaceConduct.TCitationRowFind(
            [TCitationSourceCreate(3, "Smith (1990)", 0)], "1991"));

        Assert.Equal("Smith (1990)", row.CCitationRowLead);
        Assert.Equal(string.Empty, row.CCitationRowMark);
        Assert.Equal(string.Empty, row.CCitationRowTail);
        Assert.Equal(string.Empty, row.CCitationRowCount);
    }

    [Fact]
    public void CitationRowFind_ManyReferences_OffersEightInFoundOrder()
    {
        IReadOnlyList<CCatalogReference> found =
        [
            .. Enumerable.Range(1, 10).Select(id => TCitationSourceCreate(id, "Smith " + id, id)),
        ];

        IReadOnlyList<CCitationRow> rows = TInterfaceConduct.TCitationRowFind(found, "Smith");

        Assert.Equal([1L, 2, 3, 4, 5, 6, 7, 8], rows.Select(row => row.CCitationRowId));
    }

    private static CCatalogReference TCitationSourceCreate(long id, string byline, int usage) =>
        new(id, byline, byline, CStateValue.CStateValueEmpty, CStateValue.CStateValueEmpty, usage, false);
}
