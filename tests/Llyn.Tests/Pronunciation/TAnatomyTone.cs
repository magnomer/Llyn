using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnatomyTone
{
    private const string TAnatomyToneLanguage = "Classical Chinese";

    private static IReadOnlyList<LAnatomyTone> TAnatomyToneRead() =>
        TInterface.TLanguageLoad(TAnatomyToneLanguage).LLanguageAnatomyTones;

    [Fact]
    public void LanguageLoaderLoad_ClassicalChinesePack_ReadsToneRows()
    {
        IReadOnlyList<LAnatomyTone> rows = TAnatomyToneRead();

        Assert.NotEmpty(rows);
        Assert.Contains(rows, row => row.TAnatomyToneMatch("Mandarin"));
        Assert.Contains(rows, row => row.TAnatomyToneMatch("Cantonese"));
        Assert.Empty(TInterface.TLanguageLoad("Korean").LLanguageAnatomyTones);
    }

    [Fact]
    public void AnatomyToneScan_MandarinLevelTone_NamesPlainAndChecked()
    {
        Assert.Equal(["1", "7"], TInterface.TAnatomyToneScan(TAnatomyToneRead(), "Mandarin", "55"));
    }

    [Fact]
    public void AnatomyToneScan_CantoneseVariants_ReadOneClass()
    {
        IReadOnlyList<LAnatomyTone> rows = TAnatomyToneRead();

        Assert.Equal(["3"], TInterface.TAnatomyToneScan(rows, "Cantonese", "35"));
        Assert.Equal(["3"], TInterface.TAnatomyToneScan(rows, "Cantonese", "34"));
        Assert.Equal(["4S", "4"], TInterface.TAnatomyToneScan(rows, "Cantonese", "13"));
    }

    [Fact]
    public void AnatomyToneScan_SandhiForm_ReadsCitationTone()
    {
        Assert.Equal(["3", "4S"], TInterface.TAnatomyToneScan(TAnatomyToneRead(), "Mandarin", "214-21"));
    }

    [Fact]
    public void AnatomyToneScan_UnlistedContour_ReadsNothing()
    {
        IReadOnlyList<LAnatomyTone> rows = TAnatomyToneRead();

        Assert.Empty(TInterface.TAnatomyToneScan(rows, "Mandarin", "44"));
        Assert.Empty(TInterface.TAnatomyToneScan(rows, "Mandarin", ""));
        Assert.Empty(TInterface.TAnatomyToneScan(rows, "Korean", "55"));
    }
}
