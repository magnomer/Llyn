using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TShelfVista
{
    [Fact]
    public void ShelfVistaRestore_QueriesHeld_CarriesThemIntoTheFreshVistas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Book");
        engine.TEngineCitationCreate("Tome");
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        shelf.CShelfQuerySet("Tom");
        shelf.CShelfFootnote.CFootnoteQuerySet("zzz");

        shelf.CShelfVistaRestore();

        Assert.Equal(["Tome"], shelf.CShelfRollRead().CShelfRollRows.Select(row => row.CCatalogReferenceName));
        Assert.Equal("Source.Unmatched", shelf.CShelfFootnote.CFootnoteEmptyKey);
    }

    [Fact]
    public void ShelfVistaRestore_ReferenceNoticeAfterASecondRestore_RaisesTheShelfOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        int marshalled = 0;
        CShelf shelf = TShelfMarshalCreate(atelier, () => marshalled++);
        shelf.CShelfVistaRestore();
        shelf.CShelfVistaRestore();
        marshalled = 0;
        int rows = 0;
        int footnotes = 0;
        shelf.CShelfPanel.CPanelRowsChanged += () => rows++;
        shelf.CShelfFootnote.CFootnotePanel.CPanelRowsChanged += () => footnotes++;

        engine.TEngineBulletinRaise(LSubject.LSubjectReference, book.LReferenceId);

        Assert.Equal(1, rows);
        Assert.Equal(1, footnotes);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void ShelfVistaRestore_EntryNotice_RaisesTheShelfRowsForTheirTallies()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry hearth = TShelfEntrySave(engine, "hearth");
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int rows = 0;
        shelf.CShelfPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, hearth.LEntryId);

        Assert.Equal(1, rows);
    }

    [Fact]
    public void ShelfVistaRestore_EntryNotice_RaisesTheFootnoteRowsThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry hearth = TShelfEntrySave(engine, "hearth");
        int marshalled = 0;
        CShelf shelf = TShelfMarshalCreate(atelier, () => marshalled++);
        shelf.CShelfVistaRestore();
        int footnotes = 0;
        shelf.CShelfFootnote.CFootnotePanel.CPanelRowsChanged += () => footnotes++;
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, hearth.LEntryId);

        Assert.True(footnotes > 0);
        Assert.True(marshalled > 0);
    }

    [Fact]
    public void ShelfVistaRestore_SettingsNotice_RaisesTheShelfRowsThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CShelf shelf = TShelfMarshalCreate(atelier, () => marshalled++);
        shelf.CShelfVistaRestore();
        int rows = 0;
        shelf.CShelfPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void ShelfVistaRestore_WorkspaceNotice_ClosesTheChosenReferenceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        int marshalled = 0;
        CShelf shelf = TShelfMarshalCreate(atelier, () => marshalled++);
        shelf.CShelfVistaRestore();
        shelf.CShelfReferenceSelect(book.LReferenceId);
        Assert.True(shelf.CShelfBinEnabled);
        marshalled = 0;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(shelf.CShelfBinEnabled);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void AtelierClose_EntryEditorOpen_CancelsTheEditorDeskAndStopsPlayback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(
            new Dictionary<string, System.Func<object?[]?, object?>>
            {
                ["LEngineRecordingStop"] = _ =>
                {
                    stopped++;
                    return null;
                },
            });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        LEntry hearth = TShelfEntrySave(engine, "hearth");
        CShelf shelf = TShelf.TShelfPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        shelf.CShelfEntrySelect(hearth.LEntryId);
        shelf.CShelfScribeToggle(true);
        Assert.True(shelf.CShelfEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(shelf.CShelfEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }

    private static CShelf TShelfMarshalCreate(CAtelier atelier, System.Action counted)
    {
        return CShelf.CShelfCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                counted();
                run();
            });
    }

    private static LEntry TShelfEntrySave(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
