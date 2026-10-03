using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplaySoundBlock
{
    [Fact]
    public void DisplayBlocksRead_NothingShown_AnswersEmptyBlocks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDisplaySound area = TDisplaySound.TDisplayWingPrepare(atelier, []).CWingDisplay.CDisplaySound;

        CLecternFanqie fanqie = area.CDisplayFanqieRead();
        CLecternScript script = area.CDisplayScriptRead();
        CLecternParadigm paradigm = area.CDisplayParadigmRead();

        Assert.Empty(fanqie.CLecternFanqieGroups);
        Assert.False(fanqie.CLecternFanqiePending);
        Assert.Empty(fanqie.CLecternFanqieReading);
        Assert.False(fanqie.CLecternFanqieAnchor.CLecternAnchorOffered);
        Assert.Empty(script.CLecternScriptGroups);
        Assert.False(script.CLecternScriptPending);
        Assert.Empty(paradigm.CLecternParadigmSlots);
        Assert.Null(paradigm.CLecternParadigmFont.CFontFamily);
    }

    [Fact]
    public void DisplayBlocksRead_EnglishEntry_AnswersNoRimeScriptOrParadigmRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(TDisplaySound.TDisplayEnglishSave(engine).LEntryId);
        CDisplaySound area = wing.CWingDisplay.CDisplaySound;

        CLecternFanqie fanqie = area.CDisplayFanqieRead();
        CLecternScript script = area.CDisplayScriptRead();
        CLecternParadigm paradigm = area.CDisplayParadigmRead();

        Assert.Empty(fanqie.CLecternFanqieGroups);
        Assert.False(fanqie.CLecternFanqiePending);
        Assert.Empty(fanqie.CLecternFanqieAnchor.CLecternAnchorTexts);
        Assert.Empty(script.CLecternScriptGroups);
        Assert.False(script.CLecternScriptPending);
        Assert.Empty(paradigm.CLecternParadigmSlots);
    }

    [Fact]
    public void DisplayParadigmRead_MorphologyOff_AnswersTheAbsentTipWithoutALookup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            new HttpClient(new TSourceHandler(string.Empty, HttpStatusCode.ServiceUnavailable, gate.Task)));
        engine.TEngineFrequencySave(false);
        engine.TEngineMorphologySave(false);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        long entry = TDisplayCatSave(engine).LEntryId;
        wing.CWingEntryOpen(entry);

        CLecternParadigm paradigm = wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.False(engine.TEngineInflectionCheck(entry));
        Assert.Equal(
            [new CParadigmSlot(string.Empty, "plural", "…", "Paradigm.Absent")], paradigm.CLecternParadigmSlots);
        gate.SetResult();
    }

    [Fact]
    public async Task DisplayParadigmRead_LookupGatedThenLost_AnswersThePendingTipThenTheLostTip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            new HttpClient(new TSourceHandler(string.Empty, HttpStatusCode.ServiceUnavailable, gate.Task)));
        engine.TEngineFrequencySave(false);
        engine.TEngineMorphologySave(true);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        long entry = TDisplayCatSave(engine).LEntryId;
        wing.CWingEntryOpen(entry);

        CLecternParadigm pending = wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();
        gate.SetResult();
        DateTime deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        while (engine.TEngineInflectionCheck(entry))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the inflection lookup to settle.");
            await Task.Delay(20);
        }

        CLecternParadigm lost = wing.CWingDisplay.CDisplaySound.CDisplayParadigmRead();

        Assert.Equal(
            [new CParadigmSlot(string.Empty, "plural", "…", "Paradigm.Pending")], pending.CLecternParadigmSlots);
        Assert.Equal(
            [new CParadigmSlot(string.Empty, "plural", "…", "Paradigm.Lost")], lost.CLecternParadigmSlots);
    }

    [Fact]
    public void DisplayDiweiAndStemOpen_ShownEntry_RaiseTheShownLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        CDisplaySound area = wing.CWingDisplay.CDisplaySound;
        List<string> opened = [];
        area.CDisplayDiweiChosen += (language, kind, key) => opened.Add(language + kind + key);
        area.CDisplayStemChosen += (language, key) => opened.Add(language + (key ?? "-"));
        bool early = area.CDisplayDiweiOpen(true, "k");
        wing.CWingEntryOpen(TDisplaySound.TDisplayKoreanSave(engine).LEntryId);

        bool diwei = area.CDisplayDiweiOpen(true, "k");
        bool rime = area.CDisplayDiweiOpen(false, "寒 I");
        bool stem = area.CDisplayStemOpen(null);

        Assert.False(early);
        Assert.True(diwei);
        Assert.True(rime);
        Assert.True(stem);
        Assert.Equal(["Koreaninitialk", "Koreanrime寒 I", "Korean-"], opened);
    }

    [Fact]
    public void DisplayDiweiOpen_EmptyKey_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplaySound.TDisplayWingPrepare(atelier, []);
        CDisplaySound area = wing.CWingDisplay.CDisplaySound;
        List<string> opened = [];
        area.CDisplayDiweiChosen += (language, kind, key) => opened.Add(language + kind + key);
        wing.CWingEntryOpen(TDisplaySound.TDisplayKoreanSave(engine).LEntryId);

        bool initial = area.CDisplayDiweiOpen(true, string.Empty);
        bool rime = area.CDisplayDiweiOpen(false, string.Empty);

        Assert.False(initial);
        Assert.False(rime);
        Assert.Empty(opened);
    }

    [Fact]
    public void DisplayFanqieSet_ShownEntry_SetsTheRankForTheShownEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> set = [];
        CEditor editor = TInterfaceEditor.TEditorCreate(
            engine,
            TEngineFake.TEngineCreate<LPhonologyPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineSoundStart"] = _ => null,
                ["LEngineFanqieSet"] = args =>
                {
                    set.Add(string.Join(",", args!));
                    return null;
                },
            }));
        editor.CEditorDisplay.CDisplaySound.CDisplayFanqieSet(3, 1, false);
        editor.CEditorDisplay.LDisplaySound.TDisplaySoundShow(7, TDisplaySound.TDisplayDraftCreate("國", "Korean", []));

        editor.CEditorDisplay.CDisplaySound.CDisplayFanqieSet(3, 2, true);

        Assert.Equal(["7,3,2,True"], set);
    }

    private static LEntry TDisplayCatSave(LEngine engine) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "cat",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1)],
            [],
            speeches: ["Noun, countable"]));
}
