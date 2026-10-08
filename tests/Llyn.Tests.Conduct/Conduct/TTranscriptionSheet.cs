using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTranscriptionSheet
{
    [Fact]
    public void TranscriptionRead_TwoRows_MarksTheSchemeTheOtherRowHolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTranscription transcription = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                    TInterface.TTranscriptionDraftCreate("Yale", "hēung góng"),
                ])).TEditorFixtureTranscription;

        CTranscriptionSheet sheet = transcription.CTranscriptionRead();

        Assert.True(sheet.CTranscriptionSheetShown);
        Assert.False(sheet.CTranscriptionSheetFree);
        Assert.Equal(
            [new CScheme("Jyutping", true), new CScheme("Yale", false)],
            sheet.CTranscriptionSheetRows[1].CTranscriptionRowSchemes);
        Assert.Equal(
            [new CScheme("Jyutping", false), new CScheme("Yale", true)],
            sheet.CTranscriptionSheetRows[0].CTranscriptionRowSchemes);
    }

    [Fact]
    public void TranscriptionRead_LanguageWithoutSchemes_AnswersTheHiddenBlockWithItsRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTranscription transcription = TTranscriptionPrepare(
            engine, TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Yale", "wai")], "English"))
            .TEditorFixtureTranscription;

        CTranscriptionSheet sheet = transcription.CTranscriptionRead();

        Assert.False(sheet.CTranscriptionSheetShown);
        Assert.False(sheet.CTranscriptionSheetFree);
        CTranscriptionRow row = Assert.Single(sheet.CTranscriptionSheetRows);
        Assert.Equal("wai", row.CTranscriptionRowDraft.CTranscriptionDraftText);
        Assert.Empty(row.CTranscriptionRowSchemes);
    }

    [Fact]
    public void TranscriptionRead_EmptyDesk_AnswersTheHiddenBlockThatAddsNothing()
    {
        CTranscriptionSheet sheet = TTranscriptionEmptyPrepare().CTranscriptionRead();

        Assert.False(sheet.CTranscriptionSheetShown);
        Assert.False(sheet.CTranscriptionSheetFree);
        Assert.Empty(sheet.CTranscriptionSheetRows);
    }

    [Fact]
    public void TranscriptionAdd_OneSchemeFree_AddsABlankRowUnderItThenNoMore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTranscriptionPrepare(
            engine, TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]));
        Assert.True(editor.TEditorFixtureTranscription.CTranscriptionRead().CTranscriptionSheetFree);

        editor.TEditorFixtureTranscription.CTranscriptionAdd(0);
        editor.TEditorFixtureTranscription.CTranscriptionAdd(0);

        CTranscriptionSheet sheet = editor.TEditorFixtureTranscription.CTranscriptionRead();
        Assert.Equal(
            [("Jyutping", "hoeng1"), ("Yale", string.Empty)],
            TTranscriptionRowsRead(sheet));
        Assert.False(sheet.CTranscriptionSheetFree);
        Assert.True(editor.TEditorFixtureDesk.TDeskChangeCheck());
    }

    [Fact]
    public void TranscriptionAdd_PressedRowOrNone_PlacesTheRowAfterItOrAtTheEnd()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LTranscriptionDraft> rows =
        [
            TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1"),
            TInterface.TTranscriptionDraftCreate("Wade", "hsiang"),
        ];
        CTranscription transcription =
            TTranscriptionPrepare(engine, TTranscriptionSave(engine, rows)).TEditorFixtureTranscription;
        long jyutping = transcription.CTranscriptionRead()
            .CTranscriptionSheetRows[0].CTranscriptionRowDraft.CTranscriptionDraftId;

        transcription.CTranscriptionAdd(jyutping);

        Assert.Equal(
            ["Jyutping", "Yale", "Wade"],
            TTranscriptionRowsRead(transcription.CTranscriptionRead()).Select(static row => row.Item1));

        CTranscription other =
            TTranscriptionPrepare(engine, TTranscriptionSave(engine, rows)).TEditorFixtureTranscription;
        other.CTranscriptionAdd(0);

        Assert.Equal(
            ["Jyutping", "Wade", "Yale"],
            TTranscriptionRowsRead(other.CTranscriptionRead()).Select(static row => row.Item1));
    }

    [Fact]
    public void TranscriptionAdd_GlyphRowBeforePressedRow_PlacesTheRowRightAfterIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Hanja", "漢字"),
                    TInterface.TTranscriptionDraftCreate("Romanization", "hanja"),
                ],
                "Korean"));
        long roman = Assert.Single(editor.TEditorFixtureTranscription.CTranscriptionRead().CTranscriptionSheetRows)
            .CTranscriptionRowDraft.CTranscriptionDraftId;

        editor.TEditorFixtureTranscription.CTranscriptionAdd(roman);

        Assert.Equal(
            [("Romanization", "hanja"), ("Phonetic", string.Empty)],
            TTranscriptionRowsRead(editor.TEditorFixtureTranscription.CTranscriptionRead()));
        Assert.Equal(
            "Hanja",
            Assert.Single(editor.TEditorFixtureTimbre.CTimbreGlyphRead().CTimbreGlyphRows).CTranscriptionDraftScheme);
    }

    [Fact]
    public void TranscriptionSchemeSet_FreeSchemeOrTakenScheme_SwitchesTheRowOrRefusesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTranscription transcription = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1"),
                    TInterface.TTranscriptionDraftCreate("Wade", "hsiang"),
                ])).TEditorFixtureTranscription;
        IReadOnlyList<CTranscriptionRow> rows =
            transcription.CTranscriptionRead().CTranscriptionSheetRows;

        transcription.CTranscriptionSchemeSet(
            rows[1].CTranscriptionRowDraft.CTranscriptionDraftId, "Yale");
        transcription.CTranscriptionSchemeSet(
            rows[0].CTranscriptionRowDraft.CTranscriptionDraftId, "Yale");

        CTranscriptionSheet sheet = transcription.CTranscriptionRead();
        Assert.Equal([("Jyutping", "hoeng1"), ("Yale", "hsiang")], TTranscriptionRowsRead(sheet));
        Assert.False(sheet.CTranscriptionSheetFree);
    }

    [Fact]
    public void TranscriptionRemove_HeldRow_DropsItAndFreesItsScheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1"),
                    TInterface.TTranscriptionDraftCreate("Yale", "hēung"),
                ]));
        long yale = editor.TEditorFixtureTranscription.CTranscriptionRead()
            .CTranscriptionSheetRows[1].CTranscriptionRowDraft.CTranscriptionDraftId;

        editor.TEditorFixtureTranscription.CTranscriptionRemove(yale);

        CTranscriptionSheet sheet = editor.TEditorFixtureTranscription.CTranscriptionRead();
        Assert.Equal([("Jyutping", "hoeng1")], TTranscriptionRowsRead(sheet));
        Assert.True(sheet.CTranscriptionSheetFree);
        Assert.True(editor.TEditorFixtureDesk.TDeskChangeCheck());
    }

    [Fact]
    public void TranscriptionAdd_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        CTranscription transcription = editor.TEditorFixtureTranscription;
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftFilling;
            transcription.CTranscriptionAdd(0);
        };

        editor.TEditorFixtureOpen(entry);

        Assert.True(filling);
        Assert.Equal([("Jyutping", "hoeng1")], TTranscriptionRowsRead(transcription.CTranscriptionRead()));
    }

    [Fact]
    public void TranscriptionSchemeSet_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        CTranscription transcription = editor.TEditorFixtureTranscription;
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftFilling;
            foreach (CTranscriptionRow row in transcription.CTranscriptionRead().CTranscriptionSheetRows)
            {
                transcription.CTranscriptionSchemeSet(
                    row.CTranscriptionRowDraft.CTranscriptionDraftId, "Yale");
            }
        };

        editor.TEditorFixtureOpen(entry);

        Assert.True(filling);
        Assert.Equal([("Jyutping", "hoeng1")], TTranscriptionRowsRead(transcription.CTranscriptionRead()));
    }

    [Fact]
    public void TranscriptionRemove_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        CTranscription transcription = editor.TEditorFixtureTranscription;
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftFilling;
            foreach (CTranscriptionRow row in transcription.CTranscriptionRead().CTranscriptionSheetRows)
            {
                transcription.CTranscriptionRemove(row.CTranscriptionRowDraft.CTranscriptionDraftId);
            }
        };

        editor.TEditorFixtureOpen(entry);

        Assert.True(filling);
        Assert.Equal([("Jyutping", "hoeng1")], TTranscriptionRowsRead(transcription.CTranscriptionRead()));
    }

    private static IReadOnlyList<(string, string)> TTranscriptionRowsRead(CTranscriptionSheet sheet)
    {
        return sheet.CTranscriptionSheetRows
            .Select(static row => (row.CTranscriptionRowDraft.CTranscriptionDraftScheme,
                row.CTranscriptionRowDraft.CTranscriptionDraftText))
            .ToList();
    }

    private static CTranscription TTranscriptionEmptyPrepare()
    {
        return new TEditorFixture(TInterfaceEditor.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TInterfaceConduct.TEntryBundleCreate([]),
                TInterfaceConduct.TPhonologyBundleCreate([]),
                TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
                {
                    ["add_LEngineFoldChanged"] = _ => null,
                    ["remove_LEngineFoldChanged"] = _ => null,
                }),
                TEngineFake.TEngineStubCreate<LMediaPort>()))
            .TEditorFixtureTranscription;
    }

    private static TEditorFixture TTranscriptionPrepare(LEngine engine, long entry)
    {
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(entry);
        return editor;
    }

    private static long TTranscriptionSave(
        LEngine engine, IReadOnlyList<LTranscriptionDraft> rows, string language = "Cantonese")
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "香港",
            language,
            string.Empty,
            string.Empty,
            [],
            [],
            transcriptions: rows)).LEntryId;
    }
}
