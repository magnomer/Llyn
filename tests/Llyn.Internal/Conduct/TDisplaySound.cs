using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplaySound
{
    [Fact]
    public void DisplayGlyphRead_HanjaRow_AnswersTheRowReadyToShow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(TDisplayKoreanSave(engine).LEntryId);

        CLecternGlyph glyph = wing.CWingDisplay.CDisplaySound.CDisplayGlyphRead();

        Assert.True(glyph.CLecternGlyphShown);
        Assert.Equal("Scheme.Hanja", glyph.CLecternGlyphKey);
        Assert.Equal("Hanja", glyph.CLecternGlyphName);
        Assert.Equal(
            [new CGlyphCell("漢", "Classical Chinese", true), new CGlyphCell("字", "Classical Chinese", true)],
            glyph.CLecternGlyphCells);
        Assert.Equal("Batang, Malgun Gothic", glyph.CLecternGlyphFont.CFontFamily);
    }

    [Fact]
    public void DisplayGlyphRead_LanguageWithoutGlyphOrNothingShown_AnswersTheHiddenRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        CLecternGlyph blank = wing.CWingDisplay.CDisplaySound.CDisplayGlyphRead();
        wing.CWingEntryOpen(TDisplayEnglishSave(engine).LEntryId);

        CLecternGlyph glyph = wing.CWingDisplay.CDisplaySound.CDisplayGlyphRead();

        Assert.False(blank.CLecternGlyphShown);
        Assert.Empty(blank.CLecternGlyphCells);
        Assert.False(glyph.CLecternGlyphShown);
        Assert.Empty(glyph.CLecternGlyphName);
    }

    [Fact]
    public void DisplayGlyphOpen_LinkedCell_RaisesTheCharacterEntryForTheLibrary()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingPrepare(atelier, asked);
        List<long> opened = [];
        wing.CWingDisplay.CDisplaySound.CDisplayRowChosen += (tab, entry) =>
        {
            Assert.Equal("Library", tab);
            opened.Add(entry);
        };

        bool shown = wing.CWingDisplay.CDisplaySound.CDisplayGlyphOpen("漢", "Classical Chinese");
        bool refused = wing.CWingDisplay.CDisplaySound.CDisplayGlyphOpen(" ", "Classical Chinese");

        Assert.True(shown);
        Assert.True(Assert.Single(opened) > 0);
        Assert.False(refused);
        Assert.Equal(["Glyph.OpenFailed"], asked);
    }

    [Fact]
    public void DisplayTranscriptionRead_GlyphAndEmptyRows_ListsOnlyOtherFilledRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        Assert.Empty(wing.CWingDisplay.CDisplaySound.CDisplayTranscriptionRead());
        wing.CWingEntryOpen(TDisplayKoreanSave(engine).LEntryId);

        IReadOnlyList<CTranscriptionDraft> rows = wing.CWingDisplay.CDisplaySound.CDisplayTranscriptionRead();

        CTranscriptionDraft row = Assert.Single(rows);
        Assert.Equal("Revised Romanization", row.CTranscriptionDraftScheme);
        Assert.Equal("hanja", row.CTranscriptionDraftText);
    }

    [Fact]
    public void DisplayReflexRead_StoredEntry_AnswersWrittenRowsWithTheirAnchors()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        CLecternReflex blank = wing.CWingDisplay.CDisplaySound.CDisplayReflexRead();
        LEntry kindle = engine.TEngineEntrySave(TDisplayDraftCreate(
            "kindle",
            "English",
            [
                TInterface.TReflexDraftCreate("Korean", string.Empty, "a"),
                TInterface.TReflexDraftCreate("Korean", string.Empty, " "),
            ]));
        wing.CWingEntryOpen(kindle.LEntryId);

        CLecternReflex reflex = wing.CWingDisplay.CDisplaySound.CDisplayReflexRead();

        Assert.Empty(blank.CLecternReflexRows);
        CReflex row = Assert.Single(reflex.CLecternReflexRows);
        Assert.Equal("a", row.CReflexText);
        Assert.True(row.CReflexLead);
        Assert.False(reflex.CLecternReflexAnchor.CLecternAnchorOffered);
        Assert.Equal(string.Empty, reflex.CLecternReflexAnchor.CLecternAnchorTexts[row.CReflexId]);
        Assert.False(reflex.CLecternReflexPending);
    }

    [Fact]
    public void DisplayReflexResonate_ShownDraftWithoutRows_ReadsTheStoredRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        LEntry kindle = engine.TEngineEntrySave(
            TDisplayDraftCreate("kindle", "English", [TInterface.TReflexDraftCreate("Korean", string.Empty, "a")]));
        editor.CEditorDisplay.LDisplaySound.TDisplaySoundShow(
            kindle.LEntryId, TDisplayDraftCreate("kindle", "English", []));
        CLecternReflex shown = editor.CEditorDisplay.CDisplaySound.CDisplayReflexRead();

        CLecternReflex reloaded = editor.CEditorDisplay.CDisplaySound.CDisplayReflexResonate();

        Assert.Empty(shown.CLecternReflexRows);
        Assert.Equal("a", Assert.Single(reloaded.CLecternReflexRows).CReflexText);
    }

    [Fact]
    public void DisplayReflexToggle_Opened_SetsTheSharedFoldAndRaisesTheChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        CDisplaySound area = editor.CEditorDisplay.CDisplaySound;
        int changed = 0;
        area.CDisplayFoldChanged += () => changed++;

        area.CDisplayReflexToggle(true);

        Assert.True(area.CDisplayFoldOpened);
        Assert.True(editor.CEditorDisplay.LDisplaySound.LDisplayFoldOpened);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void DisplayPlaybackRead_AccentWithAudioOnly_ShowsTheTrayButNotTheButton()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TMediaCreate(engine));
        CWing wing = TDisplayWingPrepare(atelier, []);
        CLecternPlayback blank = wing.CWingDisplay.CDisplaySound.CDisplayPlaybackRead();
        LEntry water = engine.TEngineEntrySave(TDisplayDraftCreate("water", "English", []) with
        {
            LEntryDraftPronunciations =
            [
                TInterface.TPronunciationDraftCreate("ˈwɔːtə", "British"),
                TInterface.TPronunciationDraftCreate("ˈwɑːtɚ", "American", "missing.ogg"),
            ],
        });
        wing.CWingEntryOpen(water.LEntryId);

        CLecternPlayback playback = wing.CWingDisplay.CDisplaySound.CDisplayPlaybackRead();

        Assert.Equal(new CLecternPlayback(false, false), blank);
        Assert.Equal(new CLecternPlayback(false, true), playback);
    }

    [Fact]
    public void DisplayPlaybackStart_AccentRow_PlaysAtTheLevelAndTheCancelStopsThatPlay()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> played = [];
        List<int> stopped = [];
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRecordingPlay"] = args =>
                {
                    object? file = args![0] is LEntryDraft draft ? draft.LEntryDraftHeadword : args[0];
                    played.Add(string.Concat(file, "@", args[1]));
                    return played.Count + 6;
                },
                ["LEngineRecordingStop"] = args =>
                {
                    stopped.Add((int)args![0]!);
                    return null;
                },
            }));
        CWing wing = TDisplayWingPrepare(atelier, []);
        CDisplaySound area = wing.CWingDisplay.CDisplaySound;
        area.CDisplayPlaybackStart(0.5);
        wing.CWingEntryOpen(TDisplayEnglishSave(engine).LEntryId);

        area.CDisplayPlaybackStart("row.ogg", 0.25);
        area.CDisplayPlaybackStart(0.5);
        area.CDisplayPlaybackCancel();

        Assert.Equal(["row.ogg@0.25", "water@0.5"], played);
        Assert.Equal([8], stopped);
    }

    [Fact]
    public void DisplayPlaybackCancel_NothingPlaying_KeepsTheShownEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRecordingStop"] = _ => null,
            }));
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(TDisplayEnglishSave(engine).LEntryId);

        wing.CWingDisplay.CDisplaySound.CDisplayPlaybackCancel();

        Assert.Equal("water", wing.CWingDisplay.CDisplayArea.CDisplayShown.CLecternHeadword);
    }

    [Fact]
    public void DisplayBlocksRead_NothingShown_AnswersEmptyBlocks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDisplaySound area = TDisplayWingPrepare(atelier, []).CWingDisplay.CDisplaySound;

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
        Assert.False(paradigm.CLecternParadigmMorphology);
        Assert.Null(paradigm.CLecternParadigmFont.CFontFamily);
    }

    [Fact]
    public void DisplayBlocksRead_EnglishEntry_AnswersNoRimeOrScriptAndTheMorphologySetting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(TDisplayEnglishSave(engine).LEntryId);
        CDisplaySound area = wing.CWingDisplay.CDisplaySound;

        CLecternFanqie fanqie = area.CDisplayFanqieRead();
        CLecternScript script = area.CDisplayScriptRead();
        CLecternParadigm paradigm = area.CDisplayParadigmRead();

        Assert.Empty(fanqie.CLecternFanqieGroups);
        Assert.False(fanqie.CLecternFanqiePending);
        Assert.Empty(fanqie.CLecternFanqieAnchor.CLecternAnchorTexts);
        Assert.Empty(script.CLecternScriptGroups);
        Assert.False(script.CLecternScriptPending);
        Assert.True(paradigm.CLecternParadigmMorphology);
    }

    [Fact]
    public void DisplayDiweiAndStemOpen_ShownEntry_RaiseTheShownLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        CDisplaySound area = wing.CWingDisplay.CDisplaySound;
        List<string> opened = [];
        area.CDisplayDiweiChosen += (language, kind, key) => opened.Add(language + kind + key);
        area.CDisplayStemChosen += (language, key) => opened.Add(language + (key ?? "-"));
        bool early = area.CDisplayDiweiOpen("initial", "k");
        wing.CWingEntryOpen(TDisplayKoreanSave(engine).LEntryId);

        bool diwei = area.CDisplayDiweiOpen("initial", "k");
        bool stem = area.CDisplayStemOpen(null);

        Assert.False(early);
        Assert.True(diwei);
        Assert.True(stem);
        Assert.Equal(["Koreaninitialk", "Korean-"], opened);
    }

    [Fact]
    public void DisplayFanqieSet_ShownEntry_SetsTheRankForTheShownEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> set = [];
        CEditor editor = TInterfaceConduct.TEditorCreate(
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
        editor.CEditorDisplay.CDisplaySound.CDisplayFanqieSet(3, 1);
        editor.CEditorDisplay.LDisplaySound.TDisplaySoundShow(7, TDisplayDraftCreate("國", "Korean", []));

        editor.CEditorDisplay.CDisplaySound.CDisplayFanqieSet(3, 2);

        Assert.Equal(["7,3,2"], set);
    }

    private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        return wing;
    }

    private static LEntryDraft TDisplayDraftCreate(
        string headword, string language, IReadOnlyList<LReflexDraft> reflexes) =>
        TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1)],
            [],
            reflexes: reflexes);

    private static LEntry TDisplayEnglishSave(LEngine engine) =>
        engine.TEngineEntrySave(TDisplayDraftCreate("water", "English", []));

    private static LEntry TDisplayKoreanSave(LEngine engine) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "한자",
            "Korean",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a character", 1)],
            [],
            transcriptions:
            [
                TInterface.TTranscriptionDraftCreate("Revised Romanization", "hanja"),
                TInterface.TTranscriptionDraftCreate("Hanja", "漢字"),
                TInterface.TTranscriptionDraftCreate("Yale", string.Empty),
            ]));
}
