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
    public void SoundingFanqieRead_FreshDraft_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CSounding sounding = TSoundingEditorPrepare(engine, null).TEditorFixtureSounding;

        CLecternParadigm paradigm = sounding.CSoundingParadigmRead();

        Assert.Empty(sounding.CSoundingFanqieRead().CSoundingFanqieGroups);
        Assert.Empty(sounding.CSoundingScriptRead().CSoundingScriptGroups);
        Assert.Empty(paradigm.CLecternParadigmSlots);
        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
        Assert.Equal(new CFont(null, null, CFontSlant.CFontSlantTheme), paradigm.CLecternParadigmFont);
    }

    [Fact]
    public void SoundingFanqieRead_StoredEntry_ShapesTheEngineGroups()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        List<object?> asked = [];
        IReadOnlyList<LFanqieGroup> groups = TInterface.TFanqieGroupScan(
            [TInterface.TFanqieRowCreate("水", "Guangyun", "式軌切", id: 7)],
            [TInterface.TFanqieBookCreate("Guangyun", "Guangyun")]);
        CSounding sounding = TSoundingCreate(desk, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFanqieRead"] = args =>
            {
                asked.Add(args![0]);
                return groups;
            },
        }, []);

        CFanqieGroup group = Assert.Single(sounding.CSoundingFanqieRead().CSoundingFanqieGroups);

        Assert.Equal(7, Assert.Single(group.CFanqieGroupRows).CFanqieRowId);
        Assert.Equal([desk.CDeskStoredRead()], asked);
    }

    [Fact]
    public void SoundingFanqieRead_RefusedRead_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        CSounding sounding = TSoundingCreate(desk, [], []);

        CSoundingFanqie fanqie = sounding.CSoundingFanqieRead();
        CSoundingScript script = sounding.CSoundingScriptRead();

        Assert.Empty(fanqie.CSoundingFanqieGroups);
        Assert.Empty(script.CSoundingScriptGroups);
        Assert.False(fanqie.CSoundingFanqieRebuildable);
        Assert.False(script.CSoundingScriptRebuildable);
        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
    }

    [Fact]
    public void SoundingReadingRead_RefusedRead_ShowsTheReadingFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        List<string> notices = [];
        CSounding sounding = TSoundingCreate(desk, [], notices);

        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
        Assert.Equal(["Display.ReadingFailed"], notices);
    }

    [Fact]
    public void SoundingParadigmRead_RefusedMorphology_ShowsTheMorphologyFailure()
    {
        List<string> notices = [];
        CSounding sounding = new TEditorFixture(TInterfaceEditor.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TInterfaceConduct.TEntryBundleCreate([]),
                TInterfaceConduct.TPhonologyBundleCreate([]),
                TInterfaceConduct.TSettingsCreate(),
                TEngineFake.TEngineStubCreate<LMediaPort>(),
                TEnvoyFake.TEnvoyCreate(false, notices)))
            .TEditorFixtureSounding;

        Assert.Empty(sounding.CSoundingParadigmRead().CLecternParadigmSlots);
        Assert.Equal(["Sound.MorphologyFailed"], notices);
    }

    [Fact]
    public void SoundingFanqieResolve_StoredEntry_AnnouncesTheChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        List<string> notices = [];
        CSounding sounding = TSoundingCreate(desk, new Dictionary<string, Func<object?[]?, object?>>
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
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        List<string> notices = [];
        CSounding sounding = TSoundingCreate(desk, [], notices);
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
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        List<object?> sent = [];
        CSounding sounding = TSoundingCreate(desk, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFanqieSet"] = args =>
            {
                sent.AddRange(args!);
                return null;
            },
        }, []);

        sounding.CSoundingFanqieSet(7, 2, true);

        Assert.Equal([desk.CDeskStoredRead(), 7L, 2, true], sent);
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
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LMorphology plural = TInterfaceInflection.TMorphologyCreate(1, 1, "plural", 0);
        IReadOnlyList<LParadigmRow> rows = TInterface.TParadigmRowScan(
            [TInterface.TParadigmSlotCreate(noun, plural, null, LState.LStateUnspecified)]);
        CSounding sounding = TSoundingCreate(desk,new Dictionary<string, Func<object?[]?, object?>>
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
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LSpeechValue verb = TInterface.TSpeechValueCreate("English", 2, "verb", 1) with { LSpeechValueId = 2 };
        LMorphology plural = TInterfaceInflection.TMorphologyCreate(1, 1, "plural", 0);
        LMorphology past = TInterfaceInflection.TMorphologyCreate(2, 1, "past", 0);
        LInflection wolves = TInterfaceInflection.TInflectionCreate(1, 0, "wolves", null, 1, [1]);
        IReadOnlyList<LParadigmRow> rows = TInterface.TParadigmRowScan(
        [
            TInterface.TParadigmSlotCreate(noun, plural, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(noun, past, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(verb, past, null, LState.LStateUnknown),
        ]);
        List<(string, LFontRole)> asked = [];
        CSounding sounding = TSoundingCreate(desk, new Dictionary<string, Func<object?[]?, object?>>
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
        Assert.Equal(new CFont("Noto Serif", 21, CFontSlant.CFontSlantTheme), paradigm.CLecternParadigmFont);
        Assert.Equal([("Latin", LFontRole.LFontRoleHeadword)], asked);
    }

    [Fact]
    public void SoundingScriptRead_StoredEntry_AnswersTheWholeBlockInTheDraftLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine)).TEditorFixtureDesk;
        List<(string, LFontRole)> asked = [];
        List<string> packs = [];
        CSounding sounding = TSoundingCreate(desk,new Dictionary<string, Func<object?[]?, object?>>
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
        Assert.Equal(new CFont("Noto Serif", 21, CFontSlant.CFontSlantTheme), script.CSoundingScriptFont);
        Assert.Equal(["English", "English"], packs);
        Assert.Equal([("English", LFontRole.LFontRoleGlyph), ("English", LFontRole.LFontRoleGlyph)], asked);
    }

    [Fact]
    public void SoundingFanqieRead_NoStoredEntry_OffersNoRebuildAndWaitsForNothing()
    {
        CSounding sounding = new TEditorFixture(TInterfaceEditor.TEditorCreate(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                TInterfaceConduct.TEntryBundleCreate([]),
                TInterfaceConduct.TPhonologyBundleCreate(new Dictionary<string, Func<object?[]?, object?>>
                {
                    ["LEngineBookCheck"] = _ => true,
                    ["LEngineStyleCheck"] = _ => true,
                    ["LEngineFanqieCheck"] = _ => true,
                    ["LEngineScriptCheck"] = _ => true,
                    ["LEngineInflectionCheck"] = _ => true,
                }),
                TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
                {
                    ["LEngineMorphologyCheck"] = _ => false,
                }),
                TEngineFake.TEngineStubCreate<LMediaPort>()))
            .TEditorFixtureSounding;

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
        CSounding sounding = TSoundingDiweiPrepare(engine, atelier, cells).TEditorFixtureSounding;

        sounding.CSoundingDiweiOpen(true, "sh");
        sounding.CSoundingDiweiOpen(false, "寒 I");

        Assert.Equal(["English initial sh", "English rime 寒 I"], cells);
    }

    [Fact]
    public void SoundingDiweiOpen_EmptyKey_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> cells = [];
        CSounding sounding = TSoundingDiweiPrepare(engine, atelier, cells).TEditorFixtureSounding;

        sounding.CSoundingDiweiOpen(true, string.Empty);
        sounding.CSoundingDiweiOpen(false, string.Empty);

        Assert.Empty(cells);
    }

    private static TEditorFixture TSoundingDiweiPrepare(LEngine engine, CAtelier atelier, List<string> cells)
    {
        TEditorFixture editor = TEditorFixture.TEditorFixtureCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(TSoundingEntrySave(engine));
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

    private static TEditorFixture TSoundingEditorPrepare(LEngine engine, long? entry)
    {
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(entry);
        return editor;
    }

    private static CSounding TSoundingCreate(
        CDesk desk,
        Dictionary<string, Func<object?[]?, object?>> answers,
        List<string> notices,
        LSettingsPort? pack = null)
    {
        return TInterfaceConductSound.TSoundingCreate(
            desk,
            TInterfaceConduct.TPhonologyBundleCreate(answers),
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
