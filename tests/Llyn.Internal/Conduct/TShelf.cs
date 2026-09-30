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
        Assert.True(shelf.CShelfColophonShown);
        Assert.True(shelf.CShelfBinEnabled);
        Assert.False(shelf.CShelfDisplayShown);
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
        shelf.CShelfEditor.CEditorDraftChanged += shown.Add;

        shelf.CShelfReferenceCreate();

        CExampleDraft? cited = shown[0].CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.CExampleDraftReference);
        Assert.True(shelf.CShelfEditorShown);
        Assert.False(shelf.CShelfBinEnabled);
        Assert.True(shelf.CShelfModeEnabled);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
    }

    [Fact]
    public void ShelfReferenceCreate_NothingChosen_OpensASourceDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        shelf.CShelfReferenceCreate();

        Assert.True(shelf.CShelfImprint.CImprintHeld);
        Assert.False(shelf.CShelfImprint.CImprintDesk.CDeskStored);
        Assert.True(shelf.CShelfImprintShown);
        Assert.True(shelf.CShelfScribeChecked);
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

        Assert.True(shelf.CShelfDisplayShown);
        Assert.False(shelf.CShelfBinEnabled);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfPortraitAllowed);

        shelf.CShelfScribeToggle(true);

        Assert.True(shelf.CShelfEditorShown);

        shelf.CShelfReferenceCreate();
        shelf.CShelfScribeToggle(false);

        Assert.True(shelf.CShelfColophonShown);
        Assert.True(shelf.CShelfBinEnabled);
        Assert.True(shelf.CShelfViewerChecked);
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

        shelf.CShelfReferenceDelete();

        Assert.Contains(shelf.CShelfRollRead().CShelfRollRows, row => row.CCatalogReferenceName == "Book");

        shelf.CShelfFootnote.CFootnotePanel.CPanelScribeToggle(false);
        shelf.CShelfReferenceSelect(book.LReferenceId);
        shelf.CShelfReferenceDelete();

        Assert.DoesNotContain(shelf.CShelfRollRead().CShelfRollRows, row => row.CCatalogReferenceName == "Book");
        Assert.False(shelf.CShelfBinEnabled);
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
            shelf.CShelfOrderSet(order);

            Assert.Equal(listed, shelf.CShelfRollRead().CShelfRollRows.Count);
            Assert.Equal(order, shelf.CShelfPanel.CPanelOrder);
        }

        shelf.CShelfOrderSet(CCatalogOrder.CCatalogOrderName);
        shelf.CShelfOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderName, shelf.CShelfPanel.CPanelOrder);
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

        shelf.CShelfQuerySet("zzz");

        Assert.Empty(shelf.CShelfRollRead().CShelfRollRows);
        Assert.True(shelf.CShelfRollRead().CShelfRollEmpty);
        Assert.False(shelf.CShelfBinEnabled);
    }

    [Fact]
    public void ShelfFilterSet_HiddenLanguage_MarksTheShelfFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        shelf.CShelfFilterSet(new CCatalogFilter(["Latin"]));

        Assert.True(shelf.CShelfFiltered);
        Assert.Equal(["Latin"], shelf.CShelfPanel.CPanelFilter.CCatalogFilterHidden);

        shelf.CShelfFilterSet(new CCatalogFilter([]));

        Assert.False(shelf.CShelfFiltered);
    }

    [Fact]
    public void ShelfReferenceSelect_KeptLeave_StaysOnTheHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.CShelfReferenceCreate();
        shelf.CShelfImprint.CImprintTitleSet("Tome");
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        shelf.CShelfReferenceSelect(book.LReferenceId);

        Assert.Equal(0, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.True(shelf.TShelfChangeRead());
        Assert.True(shelf.CShelfImprintShown);
        Assert.False(shelf.CShelfBinEnabled);
    }

    [Fact]
    public void ShelfLeaveConfirm_StoredLeave_SavesTheSourceDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        shelf.CShelfReferenceCreate();
        shelf.CShelfImprint.CImprintTitleSet("Tome");

        Assert.True(shelf.TShelfLeaveConfirm());

        Assert.Equal(["Leave"], asked);
        Assert.False(shelf.TShelfChangeRead());
        Assert.Contains(shelf.CShelfRollRead().CShelfRollRows, row => row.CCatalogReferenceName == "Tome");
        Assert.True(shelf.CShelfColophonShown);
        Assert.True(shelf.CShelfBinEnabled);
    }

    [Fact]
    public void ShelfLeaveConfirm_NothingUnsaved_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.True(shelf.TShelfLeaveConfirm());
        Assert.Empty(asked);
    }

    [Fact]
    public void ShelfDraftSave_ChangedSourceDraft_StoresAndShowsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        shelf.CShelfReferenceCreate();
        shelf.CShelfImprint.CImprintTitleSet("Tome");

        Assert.True(shelf.CShelfStoreEnabled);
        Assert.True(shelf.CShelfChronicleRead().CShelfBackward);

        shelf.CShelfDraftUndo();

        Assert.False(shelf.CShelfChronicleRead().CShelfBackward);
        Assert.True(shelf.CShelfChronicleRead().CShelfForward);

        shelf.CShelfDraftRedo();
        shelf.CShelfDraftSave();

        Assert.False(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfColophonShown);
        Assert.Contains(
            shelf.CShelfRollRead().CShelfRollRows,
            row => row.CCatalogReferenceName == "Tome" && row.CCatalogReferenceChosen);
    }

    [Fact]
    public void ShelfDraftFinish_UnstoredSourceDraft_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        shelf.CShelfReferenceCreate();
        shelf.CShelfImprint.CImprintTitleSet("Tome");

        Assert.True(shelf.TShelfDraftFinish(false));

        Assert.False(shelf.CShelfImprint.CImprintHeld);
        Assert.False(shelf.TShelfChangeRead());
        Assert.DoesNotContain(shelf.CShelfRollRead().CShelfRollRows, row => row.CCatalogReferenceName == "Tome");
    }

    [Fact]
    public void ShelfDraftFinish_EntrySide_DropsTheEntryDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.CShelfReferenceSelect(book.LReferenceId);
        shelf.CShelfReferenceCreate();

        Assert.True(shelf.CShelfEditor.CEditorDesk.CDeskHeld);

        Assert.True(shelf.TShelfDraftFinish(false));

        Assert.False(shelf.CShelfEditor.CEditorDesk.CDeskHeld);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
    }

    [Fact]
    public void ShelfEntrySelect_KeptLeave_StaysOnTheHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        LEntry water = TShelfEntrySave(engine, "water");
        shelf.CShelfReferenceCreate();
        shelf.CShelfImprint.CImprintTitleSet("Tome");

        shelf.CShelfEntrySelect(water.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.True(shelf.CShelfImprintShown);
        Assert.False(shelf.CShelfDisplayShown);
        Assert.True(shelf.TShelfChangeRead());
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

        Assert.True(shelf.CShelfImprintShown);
        Assert.True(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfScribeChecked);

        shelf.CShelfScribeToggle(false);

        Assert.True(shelf.CShelfColophonShown);
        Assert.False(shelf.CShelfImprint.CImprintHeld);
        Assert.True(shelf.CShelfViewerChecked);
    }

    [Fact]
    public void ShelfEntryNotice_EntrySideClosed_ReshowsTheChosenSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.CShelfReferenceSelect(book.LReferenceId);
        List<CColophon> shown = [];
        shelf.CShelfColophonChanged += shown.Add;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, 0);

        Assert.Equal("Book", Assert.Single(shown).CColophonTitle);
        Assert.True(shelf.CShelfColophonShown);
    }

    [Fact]
    public void ShelfWorkspaceNotice_SourceShown_ClosesBothSides()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LReference book = engine.TEngineCitationCreate("Book");
        shelf.TShelfReferenceOpen(book.LReferenceId);

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(shelf.CShelfBinEnabled);
        Assert.False(shelf.CShelfModeEnabled);
        Assert.False(shelf.CShelfPressAllowed);
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

    private static LEntry TShelfEntrySave(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
