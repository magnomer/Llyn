using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSounding
{
    [Fact]
    public void SoundingVarietyRead_NamedOrBlankVariety_KeysTheLabelAndTheFlag()
    {
        Assert.Equal(
            new CVariety("Scottish", "Variety.Scottish", "English/Scottish"),
            CSounding.CSoundingVarietyRead("English", "Scottish"));
        Assert.Equal(
            new CVariety(string.Empty, "Variety.", string.Empty),
            CSounding.CSoundingVarietyRead("English", string.Empty));
    }

    [Fact]
    public void SoundingFanqieRead_FreshDraft_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, null);
        CSounding sounding = editor.CEditorSounding;

        CLecternParadigm paradigm = sounding.CSoundingParadigmRead();

        Assert.Empty(sounding.CSoundingFanqieRead().CSoundingFanqieGroups);
        Assert.Empty(sounding.CSoundingScriptRead().CSoundingScriptGroups);
        Assert.Empty(paradigm.CLecternParadigmSlots);
        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
        Assert.Equal(new CFont(null, null, null), paradigm.CLecternParadigmFont);
    }

    [Fact]
    public void SoundingFanqieRead_StoredEntry_ShapesTheEngineGroups()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        List<object?> asked = [];
        IReadOnlyList<LFanqieGroup> groups = TInterface.TFanqieGroupScan(
            [TInterface.TFanqieRowCreate("水", "Guangyun", "式軌切", id: 7)],
            [TInterface.TFanqieBookCreate("Guangyun", "Guangyun")]);
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFanqieRead"] = args =>
            {
                asked.Add(args![0]);
                return groups;
            },
        }, []);

        CFanqieGroup group = Assert.Single(sounding.CSoundingFanqieRead().CSoundingFanqieGroups);

        Assert.Equal(7, Assert.Single(group.CFanqieGroupRows).CFanqieRowId);
        Assert.Equal([editor.CEditorDesk.CDeskStoredRead()], asked);
    }

    [Fact]
    public void SoundingFanqieRead_RefusedRead_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        CSounding sounding = TSoundingCreate(editor, [], []);

        CSoundingFanqie fanqie = sounding.CSoundingFanqieRead();
        CSoundingScript script = sounding.CSoundingScriptRead();

        Assert.Empty(fanqie.CSoundingFanqieGroups);
        Assert.Empty(script.CSoundingScriptGroups);
        Assert.False(fanqie.CSoundingFanqieRebuildable);
        Assert.False(script.CSoundingScriptRebuildable);
        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
    }

    [Fact]
    public void SoundingFanqieResolve_StoredEntry_AnnouncesTheChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        List<string> notices = [];
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFanqieRebuild"] = _ => null,
        }, notices);
        int changes = 0;
        sounding.CSoundingChanged += () => changes++;

        sounding.CSoundingFanqieResolve();

        Assert.Equal(1, changes);
        Assert.Empty(notices);
    }

    [Fact]
    public void SoundingFanqieResolve_RefusedRebuild_ShowsTheNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        List<string> notices = [];
        CSounding sounding = TSoundingCreate(editor, [], notices);
        int changes = 0;
        sounding.CSoundingChanged += () => changes++;

        sounding.CSoundingFanqieResolve();
        sounding.CSoundingFanqieSet(7, 1, false);
        sounding.CSoundingScriptResolve();

        Assert.Equal(0, changes);
        Assert.Equal(
            ["Display.FanqieRebuildFailed", "Display.FanqieRepresentativeFailed", "Display.ScriptRebuildFailed"],
            notices);
    }

    [Fact]
    public void SoundingFanqieSet_StoredEntry_SendsTheRank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        List<object?> sent = [];
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFanqieSet"] = args =>
            {
                sent.AddRange(args!);
                return null;
            },
        }, []);

        sounding.CSoundingFanqieSet(7, 2, true);

        Assert.Equal([editor.CEditorDesk.CDeskStoredRead(), 7L, 2, true], sent);
    }

    [Fact]
    public void SchemeKeyRead_SchemeOrBlank_PrefixesTheSchemeKey()
    {
        Assert.Equal("Scheme.Yale", CScheme.CSchemeKeyRead("Yale"));
        Assert.Equal("Scheme.", CScheme.CSchemeKeyRead(string.Empty));
    }

    [Theory]
    [InlineData(true, true, "…", "Paradigm.Pending")]
    [InlineData(true, false, "…", "Paradigm.Pending")]
    [InlineData(false, true, "…", "Paradigm.Held")]
    [InlineData(false, false, "…", "Paradigm.Absent")]
    public void SoundingParadigmRead_UnansweredSlot_AnswersTheTipOfItsPendingAndMorphologyVerdicts(
        bool pending, bool morphology, string text, string tip)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LMorphology plural = TInterface.TMorphologyCreate(1, 1, "plural", 0);
        IReadOnlyList<LParadigmRow> rows = TInterface.TParadigmRowScan(
            [TInterface.TParadigmSlotCreate(noun, plural, null, LState.LStateUnspecified)]);
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineParadigmScan"] = _ => rows,
            ["LEngineLanguageResolve"] = _ => "Latin",
            ["LEngineInflectionCheck"] = _ => pending,
        }, [], TSoundingPackCreate([], morphology));

        CLecternParadigm paradigm = sounding.CSoundingParadigmRead();

        Assert.Equal([new CParadigmSlot(string.Empty, "plural", text, tip)], paradigm.CLecternParadigmSlots);
    }

    [Fact]
    public void SoundingParadigmRead_TwoSlotsOneForm_JoinsThemIntoOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LSpeechValue verb = TInterface.TSpeechValueCreate("English", 2, "verb", 1) with { LSpeechValueId = 2 };
        LMorphology plural = TInterface.TMorphologyCreate(1, 1, "plural", 0);
        LMorphology past = TInterface.TMorphologyCreate(2, 1, "past", 0);
        LInflection wolves = TInterface.TInflectionCreate(1, 0, "wolves", null, 1, [1]);
        IReadOnlyList<LParadigmRow> rows = TInterface.TParadigmRowScan(
        [
            TInterface.TParadigmSlotCreate(noun, plural, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(noun, past, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(verb, past, null, LState.LStateUnknown),
        ]);
        List<(string, LFontRole)> asked = [];
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineParadigmScan"] = _ => rows,
            ["LEngineLanguageResolve"] = _ => "Latin",
            ["LEngineInflectionCheck"] = _ => true,
        }, [], TSoundingPackCreate(asked, true));

        CLecternParadigm paradigm = sounding.CSoundingParadigmRead();

        Assert.Equal(
            [
                new CParadigmSlot("noun", "plural, past", "wolves", null),
                new CParadigmSlot("verb", "past", "—", "Paradigm.Unknown"),
            ],
            paradigm.CLecternParadigmSlots);
        Assert.Equal(new CFont("Noto Serif", 21, null), paradigm.CLecternParadigmFont);
        Assert.Equal([("Latin", LFontRole.LFontRoleHeadword)], asked);
    }

    [Fact]
    public void SoundingScriptRead_StoredEntry_AnswersTheWholeBlockInTheDraftLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        List<(string, LFontRole)> asked = [];
        List<string> packs = [];
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineScriptRead"] = _ => new List<LScriptGroup>(),
            ["LEngineFanqieRead"] = _ => new List<LFanqieGroup>(),
            ["LEngineScriptCheck"] = _ => true,
            ["LEngineFanqieCheck"] = _ => false,
            ["LEngineStyleCheck"] = args =>
            {
                packs.Add((string)args![0]!);
                return true;
            },
            ["LEngineBookCheck"] = args =>
            {
                packs.Add((string)args![0]!);
                return false;
            },
        }, [], TSoundingPackCreate(asked, false));

        CSoundingScript script = sounding.CSoundingScriptRead();
        CSoundingFanqie fanqie = sounding.CSoundingFanqieRead();

        Assert.True(script.CSoundingScriptPending);
        Assert.True(script.CSoundingScriptRebuildable);
        Assert.False(fanqie.CSoundingFanqiePending);
        Assert.False(fanqie.CSoundingFanqieRebuildable);
        Assert.Equal(new CFont("Noto Serif", 21, null), script.CSoundingScriptFont);
        Assert.Equal(["English", "English"], packs);
        Assert.Equal([("English", LFontRole.LFontRoleGlyph), ("English", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void SoundingFanqieRead_NoStoredEntry_OffersNoRebuildAndWaitsForNothing()
    {
        CSounding sounding = TInterfaceConduct.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                TEngineFake.TEngineCreate<LPhonologyPort>(new Dictionary<string, Func<object?[]?, object?>>
                {
                    ["LEngineBookCheck"] = _ => true,
                    ["LEngineStyleCheck"] = _ => true,
                    ["LEngineFanqieCheck"] = _ => true,
                    ["LEngineScriptCheck"] = _ => true,
                    ["LEngineInflectionCheck"] = _ => true,
                }),
                TEngineFake.TEngineStubCreate<LSettingsPort>(),
                TEngineFake.TEngineStubCreate<LMediaPort>())
            .CEditorSounding;

        CSoundingFanqie fanqie = sounding.CSoundingFanqieRead();
        CSoundingScript script = sounding.CSoundingScriptRead();

        Assert.False(fanqie.CSoundingFanqieRebuildable);
        Assert.False(script.CSoundingScriptRebuildable);
        Assert.False(fanqie.CSoundingFanqiePending);
        Assert.False(script.CSoundingScriptPending);
        Assert.Empty(sounding.CSoundingParadigmRead().CLecternParadigmSlots);
    }

    [Fact]
    public void SoundingDiweiOpen_HeldDraft_OpensTheCellInTheDraftLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> cells = [];
        CEditor editor = TSoundingDiweiPrepare(engine, atelier, cells);

        editor.CEditorSounding.CSoundingDiweiOpen(true, "sh");
        editor.CEditorSounding.CSoundingDiweiOpen(false, "寒 I");

        Assert.Equal(["English initial sh", "English rime 寒 I"], cells);
    }

    [Fact]
    public void SoundingDiweiOpen_EmptyKey_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> cells = [];
        CEditor editor = TSoundingDiweiPrepare(engine, atelier, cells);

        editor.CEditorSounding.CSoundingDiweiOpen(true, string.Empty);
        editor.CEditorSounding.CSoundingDiweiOpen(false, string.Empty);

        Assert.Empty(cells);
    }

    private static CEditor TSoundingDiweiPrepare(LEngine engine, CAtelier atelier, List<string> cells)
    {
        CEditor editor = CEditor.CEditorCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(TSoundingEntrySave(engine));
        CNavigation navigation = atelier.CAtelierNavigation;
        navigation.TNavigationTabAdd("Yunjing", static () => true, static () => 0, static _ => { }, static _ => { });
        navigation.TNavigationDiweiAttach((language, kind, key) => cells.Add(language + " " + kind + " " + key));
        return editor;
    }

    private static long TSoundingEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], [])).LEntryId;
    }

    private static CEditor TSoundingEditorPrepare(LEngine engine, long? entry)
    {
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        return editor;
    }

    private static CSounding TSoundingCreate(
        CEditor editor,
        Dictionary<string, Func<object?[]?, object?>> answers,
        List<string> notices,
        LSettingsPort? pack = null)
    {
        return TInterfaceConduct.TSoundingCreate(
            editor.CEditorDesk,
            TEngineFake.TEngineCreate<LPhonologyPort>(answers),
            TEnvoyFake.TEnvoyCreate(false, notices),
            pack);
    }

    private static LSettingsPort TSoundingPackCreate(List<(string, LFontRole)> asked, bool morphology)
    {
        return TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFontRead"] = args =>
            {
                asked.Add(((string)args![0]!, (LFontRole)args[1]!));
                return TInterfaceFont.TFontCreate("Noto Serif", 21);
            },
            ["LEngineMorphologyCheck"] = _ => morphology,
        });
    }
}
