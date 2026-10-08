using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTimbreReflex
{
    [Fact]
    public void TimbreReflexRead_StoredEntry_AnswersEveryRowWithItsAnchorAndTheFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(
            engine,
            TTimbreReflexSave(
                engine,
                "kindle",
                "English",
                [
                    TInterface.TReflexDraftCreate("Korean", string.Empty, "a"),
                    TInterface.TReflexDraftCreate("Wu", string.Empty, string.Empty),
                ]));
        List<object?[]?> checks = [];
        List<object?[]?> formats = [];
        CTimbre timbre = TInterfaceConductSound.TTimbreCreate(
            editor,
            TTimbreGuisePrepare(),
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

        CTimbreReflex reflex = timbre.CTimbreReflexRead();

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
    public void TimbreReflexRead_EmptyDesk_AnswersNoRowsAndNoAnchor()
    {
        CTimbreReflex reflex = TTimbre.TTimbrePrepare([]).CTimbreReflexRead();

        Assert.False(reflex.CTimbreReflexShown);
        Assert.Empty(reflex.CTimbreReflexRows);
        Assert.False(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Empty(reflex.CTimbreReflexAnchor.CLecternAnchorTexts);
        Assert.False(reflex.CTimbreReflexOpened);
        Assert.False(reflex.CTimbreReflexPending);
    }

    [Fact]
    public void TimbreReflexRead_FreshDraft_OffersNoAnchor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, null);
        CTimbre timbre = TInterfaceConductSound.TTimbreCreate(
            editor, TTimbreGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());

        CTimbreReflex reflex = timbre.CTimbreReflexRead();

        Assert.Empty(reflex.CTimbreReflexRows);
        Assert.False(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Empty(reflex.CTimbreReflexAnchor.CLecternAnchorTexts);
    }

    [Fact]
    public void TimbreReflexRead_RefusedAnchor_AnswersTheRowsUnanchored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(
            engine,
            TTimbreReflexSave(
                engine, "water", "English", [TInterface.TReflexDraftCreate("Korean", string.Empty, "a")]));
        CTimbre timbre = TInterfaceConductSound.TTimbreCreate(
            editor, TTimbreGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());

        CTimbreReflex reflex = timbre.CTimbreReflexRead();

        Assert.Equal("a", Assert.Single(reflex.CTimbreReflexRows).CReflexText);
        Assert.False(reflex.CTimbreReflexAnchor.CLecternAnchorOffered);
        Assert.Empty(reflex.CTimbreReflexAnchor.CLecternAnchorTexts);
    }

    [Fact]
    public async Task TimbreReflexStart_StoredEntryWithoutReflexes_StartsTheLookupWhenTheDraftOpens()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(string.Empty, gate.Task));
        long entry = TTimbreReflexSave(engine, "弄", pack.TLanguageFixtureName, []);

        CEditor editor = TTimbreReflexPrepare(engine, entry);

        Assert.True(editor.CEditorTimbre.CTimbreReflexPending);
        Assert.True(editor.CEditorTimbre.CTimbreReflexRead().CTimbreReflexPending);
        gate.SetResult();
        await TReflexFixture.TReflexSettle(engine, entry);
    }

    [Fact]
    public void TimbreReflexStart_ReflectedOrFreshDraft_StartsNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(string.Empty, gate.Task));
        long entry = TTimbreReflexSave(
            engine, "弄", pack.TLanguageFixtureName, [TInterface.TReflexDraftCreate("Korean", string.Empty, "롱")]);

        CEditor reflected = TTimbreReflexPrepare(engine, entry);
        CEditor fresh = TTimbreReflexPrepare(engine, null);
        fresh.CEditorLanguageSet(pack.TLanguageFixtureName);
        fresh.CEditorHeadwordSet("弄");

        Assert.False(reflected.CEditorTimbre.CTimbreReflexPending);
        Assert.False(engine.TEngineReflexCheck(entry));
        Assert.False(fresh.CEditorTimbre.CTimbreReflexRead().CTimbreReflexPending);
        gate.SetResult();
    }

    [Fact]
    public void TimbreReflexAdd_PressedRow_PlacesARowOfItsLanguageAndKindBelowIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));

        editor.CEditorTimbre.CTimbreReflexAdd(
            TTimbreReflexRead(editor).First(static row => row.CReflexText == "sy").CReflexId);

        IReadOnlyList<CReflex> rows = TTimbreReflexRead(editor);
        Assert.Equal(["Jin", "Wu", "Wu", "Wu"], rows.Select(static row => row.CReflexLanguage));
        Assert.Equal([string.Empty, string.Empty, "Go-on", "Go-on"], rows.Select(static row => row.CReflexKind));
        Assert.Equal(["sui", "si", "sy", string.Empty], rows.Select(static row => row.CReflexText));
    }

    [Fact]
    public void TimbreReflexAdd_NoOrGoneRow_AppendsABlankRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));

        editor.CEditorTimbre.CTimbreReflexAdd(0);
        editor.CEditorTimbre.CTimbreReflexAdd(long.MaxValue);

        IReadOnlyList<CReflex> rows = TTimbreReflexRead(editor);
        Assert.Equal(
            ["Jin", "Wu", "Wu", string.Empty, string.Empty], rows.Select(static row => row.CReflexLanguage));
        Assert.Equal(["sui", "si", "sy", string.Empty, string.Empty], rows.Select(static row => row.CReflexText));
    }

    [Fact]
    public void TimbreReflexRemove_HeldRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));

        editor.CEditorTimbre.CTimbreReflexRemove(TTimbreReflexRead(editor)[1].CReflexId);

        Assert.Equal(["sui", "sy"], TTimbreReflexRead(editor).Select(static row => row.CReflexText));
    }

    [Fact]
    public void TimbreReflexToggle_HeldRow_FlipsItsMainEachTime()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));
        long second = TTimbreReflexRead(editor)[1].CReflexId;

        editor.CEditorTimbre.CTimbreReflexToggle(second);
        Assert.Equal([false, true, false], TTimbreReflexRead(editor).Select(static row => row.CReflexMain));

        editor.CEditorTimbre.CTimbreReflexToggle(second);
        editor.CEditorTimbre.CTimbreReflexToggle(long.MaxValue);
        Assert.Equal([false, false, false], TTimbreReflexRead(editor).Select(static row => row.CReflexMain));
    }

    [Fact]
    public void TimbreReflexSet_TypedLanguage_WritesItAndLeadsByTheOverlaidRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entry = TTimbreReflexSave(engine);
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        CReflexTyped refused = editor.CEditorTimbre.CTimbreReflexSet(1, CReflexField.CReflexFieldLanguage, "Wu");
        Assert.Equal(CReflexField.CReflexFieldLanguage, refused.CReflexTypedField);
        Assert.Equal(string.Empty, refused.CReflexTypedText);
        Assert.Empty(refused.CReflexTypedHeads);

        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        IReadOnlyList<long> ids = TTimbreReflexRead(editor).Select(static row => row.CReflexId).ToList();

        Assert.Equal(
            [new CReflexHead(ids[0], true), new CReflexHead(ids[1], true), new CReflexHead(ids[2], false)],
            editor.CEditorTimbre.CTimbreReflexSet(ids[1], CReflexField.CReflexFieldLanguage, "Wu").CReflexTypedHeads);
        CReflexTyped typed = editor.CEditorTimbre.CTimbreReflexSet(ids[1], CReflexField.CReflexFieldLanguage, "Jin");
        Assert.Equal(
            [new CReflexHead(ids[0], true), new CReflexHead(ids[1], false), new CReflexHead(ids[2], true)],
            typed.CReflexTypedHeads);
        Assert.Equal("Jin", typed.CReflexTypedText);
        Assert.Equal("Reflex.Jin", typed.CReflexTypedKey);
        Assert.Equal(["Jin", "Jin", "Wu"], TTimbreReflexRead(editor).Select(static row => row.CReflexLanguage));
    }

    [Fact]
    public void TimbreReflexSet_TextOfARespelledRow_WritesTheRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));
        long first = TTimbreReflexRead(editor)[0].CReflexId;
        CTimbre respelled = TInterfaceConductSound.TTimbreCreate(
            editor, TTimbreGuisePrepare(true), TEngineFake.TEngineStubCreate<LDraftPort>());

        Assert.Empty(respelled.CTimbreReflexSet(first, CReflexField.CReflexFieldText, "sü").CReflexTypedHeads);

        editor.CEditorDesk.CDeskPersist();
        Assert.Equal("sü", respelled.CTimbreReflexRead().CTimbreReflexRows[0].CReflexText);
        Assert.Equal("sui", TTimbreReflexRead(editor)[0].CReflexText);
    }

    [Fact]
    public void TimbreReflexSet_TextOfAPlainRow_WritesThePhoneticText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));
        long first = TTimbreReflexRead(editor)[0].CReflexId;
        CTimbre plain = TInterfaceConductSound.TTimbreCreate(
            editor, TTimbreGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());

        Assert.Empty(plain.CTimbreReflexSet(first, CReflexField.CReflexFieldText, "sɿ").CReflexTypedHeads);

        Assert.Equal("sɿ", TTimbreReflexRead(editor)[0].CReflexText);
    }

    [Fact]
    public void TimbreReflexSet_OtherCells_WriteEachAndAnswerNoLeads()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));
        long first = TTimbreReflexRead(editor)[0].CReflexId;
        CTimbre timbre = editor.CEditorTimbre;

        IReadOnlyList<CReflexTyped> typed =
        [
            timbre.CTimbreReflexSet(first, CReflexField.CReflexFieldKind, "Kan-on"),
            timbre.CTimbreReflexSet(first, CReflexField.CReflexFieldRomanization, "si"),
            timbre.CTimbreReflexSet(first, CReflexField.CReflexFieldMeaning, "water"),
            timbre.CTimbreReflexSet(first, CReflexField.CReflexFieldNote, "literary"),
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

        CReflex row = TTimbreReflexRead(editor)[0];
        Assert.Equal(
            ["Kan-on", "si", "water", "literary"],
            new[] { row.CReflexKind, row.CReflexRomanization, row.CReflexMeaning, row.CReflexNote });
    }

    [Fact]
    public void TimbreReflexSet_WhileTheDeskFills_TakesNothingAndAnswersTheHeldText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreReflexPrepare(engine, TTimbreReflexSave(engine));
        long first = TTimbreReflexRead(editor)[0].CReflexId;
        CTimbre timbre = TInterfaceConductSound.TTimbreCreate(
            editor, TTimbreGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>());
        List<CReflexTyped> typed = [];
        editor.CEditorDesk.CDeskDraftChanged += _ =>
            typed.Add(timbre.CTimbreReflexSet(first, CReflexField.CReflexFieldText, "zz"));

        editor.CEditorDesk.CDeskDraftResonate();

        CReflexTyped answer = Assert.Single(typed);
        Assert.Equal(CReflexField.CReflexFieldText, answer.CReflexTypedField);
        Assert.Equal("sui", answer.CReflexTypedText);
        Assert.Empty(answer.CReflexTypedHeads);
        Assert.Equal("sui", TTimbreReflexRead(editor)[0].CReflexText);
    }

    private static long TTimbreReflexSave(LEngine engine)
    {
        return TTimbreReflexSave(
            engine,
            "水",
            "Chinese",
            [
                TInterface.TReflexDraftCreate("Wu", "Go-on", "sy"),
                TInterface.TReflexDraftCreate("Wu", string.Empty, "si"),
                TInterface.TReflexDraftCreate("Jin", string.Empty, "sui"),
            ]);
    }

    private static IReadOnlyList<CReflex> TTimbreReflexRead(CEditor editor)
    {
        editor.CEditorDesk.CDeskPersist();
        return TInterfaceConductSound
            .TTimbreCreate(editor, TTimbreGuisePrepare(), TEngineFake.TEngineStubCreate<LDraftPort>())
            .CTimbreReflexRead()
            .CTimbreReflexRows;
    }

    private static CEditor TTimbreReflexPrepare(LEngine engine, long? entry)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        return editor;
    }

    private static long TTimbreReflexSave(
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

    private static CPhonologyBundle TTimbreGuisePrepare(bool respelled = false)
    {
        return TInterfaceConduct.TPhonologyBundleCreate(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineGuiseRead"] = args => ((IReadOnlyList<string>)args![1]!)
                .Select(_ => TInterface.TReflexGuiseCreate(respelled, false, true))
                .ToList(),
        });
    }
}
