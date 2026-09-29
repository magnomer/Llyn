using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSoundingAnchor
{
    private const string TSoundingAnchorPack =
        """
        { "fanqie": [
            { "name": "book", "url": "https://example.test/book/{word}", "match": "{word}" } ] }
        """;

    [Fact]
    public void SoundingAnchorOpen_AnchoredReflex_MarksTheHeldReading()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TSoundingAnchorPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (long entry, long fanqie) = TSoundingAnchorPlace(engine, workspace, pack.TLanguageFixtureName);
        CEditor editor = TSoundingAnchorPrepare(engine, entry);
        CSoundingAnchor anchor = CSoundingAnchor.CSoundingAnchorCreate(editor);
        long reflex = editor.CEditorDraftRead()!.CEntryDraftReflexes.Single().CReflexDraftId;

        CAnchor menu = anchor.CSoundingAnchorOpen(reflex);
        CAnchor unknown = anchor.CSoundingAnchorOpen(reflex + 1000);

        CAnchorRow row = Assert.Single(menu.CAnchorRows);
        Assert.Equal(fanqie, row.CAnchorRowId);
        Assert.True(row.CAnchorRowHeld);
        Assert.False(menu.CAnchorEmpty);
        Assert.Empty(unknown.CAnchorRows);
        Assert.True(unknown.CAnchorEmpty);
    }

    [Fact]
    public void SoundingAnchorOpen_FreshDraft_AnswersTheEmptyMenu()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingAnchorPrepare(engine, null);
        CSoundingAnchor anchor = CSoundingAnchor.CSoundingAnchorCreate(editor);

        CAnchor menu = anchor.CSoundingAnchorOpen(7);

        Assert.Empty(menu.CAnchorRows);
        Assert.True(menu.CAnchorEmpty);
    }

    [Fact]
    public void SoundingAnchorSet_OpenedReflex_TiesAndUntiesTheReadingInTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingAnchorPrepare(engine, TSoundingAnchorSave(engine));
        CSoundingAnchor anchor = CSoundingAnchor.CSoundingAnchorCreate(editor);
        long reflex = editor.CEditorDraftRead()!.CEntryDraftReflexes.Single().CReflexDraftId;
        anchor.CSoundingAnchorOpen(reflex);

        anchor.CSoundingAnchorSet(7, true);
        IReadOnlyList<long> tied = editor.CEditorDraftRead()!.CEntryDraftReflexes.Single().CReflexDraftAnchors;
        anchor.CSoundingAnchorSet(7, false);

        Assert.Equal([7L], tied);
        Assert.Empty(editor.CEditorDraftRead()!.CEntryDraftReflexes.Single().CReflexDraftAnchors);
    }

    [Fact]
    public void SoundingAnchorClose_ClosedMenu_LeavesTheDraftAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingAnchorPrepare(engine, TSoundingAnchorSave(engine));
        CSoundingAnchor anchor = CSoundingAnchor.CSoundingAnchorCreate(editor);
        long reflex = editor.CEditorDraftRead()!.CEntryDraftReflexes.Single().CReflexDraftId;
        anchor.CSoundingAnchorOpen(reflex);

        anchor.CSoundingAnchorClose();
        anchor.CSoundingAnchorSet(7, true);

        Assert.Empty(editor.CEditorDraftRead()!.CEntryDraftReflexes.Single().CReflexDraftAnchors);
        Assert.False(editor.CEditorDesk.CDeskChanged);
    }

    private static (long, long) TSoundingAnchorPlace(LEngine engine, TWorkspace workspace, string language)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "爛",
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: [TInterface.TReflexDraftCreate("Korean", "", "란")]));

        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, "爛", [TInterface.TFanqieRowCreate("爛", 0, "來", "寒", "一", "去")]);
        long id = fanqie.TFanqieRead(language, "爛").Single().LFanqieRowId;
        engine.TEntryAnchorApply(entry.LEntryId, [id]);
        return (entry.LEntryId, id);
    }

    private static long TSoundingAnchorSave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            "ˈwɔːtə",
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1)],
            [],
            reflexes: [TInterface.TReflexDraftCreate("Korean", "", "워터")])).LEntryId;
    }

    private static CEditor TSoundingAnchorPrepare(LEngine engine, long? entry)
    {
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        return editor;
    }
}
