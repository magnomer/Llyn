using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TKindred
{
    [Fact]
    public void KindredRead_StoredEntry_AnswersEveryRowWithItsAnchorAndTheFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(
            engine,
            TKindredSave(
                engine,
                "kindle",
                "English",
                [
                    TInterface.TReflexDraftCreate("Korean", string.Empty, "a"),
                    TInterface.TReflexDraftCreate("Wu", string.Empty, string.Empty),
                ]));
        List<object?[]?> checks = [];
        List<object?[]?> formats = [];
        CKindred kindred = TInterfaceConductSound.TKindredCreate(
            editor,
            TKindredGuisePrepare(),
            TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineAnchorCheck"] = args =>
                {
                    checks.Add(args);
                    return true;
                },
                ["LEngineAnchorFormat"] = args =>
                {
                    formats.Add(args);
                    return "Guangyun";
                },
            }));
        editor.CEditorDisplay.CDisplaySound.CDisplayReflexToggle(true);

        CTimbreReflex reflex = kindred.CKindredRead();

        Assert.True(reflex.CTimbreReflexShown);
        Assert.Equal(["a", string.Empty], reflex.CTimbreReflexRows.Select(static row => row.CReflexText));
        Assert.Equal([true, true], reflex.CTimbreReflexRows.Select(static row => row.CReflexFolded));
        Assert.True(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Equal(
            reflex.CTimbreReflexRows.Select(static row => row.CReflexId),
            reflex.CTimbreReflexAnchor.CLecternAnchorTexts.Keys.Order());
        Assert.All(
            reflex.CTimbreReflexAnchor.CLecternAnchorTexts.Values, static text => Assert.Equal("Guangyun", text));
        long? stored = editor.CEditorDesk.CDeskStoredRead();
        Assert.Equal(new object?[] { stored, "kindle" }, Assert.Single(checks));
        Assert.Equal(2, formats.Count);
        Assert.All(
            formats,
            format => Assert.Equal(
                new object?[] { stored, "kindle", CReflex.LReflexSeparator },
                new[] { format![0], format[2], format[3] }));
        Assert.True(reflex.CTimbreReflexOpened);
        Assert.False(reflex.CTimbreReflexPending);
    }

    [Fact]
    public void KindredRead_EmptyDesk_AnswersNoRowsAndNoAnchor()
    {
        CTimbreReflex reflex = TTimbre.TTimbreEditorPrepare([]).CEditorKindred.CKindredRead();

        Assert.False(reflex.CTimbreReflexShown);
        Assert.Empty(reflex.CTimbreReflexRows);
        Assert.False(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Empty(reflex.CTimbreReflexAnchor.CLecternAnchorTexts);
        Assert.False(reflex.CTimbreReflexOpened);
        Assert.False(reflex.CTimbreReflexPending);
    }

    [Fact]
    public void KindredRead_FreshDraft_OffersNoAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, null);
        CKindred kindred = TInterfaceConductSound.TKindredCreate(
            editor, TKindredGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());

        CTimbreReflex reflex = kindred.CKindredRead();

        Assert.Empty(reflex.CTimbreReflexRows);
        Assert.False(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Empty(reflex.CTimbreReflexAnchor.CLecternAnchorTexts);
    }

    [Fact]
    public void KindredRead_RefusedAnchor_AnswersTheRowsUnanchored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(
            engine,
            TKindredSave(
                engine, "water", "English", [TInterface.TReflexDraftCreate("Korean", string.Empty, "a")]));
        CKindred kindred = TInterfaceConductSound.TKindredCreate(
            editor, TKindredGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());

        CTimbreReflex reflex = kindred.CKindredRead();

        Assert.Equal("a", Assert.Single(reflex.CTimbreReflexRows).CReflexText);
        Assert.False(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Empty(reflex.CTimbreReflexAnchor.CLecternAnchorTexts);
    }

    [Fact]
    public async Task KindredStart_StoredEntryWithoutReflexes_StartsTheLookupWhenTheDraftOpens()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(string.Empty, gate.Task));
        long entry = TKindredSave(engine, "弄", pack.TLanguageFixtureName, []);

        CEditor editor = TKindredPrepare(engine, entry);

        Assert.True(editor.CEditorKindred.CKindredPending);
        Assert.True(editor.CEditorKindred.CKindredRead().CTimbreReflexPending);
        gate.SetResult();
        await TReflexFixture.TReflexSettle(engine, entry);
    }

    [Fact]
    public void KindredStart_ReflectedOrFreshDraft_StartsNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(string.Empty, gate.Task));
        long entry = TKindredSave(
            engine, "弄", pack.TLanguageFixtureName, [TInterface.TReflexDraftCreate("Korean", string.Empty, "롱")]);

        CEditor reflected = TKindredPrepare(engine, entry);
        CEditor fresh = TKindredPrepare(engine, null);
        fresh.CEditorEntry.CEntryLanguageSet(pack.TLanguageFixtureName);
        fresh.CEditorEntry.CEntryHeadwordSet("弄");

        Assert.False(reflected.CEditorKindred.CKindredPending);
        Assert.False(engine.TEngineReflexCheck(entry));
        Assert.False(fresh.CEditorKindred.CKindredRead().CTimbreReflexPending);
        gate.SetResult();
    }

    [Fact]
    public void KindredAdd_PressedRow_PlacesARowOfItsLanguageAndKindBelowIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));

        editor.CEditorKindred.CKindredAdd(
            TKindredRowsRead(editor).First(static row => row.CReflexText == "sy").CReflexId);

        IReadOnlyList<CReflex> rows = TKindredRowsRead(editor);
        Assert.Equal(["Jin", "Wu", "Wu", "Wu"], rows.Select(static row => row.CReflexLanguage));
        Assert.Equal([string.Empty, string.Empty, "Go-on", "Go-on"], rows.Select(static row => row.CReflexKind));
        Assert.Equal(["sui", "si", "sy", string.Empty], rows.Select(static row => row.CReflexText));
    }

    [Fact]
    public void KindredAdd_NoOrGoneRow_AppendsABlankRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));

        editor.CEditorKindred.CKindredAdd(0);
        editor.CEditorKindred.CKindredAdd(long.MaxValue);

        IReadOnlyList<CReflex> rows = TKindredRowsRead(editor);
        Assert.Equal(
            ["Jin", "Wu", "Wu", string.Empty, string.Empty], rows.Select(static row => row.CReflexLanguage));
        Assert.Equal(["sui", "si", "sy", string.Empty, string.Empty], rows.Select(static row => row.CReflexText));
    }

    [Fact]
    public void KindredRemove_HeldRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));

        editor.CEditorKindred.CKindredRemove(TKindredRowsRead(editor)[1].CReflexId);

        Assert.Equal(["sui", "sy"], TKindredRowsRead(editor).Select(static row => row.CReflexText));
    }

    [Fact]
    public void KindredToggle_HeldRow_FlipsItsMainEachTime()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));
        long second = TKindredRowsRead(editor)[1].CReflexId;

        editor.CEditorKindred.CKindredToggle(second);
        Assert.Equal([false, true, false], TKindredRowsRead(editor).Select(static row => row.CReflexMain));

        editor.CEditorKindred.CKindredToggle(second);
        editor.CEditorKindred.CKindredToggle(long.MaxValue);
        Assert.Equal([false, false, false], TKindredRowsRead(editor).Select(static row => row.CReflexMain));
    }

    [Fact]
    public void KindredSet_TypedLanguage_WritesItAndLeadsByTheOverlaidRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TKindredSave(engine);
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        CReflexTyped refused = editor.CEditorKindred.CKindredSet(1, CReflexField.CReflexFieldLanguage, "Wu");
        Assert.Equal(CReflexField.CReflexFieldLanguage, refused.CReflexTypedField);
        Assert.Equal(string.Empty, refused.CReflexTypedText);
        Assert.Empty(refused.CReflexTypedHeads);

        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        IReadOnlyList<long> ids = TKindredRowsRead(editor).Select(static row => row.CReflexId).ToList();

        Assert.Equal(
            [new CReflexHead(ids[0], true), new CReflexHead(ids[1], true), new CReflexHead(ids[2], false)],
            editor.CEditorKindred.CKindredSet(ids[1], CReflexField.CReflexFieldLanguage, "Wu").CReflexTypedHeads);
        CReflexTyped typed = editor.CEditorKindred.CKindredSet(ids[1], CReflexField.CReflexFieldLanguage, "Jin");
        Assert.Equal(
            [new CReflexHead(ids[0], true), new CReflexHead(ids[1], false), new CReflexHead(ids[2], true)],
            typed.CReflexTypedHeads);
        Assert.Equal("Jin", typed.CReflexTypedText);
        Assert.Equal("Reflex.Jin", typed.CReflexTypedKey);
        Assert.Equal(["Jin", "Jin", "Wu"], TKindredRowsRead(editor).Select(static row => row.CReflexLanguage));
    }

    [Fact]
    public void KindredSet_TextOfARespelledRow_WritesTheRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));
        long first = TKindredRowsRead(editor)[0].CReflexId;
        CKindred respelled = TInterfaceConductSound.TKindredCreate(
            editor, TKindredGuisePrepare(true), TEngineFake.TEngineStubCreate<LDraftPort>());

        Assert.Empty(respelled.CKindredSet(first, CReflexField.CReflexFieldText, "sü").CReflexTypedHeads);

        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();
        Assert.Equal("sü", respelled.CKindredRead().CTimbreReflexRows[0].CReflexText);
        Assert.Equal("sui", TKindredRowsRead(editor)[0].CReflexText);
    }

    [Fact]
    public void KindredSet_TextOfAPlainRow_WritesThePhoneticText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));
        long first = TKindredRowsRead(editor)[0].CReflexId;
        CKindred plain = TInterfaceConductSound.TKindredCreate(
            editor, TKindredGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());

        Assert.Empty(plain.CKindredSet(first, CReflexField.CReflexFieldText, "sɿ").CReflexTypedHeads);

        Assert.Equal("sɿ", TKindredRowsRead(editor)[0].CReflexText);
    }

    [Fact]
    public void KindredSet_OtherCells_WriteEachAndAnswerNoLeads()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));
        long first = TKindredRowsRead(editor)[0].CReflexId;
        CKindred kindred = editor.CEditorKindred;

        IReadOnlyList<CReflexTyped> typed =
        [
            kindred.CKindredSet(first, CReflexField.CReflexFieldKind, "Kan-on"),
            kindred.CKindredSet(first, CReflexField.CReflexFieldRomanization, "si"),
            kindred.CKindredSet(first, CReflexField.CReflexFieldMeaning, "water"),
            kindred.CKindredSet(first, CReflexField.CReflexFieldNote, "literary"),
        ];

        Assert.All(typed, static answer => Assert.Empty(answer.CReflexTypedHeads));
        Assert.Equal(
            ["Kan-on", "si", "water", "literary"], typed.Select(static answer => answer.CReflexTypedText));
        Assert.Equal(
            [
                CReflexField.CReflexFieldKind,
                CReflexField.CReflexFieldRomanization,
                CReflexField.CReflexFieldMeaning,
                CReflexField.CReflexFieldNote,
            ],
            typed.Select(static answer => answer.CReflexTypedField));

        CReflex row = TKindredRowsRead(editor)[0];
        Assert.Equal(
            ["Kan-on", "si", "water", "literary"],
            new[] { row.CReflexKind, row.CReflexRomanization, row.CReflexMeaning, row.CReflexNote });
    }

    [Fact]
    public void KindredSet_WhileTheDeskFills_TakesNothingAndAnswersTheHeldText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TKindredPrepare(engine, TKindredSave(engine));
        long first = TKindredRowsRead(editor)[0].CReflexId;
        CKindred kindred = TInterfaceConductSound.TKindredCreate(
            editor, TKindredGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());
        List<CReflexTyped> typed = [];
        editor.CEditorDesk.CDeskDraft.CDeskDraftChanged += _ =>
            typed.Add(kindred.CKindredSet(first, CReflexField.CReflexFieldText, "zz"));

        editor.CEditorDesk.CDeskDraft.CDeskDraftResonate();

        CReflexTyped answer = Assert.Single(typed);
        Assert.Equal(CReflexField.CReflexFieldText, answer.CReflexTypedField);
        Assert.Equal("sui", answer.CReflexTypedText);
        Assert.Empty(answer.CReflexTypedHeads);
        Assert.Equal("sui", TKindredRowsRead(editor)[0].CReflexText);
    }

    private static long TKindredSave(LEngine engine)
    {
        return TKindredSave(
            engine,
            "水",
            "Chinese",
            [
                TInterface.TReflexDraftCreate("Wu", "Go-on", "sy"),
                TInterface.TReflexDraftCreate("Wu", string.Empty, "si"),
                TInterface.TReflexDraftCreate("Jin", string.Empty, "sui"),
            ]);
    }

    private static IReadOnlyList<CReflex> TKindredRowsRead(CEditor editor)
    {
        editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();
        return TInterfaceConductSound
            .TKindredCreate(editor, TKindredGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>())
            .CKindredRead()
            .CTimbreReflexRows;
    }

    private static CEditor TKindredPrepare(LEngine engine, long? entry)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        return editor;
    }

    private static long TKindredSave(
        LEngine engine, string headword, string language, IReadOnlyList<LReflexDraft> reflexes)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1)],
            [],
            reflexes: reflexes)).LEntryId;
    }

    private static CPhonologyBundle TKindredGuisePrepare(bool respelled = false)
    {
        return TInterfaceConduct.TPhonologyBundleCreate(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineGuiseRead"] = args => ((IReadOnlyList<string>)args![1]!)
                .Select(_ => TInterface.TReflexGuiseCreate(respelled, false, true))
                .ToList(),
        });
    }
}
