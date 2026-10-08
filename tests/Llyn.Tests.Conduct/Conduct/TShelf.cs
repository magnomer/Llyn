using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TShelf
{
    [Fact]
    public void ShelfReferenceSelect_Source_ShowsColophon()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        CColophon? shown = null;
        shelf.CShelfColophonChanged += colophon => shown = colophon;
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        shelf.CShelfReferenceSelect(book.LReferenceId);

        Assert.Equal(1, recorded);
        Assert.True(shelf.CShelfDiptych.CDiptychParentShown);
        Assert.True(shelf.CShelfDiptych.CDiptychBinEnabled);
        Assert.False(shelf.CShelfDiptych.CDiptychChildShown);
        Assert.Equal("Book", shown?.CColophonTitle);
        Assert.Equal(
            ["Book"],
            shelf.CShelfRollRead().CShelfRollRows
                .Where(row => row.CCatalogReferenceChosen)
                .Select(row => row.CCatalogReferenceName));
        Assert.False(shelf.CShelfRollRead().CShelfRollEmpty);
        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageNone"), shelf.CShelfRollRead().CShelfRollTally);
    }

    [Fact]
    public void ShelfReferenceCreate_SourceChosen_OpensAnEntryCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.CShelfReferenceSelect(book.LReferenceId);
        List<CEntryDraft> shown = [];
        shelf.CShelfEditor.CEditorEntry.CEntryDraftChanged += shown.Add;

        shelf.CShelfDiptych.CDiptychEntryCreate();

        CExampleDraft? cited = shown[0].CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.CExampleDraftReference);
        Assert.True(shelf.CShelfDiptych.CDiptychChildEditing);
        Assert.False(shelf.CShelfDiptych.CDiptychBinEnabled);
        Assert.True(shelf.CShelfDiptych.CDiptychModeEnabled);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
    }

    [Fact]
    public void ShelfReferenceCreate_NothingChosen_OpensASourceDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        shelf.CShelfDiptych.CDiptychEntryCreate();

        Assert.True(shelf.CShelfImprint.CImprintHeld);
        Assert.False(shelf.CShelfImprint.CImprintDesk.CDeskStored);
        Assert.True(shelf.CShelfDiptych.CDiptychParentEditing);
        Assert.True(shelf.CShelfDiptych.CDiptychScribeChecked);
        Assert.False(shelf.CShelfStoreEnabled);
    }

    [Fact]
    public void ShelfEntrySelect_ScribeOffOnFreshEntry_ReturnsToSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LEntry water = TShelfEntrySave(engine, "water");
        shelf.CShelfReferenceSelect(book.LReferenceId);

        shelf.CShelfEntrySelect(water.LEntryId);

        Assert.True(shelf.CShelfDiptych.CDiptychChildShown);
        Assert.False(shelf.CShelfDiptych.CDiptychBinEnabled);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfPortraitAllowed);

        shelf.CShelfScribeToggle(true);

        Assert.True(shelf.CShelfDiptych.CDiptychChildEditing);

        shelf.CShelfDiptych.CDiptychEntryCreate();
        shelf.CShelfScribeToggle(false);

        Assert.True(shelf.CShelfDiptych.CDiptychParentShown);
        Assert.True(shelf.CShelfDiptych.CDiptychBinEnabled);
        Assert.True(shelf.CShelfDiptych.CDiptychViewerChecked);
    }

    [Fact]
    public void ShelfReferenceDelete_EntrySide_LeavesTheSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, []));
        LReference book = engine.TEngineCitationCreate("Book");
        LEntry water = TShelfEntrySave(engine, "water");
        shelf.CShelfReferenceSelect(book.LReferenceId);
        shelf.CShelfEntrySelect(water.LEntryId);

        shelf.CShelfDiptych.CDiptychEntryDelete();

        Assert.Contains(shelf.CShelfRollRead().CShelfRollRows, row => row.CCatalogReferenceName == "Book");

        shelf.CShelfFootnote.CFootnotePanel.CPanelScribeToggle(false);
        shelf.CShelfReferenceSelect(book.LReferenceId);
        shelf.CShelfDiptych.CDiptychEntryDelete();

        Assert.DoesNotContain(shelf.CShelfRollRead().CShelfRollRows, row => row.CCatalogReferenceName == "Book");
        Assert.False(shelf.CShelfDiptych.CDiptychBinEnabled);
    }

    [Fact]
    public void ShelfOrderSet_EachOfferedOrder_KeepsEverySource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineCitationCreate("Beta");
        engine.TEngineCitationCreate("Alpha");
        int listed = shelf.CShelfRollRead().CShelfRollRows.Count;

        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderYear,
                CCatalogOrder.CCatalogOrderAuthor,
                CCatalogOrder.CCatalogOrderUsage,
            ],
            CShelf.CShelfOrderRead());
        foreach (CCatalogOrder order in CShelf.CShelfOrderRead())
        {
            shelf.CShelfPanel.CPanelAperture.CApertureOrderSet(order);

            Assert.Equal(listed, shelf.CShelfRollRead().CShelfRollRows.Count);
            Assert.Equal(order, shelf.CShelfPanel.CPanelAperture.CApertureOrder);
        }

        shelf.CShelfPanel.CPanelAperture.CApertureOrderSet(CCatalogOrder.CCatalogOrderName);
        shelf.CShelfPanel.CPanelAperture.CApertureOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderName, shelf.CShelfPanel.CPanelAperture.CApertureOrder);
        Assert.Equal(
            ["Alpha", "Beta"],
            shelf.CShelfRollRead().CShelfRollRows
                .Select(row => row.CCatalogReferenceName)
                .Where(name => name is "Alpha" or "Beta"));
    }

    [Fact]
    public void ShelfQuerySet_UnmatchedText_EmptiesTheShelfAndClosesTheSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.CShelfReferenceSelect(book.LReferenceId);

        shelf.CShelfPanel.CPanelAperture.CApertureQuerySet("zzz");

        Assert.Empty(shelf.CShelfRollRead().CShelfRollRows);
        Assert.True(shelf.CShelfRollRead().CShelfRollEmpty);
        Assert.False(shelf.CShelfDiptych.CDiptychBinEnabled);
    }

    [Fact]
    public void ShelfFilterSet_HiddenLanguage_MarksTheShelfFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        shelf.CShelfPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter(["Latin"]));

        Assert.True(shelf.CShelfPanel.CPanelAperture.CApertureFiltered);
        Assert.Equal(["Latin"], shelf.CShelfPanel.CPanelAperture.CApertureFilter.CCatalogFilterHidden);

        shelf.CShelfPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter([]));

        Assert.False(shelf.CShelfPanel.CPanelAperture.CApertureFiltered);
    }

    [Fact]
    public void ShelfScribeToggle_SourceSideOff_DropsTheSourceDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.CShelfReferenceSelect(book.LReferenceId);

        shelf.CShelfScribeToggle(true);

        Assert.True(shelf.CShelfDiptych.CDiptychParentEditing);
        Assert.True(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfDiptych.CDiptychScribeChecked);

        shelf.CShelfScribeToggle(false);

        Assert.True(shelf.CShelfDiptych.CDiptychParentShown);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfDiptych.CDiptychViewerChecked);
    }

    [Fact]
    public async Task ShelfPortraitExport_EntryOnDisplay_WritesTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");
        CShelf shelf = TShelfPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        LEntry hearth = TShelfEntrySave(engine, "hearth");

        await shelf.CShelfPortraitExport();

        Assert.False(File.Exists(path));

        shelf.CShelfEntrySelect(hearth.LEntryId);
        await shelf.CShelfPortraitExport();

        Assert.Equal("hearth", shelf.CShelfFootnote.TFootnoteFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), System.StringComparison.Ordinal);
    }

    [Fact]
    public void ShelfPortraitPrint_NothingShown_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        Task printed = shelf.CShelfPortraitPrint();

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(shelf.CShelfPressAllowed);
    }

    internal static CShelf TShelfPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CShelf shelf = CShelf.CShelfCreate(atelier, static () => true, envoy, static run => run());
        return shelf;
    }

    internal static LEntry TShelfEntrySave(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
