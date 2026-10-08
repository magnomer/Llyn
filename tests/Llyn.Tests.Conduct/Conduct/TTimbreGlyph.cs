using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTimbreGlyph
{
    [Fact]
    public void TimbreGlyphRead_KoreanDraft_SplitsTheRowsByTheGlyphScheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreGlyphPrepare(engine, TTimbreGlyphSave(engine, "Korean"));

        CTimbreGlyph glyph = editor.CEditorTimbre.CTimbreGlyphRead();

        Assert.True(glyph.CTimbreGlyphShown);
        Assert.True(glyph.CTimbreGlyphSourced);
        CTranscriptionDraft row = Assert.Single(glyph.CTimbreGlyphRows);
        Assert.Equal(("Hanja", "漢字"), (row.CTranscriptionDraftScheme, row.CTranscriptionDraftText));
        Assert.Equal(
            [("Revised Romanization", "hanja"), ("Yale", string.Empty)],
            editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows.Select(
                static other => (other.CTranscriptionRowDraft.CTranscriptionDraftScheme,
                    other.CTranscriptionRowDraft.CTranscriptionDraftText)));
    }

    [Fact]
    public void TimbreGlyphRead_LanguageWithoutGlyphOrEmptyDesk_AnswersTheHiddenBlock()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbreGlyph blank = TTimbre.TTimbrePrepare([]).CTimbreGlyphRead();
        CEditor editor = TTimbreGlyphPrepare(engine, TTimbreGlyphSave(engine, "English"));

        CTimbreGlyph glyph = editor.CEditorTimbre.CTimbreGlyphRead();

        Assert.False(blank.CTimbreGlyphShown);
        Assert.False(blank.CTimbreGlyphSourced);
        Assert.Empty(blank.CTimbreGlyphRows);
        Assert.False(glyph.CTimbreGlyphShown);
        Assert.Empty(glyph.CTimbreGlyphRows);
        Assert.Equal(
            ["Revised Romanization", "Hanja", "Yale"],
            editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows.Select(
                static row => row.CTranscriptionRowDraft.CTranscriptionDraftScheme));
    }

    [Fact]
    public void TranscriptionSet_GlyphRow_WritesTheTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreGlyphPrepare(engine, TTimbreGlyphSave(engine, "Korean"));
        long glyph = Assert.Single(editor.CEditorTimbre.CTimbreGlyphRead().CTimbreGlyphRows).CTranscriptionDraftId;

        editor.CEditorTranscription.CTranscriptionSet(glyph, "韓字");
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();

        CTimbreGlyph written = editor.CEditorTimbre.CTimbreGlyphRead();
        Assert.Equal("韓字", Assert.Single(written.CTimbreGlyphRows).CTranscriptionDraftText);
        Assert.Equal(
            ["hanja", string.Empty],
            editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows.Select(
                static row => row.CTranscriptionRowDraft.CTranscriptionDraftText));
        Assert.True(editor.CEditorDesk.TDeskChangeCheck());
    }

    [Fact]
    public void TranscriptionSet_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTimbreGlyphSave(engine, "Korean");
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.CEditorDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskDraft.CDeskDraftFilling;
            foreach (CTranscriptionDraft row in editor.CEditorTimbre.CTimbreGlyphRead().CTimbreGlyphRows)
            {
                editor.CEditorTranscription.CTranscriptionSet(row.CTranscriptionDraftId, "韓字");
            }
        };

        editor.CEditorEntryOpen(entry);
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();

        Assert.True(filling);
        Assert.Equal(
            "漢字", Assert.Single(editor.CEditorTimbre.CTimbreGlyphRead().CTimbreGlyphRows).CTranscriptionDraftText);
    }

    private static CEditor TTimbreGlyphPrepare(LEngine engine, long entry)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        return editor;
    }

    private static long TTimbreGlyphSave(LEngine engine, string language)
    {
        IReadOnlyList<LTranscriptionDraft> rows =
        [
            TInterface.TTranscriptionDraftCreate("Revised Romanization", "hanja"),
            TInterface.TTranscriptionDraftCreate("Hanja", "漢字"),
            TInterface.TTranscriptionDraftCreate("Yale", string.Empty),
        ];
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "한자",
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a character", 1)],
            [],
            transcriptions: rows)).LEntryId;
    }
}
