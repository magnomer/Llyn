using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TShelfDraft
{
    [Fact]
    public void ShelfReferenceSelect_KeptLeave_StaysOnTheHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.True(shelf.TShelfLeaveConfirm());
        Assert.Empty(asked);
    }

    [Fact]
    public void ShelfDraftSave_ChangedSourceDraft_StoresAndShowsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
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
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        LEntry water = TShelf.TShelfEntrySave(engine, "water");
        shelf.CShelfReferenceCreate();
        shelf.CShelfImprint.CImprintTitleSet("Tome");

        shelf.CShelfEntrySelect(water.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.True(shelf.CShelfImprintShown);
        Assert.False(shelf.CShelfDisplayShown);
        Assert.True(shelf.TShelfChangeRead());
    }
}
