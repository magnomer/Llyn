using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TLineage
{
    [Fact]
    public void LineageClerkRead_LinkedBox_ShowsCurrentHeadword()
    {
        TVaultFake entries = new();
        LRig rig = TInterface.TRigClerkCreate(entries);
        long linked = TLineageEntryAdd(entries, "renn", "Old English");

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0, [TInterface.TAncestorDraftCreate(-1, null, linked, "ren")]));

        Assert.Equal("renn", box.LLineageText);
        Assert.Equal(linked, box.LLineageEntryId);
    }

    [Fact]
    public void LineageClerkRead_LinkedBox_ShowsEntryLanguage()
    {
        TVaultFake entries = new();
        LRig rig = TInterface.TRigClerkCreate(entries);
        long linked = TLineageEntryAdd(entries, "renn", "Old English");

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0, [TInterface.TAncestorDraftCreate(-1, null, linked, string.Empty, "English")]));

        Assert.Equal("Old English", box.LLineageLanguage);
    }

    [Fact]
    public void LineageClerkRead_GoneEntry_ShowsStoredText()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0, [TInterface.TAncestorDraftCreate(-1, null, 404, "ren")]));

        Assert.Equal("ren", box.LLineageText);
        Assert.Null(box.LLineageEntryId);
        Assert.Empty(box.LLineageSources);
    }

    [Fact]
    public void LineageClerkRead_LinkedEntryMap_BorrowsStoredBoxes()
    {
        TVaultFake entries = new();
        LRig rig = TInterface.TRigClerkCreate(entries);
        long linked = TLineageEntryAdd(entries, "renn", "Old English");
        rig.TAncestorMapSave(
            linked,
            TInterface.TAncestorBoxCreate(1, null, "rinnan") with { LAncestorLanguage = "Proto-Germanic" });

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0, [TInterface.TAncestorDraftCreate(-1, null, linked, string.Empty)]));

        LLineage borrowed = Assert.Single(box.LLineageSources);
        Assert.False(box.LLineageBorrowed);
        Assert.True(borrowed.LLineageBorrowed);
        Assert.Equal("rinnan", borrowed.LLineageText);
        Assert.Equal("Proto-Germanic", borrowed.LLineageLanguage);
    }

    [Fact]
    public void LineageClerkRead_SourcesAndCoSources_KeepsDraftOrder()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());

        LLineage root = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0,
            [
                TInterface.TAncestorDraftCreate(-1, null, null, "brunch"),
                TInterface.TAncestorDraftCreate(-2, -1, null, "breakfast"),
                TInterface.TAncestorDraftCreate(-3, -1, null, "lunch"),
            ]));

        Assert.Equal(["breakfast", "lunch"], root.LLineageSources.Select(source => source.LLineageText));
    }

    [Fact]
    public void LineageClerkRead_TongueBox_ShowsTongueName()
    {
        LRig rig = TInterface.TRigClerkCreate(new TVaultFake());
        long? tongue = TInterface.TAncestorClerkCreate(rig).TAncestorTongueAdd("Old Norse").LAncestorLanguageTongue;

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0, [TInterface.TAncestorDraftCreate(-1, null, null, "skyrta", tongue: tongue)]));

        Assert.Equal("Old Norse", box.LLineageLanguage);
    }

    [Fact]
    public void LineageClerkRead_ChainPastDepthCap_StopsAtCap()
    {
        TVaultFake entries = new();
        LRig rig = TInterface.TRigClerkCreate(entries);
        List<long> chain = [];
        for (int index = 0; index < LLineageClerk.LLineageClerkDepth + 3; index++)
        {
            chain.Add(TLineageEntryAdd(entries, $"word{index}", "English"));
        }

        for (int index = 0; index + 1 < chain.Count; index++)
        {
            rig.TAncestorMapSave(
                chain[index],
                TInterface.TAncestorBoxCreate(1, null, string.Empty, chain[index + 1]));
        }

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            0, [TInterface.TAncestorDraftCreate(-1, null, chain[0], string.Empty)]));

        int levels = 1;
        while (box.LLineageSources.Count > 0)
        {
            box = Assert.Single(box.LLineageSources);
            levels++;
        }

        Assert.Equal(LLineageClerk.LLineageClerkDepth + 1, levels);
        Assert.NotNull(box.LLineageEntryId);
    }

    [Fact]
    public void LineageClerkRead_EntryOnPath_StopsRevisit()
    {
        TVaultFake entries = new();
        LRig rig = TInterface.TRigClerkCreate(entries);
        long reading = TLineageEntryAdd(entries, "brunch", "English");
        long linked = TLineageEntryAdd(entries, "lunch", "English");
        rig.TAncestorMapSave(linked, TInterface.TAncestorBoxCreate(1, null, string.Empty, reading));

        LLineage box = Assert.Single(TInterface.TLineageClerkCreate(rig).TLineageClerkRead(
            reading, [TInterface.TAncestorDraftCreate(-1, null, linked, string.Empty)]));

        LLineage revisit = Assert.Single(box.LLineageSources);
        Assert.Equal(reading, revisit.LLineageEntryId);
        Assert.Equal("brunch", revisit.LLineageText);
        Assert.Empty(revisit.LLineageSources);
    }

    private static long TLineageEntryAdd(TVaultFake entries, string headword, string language) =>
        entries.TVaultFakeAdd(TInterface.TEntryCreate(0, headword, language, 0, null, null)).LEntryId;
}
