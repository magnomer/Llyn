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
        CEditor editor = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                    TInterface.TTranscriptionDraftCreate("Yale", "hēung góng"),
                ]));

        CTranscriptionSheet sheet = editor.CEditorTranscription.CTranscriptionRead();

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
        CEditor editor = TTranscriptionPrepare(
            engine, TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Yale", "wai")], "English"));

        CTranscriptionSheet sheet = editor.CEditorTranscription.CTranscriptionRead();

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
        CEditor editor = TTranscriptionPrepare(
            engine, TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]));
        Assert.True(editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetFree);

        editor.CEditorTranscription.CTranscriptionAdd(0);
        editor.CEditorTranscription.CTranscriptionAdd(0);

        CTranscriptionSheet sheet = editor.CEditorTranscription.CTranscriptionRead();
        Assert.Equal(
            [("Jyutping", "hoeng1"), ("Yale", string.Empty)],
            TTranscriptionRowsRead(sheet));
        Assert.False(sheet.CTranscriptionSheetFree);
        Assert.True(editor.CEditorDesk.TDeskChangeCheck());
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
        CEditor editor = TTranscriptionPrepare(engine, TTranscriptionSave(engine, rows));
        long jyutping = editor.CEditorTranscription.CTranscriptionRead()
            .CTranscriptionSheetRows[0].CTranscriptionRowDraft.CTranscriptionDraftId;

        editor.CEditorTranscription.CTranscriptionAdd(jyutping);

        Assert.Equal(
            ["Jyutping", "Yale", "Wade"],
            TTranscriptionRowsRead(editor.CEditorTranscription.CTranscriptionRead()).Select(static row => row.Item1));

        CEditor other = TTranscriptionPrepare(engine, TTranscriptionSave(engine, rows));
        other.CEditorTranscription.CTranscriptionAdd(0);

        Assert.Equal(
            ["Jyutping", "Wade", "Yale"],
            TTranscriptionRowsRead(other.CEditorTranscription.CTranscriptionRead()).Select(static row => row.Item1));
    }

    [Fact]
    public void TranscriptionAdd_GlyphRowBeforePressedRow_PlacesTheRowRightAfterIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Hanja", "漢字"),
                    TInterface.TTranscriptionDraftCreate("Romanization", "hanja"),
                ],
                "Korean"));
        long roman = Assert.Single(editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows)
            .CTranscriptionRowDraft.CTranscriptionDraftId;

        editor.CEditorTranscription.CTranscriptionAdd(roman);

        Assert.Equal(
            [("Romanization", "hanja"), ("Phonetic", string.Empty)],
            TTranscriptionRowsRead(editor.CEditorTranscription.CTranscriptionRead()));
        Assert.Equal(
            "Hanja", Assert.Single(editor.CEditorTimbre.CTimbreGlyphRead().CTimbreGlyphRows).CTranscriptionDraftScheme);
    }

    [Fact]
    public void TranscriptionSchemeSet_FreeSchemeOrTakenScheme_SwitchesTheRowOrRefusesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1"),
                    TInterface.TTranscriptionDraftCreate("Wade", "hsiang"),
                ]));
        IReadOnlyList<CTranscriptionRow> rows =
            editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows;

        editor.CEditorTranscription.CTranscriptionSchemeSet(
            rows[1].CTranscriptionRowDraft.CTranscriptionDraftId, "Yale");
        editor.CEditorTranscription.CTranscriptionSchemeSet(
            rows[0].CTranscriptionRowDraft.CTranscriptionDraftId, "Yale");

        CTranscriptionSheet sheet = editor.CEditorTranscription.CTranscriptionRead();
        Assert.Equal([("Jyutping", "hoeng1"), ("Yale", "hsiang")], TTranscriptionRowsRead(sheet));
        Assert.False(sheet.CTranscriptionSheetFree);
    }

    [Fact]
    public void TranscriptionRemove_HeldRow_DropsItAndFreesItsScheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTranscriptionPrepare(
            engine,
            TTranscriptionSave(
                engine,
                [
                    TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1"),
                    TInterface.TTranscriptionDraftCreate("Yale", "hēung"),
                ]));
        long yale = editor.CEditorTranscription.CTranscriptionRead()
            .CTranscriptionSheetRows[1].CTranscriptionRowDraft.CTranscriptionDraftId;

        editor.CEditorTranscription.CTranscriptionRemove(yale);

        CTranscriptionSheet sheet = editor.CEditorTranscription.CTranscriptionRead();
        Assert.Equal([("Jyutping", "hoeng1")], TTranscriptionRowsRead(sheet));
        Assert.True(sheet.CTranscriptionSheetFree);
        Assert.True(editor.CEditorDesk.TDeskChangeCheck());
    }

    [Fact]
    public void TranscriptionAdd_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]);
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskFilling;
            editor.CEditorTranscription.CTranscriptionAdd(0);
        };

        editor.CEditorEntryOpen(entry);

        Assert.True(filling);
        Assert.Equal(
            [("Jyutping", "hoeng1")], TTranscriptionRowsRead(editor.CEditorTranscription.CTranscriptionRead()));
    }

    [Fact]
    public void TranscriptionSchemeSet_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]);
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskFilling;
            foreach (CTranscriptionRow row in editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows)
            {
                editor.CEditorTranscription.CTranscriptionSchemeSet(
                    row.CTranscriptionRowDraft.CTranscriptionDraftId, "Yale");
            }
        };

        editor.CEditorEntryOpen(entry);

        Assert.True(filling);
        Assert.Equal(
            [("Jyutping", "hoeng1")], TTranscriptionRowsRead(editor.CEditorTranscription.CTranscriptionRead()));
    }

    [Fact]
    public void TranscriptionRemove_FillingDesk_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTranscriptionSave(engine, [TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1")]);
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        bool filling = false;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskFilling;
            foreach (CTranscriptionRow row in editor.CEditorTranscription.CTranscriptionRead().CTranscriptionSheetRows)
            {
                editor.CEditorTranscription.CTranscriptionRemove(row.CTranscriptionRowDraft.CTranscriptionDraftId);
            }
        };

        editor.CEditorEntryOpen(entry);

        Assert.True(filling);
        Assert.Equal(
            [("Jyutping", "hoeng1")], TTranscriptionRowsRead(editor.CEditorTranscription.CTranscriptionRead()));
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
        return TInterfaceEditor.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TInterfaceConduct.TEntryBundleCreate([]),
                TInterfaceConduct.TPhonologyBundleCreate([]),
                TEngineFake.TEngineStubCreate<LSettingsPort>(),
                TEngineFake.TEngineStubCreate<LMediaPort>())
            .CEditorTranscription;
    }

    private static CEditor TTranscriptionPrepare(LEngine engine, long entry)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
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
