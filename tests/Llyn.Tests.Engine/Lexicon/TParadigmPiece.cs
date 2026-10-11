using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TParadigmPiece
{
    [Fact]
    public void ParadigmFormScan_MarkedMiddle_CutsThreePieces()
    {
        LParadigmForm form = TInterfaceInflection.TParadigmFormCreate(
            "tuve", [TInterfaceInflection.TInflectionMarkCreate(1, 2)], null);

        Assert.Equal(
            [("t", false, false), ("uv", true, false), ("e", false, false)],
            TInterfaceInflection.TParadigmFormScan(form));
    }

    [Fact]
    public void ParadigmFormScan_Split_CutsRootFromEnding()
    {
        LParadigmForm form = TInterfaceInflection.TParadigmFormCreate(
            "tengo", [TInterfaceInflection.TInflectionMarkCreate(0, 4)], null, 4);

        Assert.Equal(
            [("teng", true, true), ("o", false, false)],
            TInterfaceInflection.TParadigmFormScan(form));
    }
}
