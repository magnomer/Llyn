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
        TEditorFixture editor = TTimbreGlyphPrepare(engine, TTimbreGlyphSave(engine, "Korean"));

        CTimbreGlyph glyph = editor.TEditorFixtureTimbre.CTimbreGlyphRead();

        Assert.True(glyph.CTimbreGlyphShown);
        Assert.True(glyph.CTimbreGlyphSourced);
        CTranscriptionDraft row = Assert.Single(glyph.CTimbreGlyphRows);
        Assert.Equal(("Hanja", "漢字"), (row.CTranscriptionDraftScheme, row.CTranscriptionDraftText));
        Assert.Equal(
            [("Revised Romanization", "hanja"), ("Yale", string.Empty)],
            editor.TEditorFixtureTranscription.CTranscriptionRead().CTranscriptionSheetRows.Select(
                static other => (other.CTranscriptionRowDraft.CTranscriptionDraftScheme,
                    other.CTranscriptionRowDraft.CTranscriptionDraftText)));
    }

    [Fact]
    public void TimbreGlyphRead_LanguageWithoutGlyphOrEmptyDesk_AnswersTheHiddenBlock()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbreGlyph blank = TTimbre.TTimbrePrepare([]).CTimbreGlyphRead();
        TEditorFixture editor = TTimbreGlyphPrepare(engine, TTimbreGlyphSave(engine, "English"));

        CTimbreGlyph glyph = editor.TEditorFixtureTimbre.CTimbreGlyphRead();

        Assert.False(blank.CTimbreGlyphShown);
        Assert.False(blank.CTimbreGlyphSourced);
        Assert.Empty(blank.CTimbreGlyphRows);
        Assert.False(glyph.CTimbreGlyphShown);
        Assert.Empty(glyph.CTimbreGlyphRows);
        Assert.Equal(
            ["Revised Romanization", "Hanja", "Yale"],
            editor.TEditorFixtureTranscription.CTranscriptionRead().CTranscriptionSheetRows.Select(
                static row => row.CTranscriptionRowDraft.CTranscriptionDraftScheme));
    }

    [Fact]
    public void TranscriptionSet_GlyphRow_WritesTheTypedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTimbreGlyphPrepare(engine, TTimbreGlyphSave(engine, "Korean"));
        CTimbre timbre = editor.TEditorFixtureTimbre;
        long glyph = Assert.Single(timbre.CTimbreGlyphRead().CTimbreGlyphRows).CTranscriptionDraftId;

        editor.TEditorFixtureTranscription.CTranscriptionSet(glyph, "韓字");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        CTimbreGlyph written = timbre.CTimbreGlyphRead();
        Assert.Equal("韓字", Assert.Single(written.CTimbreGlyphRows).CTranscriptionDraftText);
        Assert.Equal(
            ["hanja", string.Empty],
            editor.TEditorFixtureTranscription.CTranscriptionRead().CTranscriptionSheetRows.Select(
                static row => row.CTranscriptionRowDraft.CTranscriptionDraftText));
        Assert.True(editor.TEditorFixtureDesk.TDeskChangeCheck());
    }

    [Fact]
    public void TranscriptionSet_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTimbreGlyphSave(engine, "Korean");
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftFilling;
            foreach (CTranscriptionDraft row in editor.TEditorFixtureTimbre.CTimbreGlyphRead().CTimbreGlyphRows)
            {
                editor.TEditorFixtureTranscription.CTranscriptionSet(row.CTranscriptionDraftId, "韓字");
            }
        };

        editor.TEditorFixtureOpen(entry);
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.True(filling);
        Assert.Equal(
            "漢字",
            Assert.Single(editor.TEditorFixtureTimbre.CTimbreGlyphRead().CTimbreGlyphRows).CTranscriptionDraftText);
    }

    private static TEditorFixture TTimbreGlyphPrepare(LEngine engine, long entry)
    {
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(entry);
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
