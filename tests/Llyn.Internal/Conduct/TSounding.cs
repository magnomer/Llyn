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
        CEditor editor = TSoundingEditorPrepare(engine, null);
        CSounding sounding = editor.CEditorSounding;

        Assert.Empty(sounding.CSoundingFanqieRead());
        Assert.Empty(sounding.CSoundingScriptRead());
        Assert.Empty(sounding.CSoundingParadigmRead());
        Assert.Empty(sounding.CSoundingAnchorScan([], "a", string.Empty));
        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
        Assert.Equal(string.Empty, sounding.CSoundingLanguageRead());
        Assert.False(sounding.CSoundingAnchorCheck("water"));
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

        CFanqieGroup group = Assert.Single(sounding.CSoundingFanqieRead());

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

        Assert.Empty(sounding.CSoundingFanqieRead());
        Assert.Empty(sounding.CSoundingScriptRead());
        Assert.Equal(string.Empty, sounding.CSoundingReadingRead("water"));
        Assert.False(sounding.CSoundingAnchorCheck("water"));
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
        sounding.CSoundingFanqieSet(7, 1);
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

        sounding.CSoundingFanqieSet(7, 2);

        Assert.Equal([editor.CEditorDesk.CDeskStoredRead(), 7L, 2], sent);
    }

    [Fact]
    public void SoundingAnchorScan_StoredEntry_ComparesInTheDraftLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TSoundingEditorPrepare(engine, TSoundingEntrySave(engine));
        List<object?> sent = [];
        LFanqieRow row = TInterface.TFanqieRowCreate("水", "Guangyun", "式軌切", id: 7);
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineAnchorScan"] = args =>
            {
                sent.Add(args![2]);
                return TInterface.TAnchorRowScan([row], [7], []);
            },
        }, []);

        CAnchorRow anchored = Assert.Single(sounding.CSoundingAnchorScan([7], "a", string.Empty));

        Assert.Equal(["English"], sent);
        Assert.Equal(7, anchored.CAnchorRowId);
        Assert.True(anchored.CAnchorRowHeld);
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
        CSounding sounding = TSoundingCreate(editor, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineParadigmScan"] = _ => rows,
            ["LEngineLanguageResolve"] = _ => "English",
        }, []);

        Assert.Equal(
            [
                new CParadigmSlot("noun", "plural, past", "wolves", false),
                new CParadigmSlot("verb", "past", null, true),
            ],
            sounding.CSoundingParadigmRead());
        Assert.Equal("English", sounding.CSoundingLanguageRead());
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
        CEditor editor, Dictionary<string, Func<object?[]?, object?>> answers, List<string> notices)
    {
        return TInterfaceConduct.TSoundingCreate(
            editor.CEditorDesk,
            TEngineFake.TEngineCreate<LPhonologyPort>(answers),
            TEngineFake.TEngineCreate<LDraftPort>(answers),
            TInterfaceConduct.TEnvoyCreate(false, notices));
    }
}
