using System.Globalization;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplay
{
    [Fact]
    public void DisplayStampFormat_UnreadableText_ReturnsEmpty()
    {
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat("not a moment"));
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat(null));
    }

    [Fact]
    public void DisplayFoldScan_NoRule_ReturnsEmpty()
    {
        Assert.Empty(TInterfaceConduct.TDisplayFoldScan([]));
    }

    [Fact]
    public void DisplayFoldSet_CurrentValue_KeepsFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);

        sound.TDisplayFoldSet(true);
        sound.TDisplayFoldSet(true);

        Assert.True(sound.LDisplayFoldOpened);
    }

    [Fact]
    public void DisplayBandResolve_UnbandedRows_ReturnsZero()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", null),
            TInterface.TFrequencyCreate("two", "3", null),
        ];

        Assert.Equal(0, TInterfaceConduct.TDisplayBandResolve(rows));
    }

    [Fact]
    public void DisplaySourceFormat_OnceInterval_FormatsInterval()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyIntervalCreate("Corpus", "0.5", 12000),
            TInterface.TFrequencyCreate("List", "7", null),
        ];

        string text = TInterfaceConduct.TDisplaySourceFormat(rows, "once in {0} words");

        Assert.Equal(
            "Corpus: once in " + 12000.ToString("N0", CultureInfo.CurrentCulture) + " words\nList: 7",
            text);
    }
}
