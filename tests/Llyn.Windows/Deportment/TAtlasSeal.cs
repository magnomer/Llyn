using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtlasSeal
{
    [Fact]
    public void AtlasLegendRead_ShapeLegend_CarriesEveryWord()
    {
        CPortraitLegend legend = new(
            "?", "Untitled", "Unwritten", "Unused", "Once", "uses", "Translation", "Source",
            "Author", "Year", "Url", "Note", "Description", new Dictionary<string, string>());

        LPortraitLegend held = TInterfaceGate.TAtlasLegendRead(legend);

        Assert.Equal("?", held.LPortraitLegendUnknown);
        Assert.Equal("Untitled", held.LPortraitLegendUntitled);
        Assert.Equal("Unwritten", held.LPortraitLegendUnwritten);
        Assert.Equal("Unused", held.LPortraitLegendUnused);
        Assert.Equal("Once", held.LPortraitLegendOnce);
        Assert.Equal("uses", held.LPortraitLegendUses);
        Assert.Equal("Translation", held.LPortraitLegendTranslation);
        Assert.Equal("Source", held.LPortraitLegendSource);
        Assert.Equal("Author", held.LPortraitLegendAuthor);
        Assert.Equal("Year", held.LPortraitLegendYear);
        Assert.Equal("Url", held.LPortraitLegendUrl);
        Assert.Equal("Note", held.LPortraitLegendNote);
        Assert.Equal("Description", held.LPortraitLegendDescription);
    }

    [Fact]
    public void AtlasSituationRead_NoSituation_ReturnsNone()
    {
        Assert.Null(TInterfaceGate.TAtlasSituationRead(null));
    }

    [Fact]
    public void AtlasSituationRead_StoredSituation_CarriesEveryField()
    {
        LSituation situation = TInterface.TSituationCreate(7, "Hearth", "By the fire", LStateValue.LStateValueUnknown);

        CSituationDraft? held = TInterfaceGate.TAtlasSituationRead(situation);

        Assert.NotNull(held);
        Assert.Equal(7, held!.CSituationDraftId);
        Assert.Equal("Hearth", held.CSituationDraftTitle.CStateValueText);
        Assert.Equal("By the fire", held.CSituationDraftDescription.CStateValueText);
        Assert.True(held.CSituationDraftKind.CStateValueUncertain);
        Assert.Empty(held.CSituationDraftImage);
        Assert.Empty(held.CSituationDraftVideo);
    }
}
