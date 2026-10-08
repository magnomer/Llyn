using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTimbre
{
    [Fact]
    public void TimbrePhonemic_RespellingPhonemicPack_ReadsTrue()
    {
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineRespellingCheck"] = _ => true,
            ["LEnginePhonemicCheck"] = _ => true,
        });

        Assert.True(timbre.CTimbrePhonemic);
    }

    [Fact]
    public void TimbrePhonemic_PhonemicPackWithoutRespelling_ReadsFalse()
    {
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineRespellingCheck"] = _ => false,
            ["LEnginePhonemicCheck"] = _ => true,
        });

        Assert.False(timbre.CTimbrePhonemic);
    }

    [Fact]
    public void TimbreSpoken_SilentPack_ReadsFalse()
    {
        CTimbre timbre = TTimbrePrepare(new() { ["LEngineSilentCheck"] = _ => true });

        Assert.False(timbre.CTimbreSpoken);
    }

    [Fact]
    public void TimbreContourRead_EmptyDesk_AsksThePackForNoLanguage()
    {
        List<string> asked = [];
        CTimbre timbre = TTimbrePrepare(new()
        {
            ["LEngineContourRead"] = args =>
            {
                asked.Add((string)args![0]!);
                return TInterface.TContourParse((string)args[1]!);
            },
        });

        IReadOnlyList<CContour> syllables = timbre.CTimbreContourRead("ma⁵⁵");

        CContour syllable = Assert.Single(syllables);
        Assert.Equal("ma⁵⁵", syllable.CContourText);
        Assert.Equal([5, 5], syllable.CContourLevels);
        Assert.Equal(["Theme.Contour.Top", "Theme.Contour.Top"], syllable.CContourKeys);
        Assert.True(syllable.CContourToned);
        Assert.Equal([string.Empty], asked);
    }

    [Fact]
    public void TimbreContourRead_LevelsOffScale_DropsThemAndTheTone()
    {
        CEditor editor = TTimbreEditorPrepare(new()
        {
            ["LEngineContourRead"] = _ => new List<LContour>
            {
                TInterface.TContourCreate("a", [0, 6, -1, int.MaxValue, int.MinValue]),
                TInterface.TContourCreate("b", [5, 9, 5, 0, 1, 3]),
                TInterface.TContourCreate(string.Empty, []),
                TInterface.TContourCreate("c", [1]),
            },
        });

        IReadOnlyList<CContour> syllables = editor.CEditorTimbre.CTimbreContourRead("a b c");

        Assert.Equal(["a", "b", string.Empty, "c"], syllables.Select(static syllable => syllable.CContourText));
        Assert.Empty(syllables[0].CContourLevels);
        Assert.Equal([5, 5, 1, 3], syllables[1].CContourLevels);
        Assert.Empty(syllables[2].CContourLevels);
        Assert.Equal([1], syllables[3].CContourLevels);
        Assert.Equal([false, true, false, true], syllables.Select(static syllable => syllable.CContourToned));
        Assert.All(
            syllables.SelectMany(static syllable => syllable.CContourLevels),
            level => Assert.Contains(level, editor.CEditorDisplay.CDisplaySound.CDisplaySoundScale));
    }

    [Fact]
    public void TimbreContourRead_NoSyllables_AnswersNone()
    {
        CTimbre timbre = TTimbrePrepare(new() { ["LEngineContourRead"] = _ => new List<LContour>() });

        Assert.Empty(timbre.CTimbreContourRead("a"));
    }

    [Fact]
    public void TimbreContourRead_TonalPack_AnswersEverySyllableReadyToDraw()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName);

        IReadOnlyList<CContour> syllables = editor.CEditorTimbre.CTimbreContourRead("ma˧˥ ma");

        Assert.Equal(2, syllables.Count);
        Assert.Equal("ma˧˥", syllables[0].CContourText);
        Assert.Equal([3, 5], syllables[0].CContourLevels);
        Assert.True(syllables[0].CContourToned);
        Assert.Equal("ma", syllables[1].CContourText);
        Assert.False(syllables[1].CContourToned);
    }

    [Fact]
    public void TimbreContourRead_ToneMarksInAPackWithoutTone_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreFlaggedPrepare(engine, "English");

        Assert.Empty(editor.CEditorTimbre.CTimbreContourRead("ma˧˥ ma"));
    }

    [Fact]
    public void TimbreContourRead_TonalPackWithoutToneMarks_AnswersNothing()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName);

        Assert.Empty(editor.CEditorTimbre.CTimbreContourRead("/həˈləʊ/"));
    }

    [Fact]
    public void ContourScale_ChaoLevels_RunsFromFiveDownToOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);

        Assert.Equal(new[] { 5, 4, 3, 2, 1 }, editor.CEditorDisplay.CDisplaySound.CDisplaySoundScale);
    }

    [Fact]
    public void TimbreAccentRead_HeldDraft_AnswersEveryRowReadyToShow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, "English"));
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestIpaCreate(draft, "ˈwɔːtə"));
        editor.CEditorDesk.TDeskVarietySet(true, 0, "British");
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, "ˈwɑːtɚ", 1));
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, string.Empty, 2));
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationVarietyCreate(draft, spoken, "American"));
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationRespellingCreate(draft, spoken, "WAH-ter"));
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAudioCreate(draft, spoken, "row.mp3", "Forvo"));

        CTimbreAccent accent = editor.CEditorTimbre.CTimbreAccentRead();

        Assert.Equal(new CRespellingMark(true, "[", "]"), accent.CTimbreAccentMark);
        Assert.Equal(CSounding.CSoundingVarietyRead("English", "British"), accent.CTimbreAccentPrimary);
        Assert.True(accent.CTimbreAccentFlagged);
        Assert.Equal(2, accent.CTimbreAccentRows.Count);
        CAccent row = accent.CTimbreAccentRows[0];
        Assert.Equal(spoken, row.CAccentId);
        Assert.Equal("WAH-ter", row.CAccentText);
        Assert.Equal("row.mp3", row.CAccentAudio);
        Assert.Equal(CSounding.CSoundingVarietyRead("English", "American"), row.CAccentVariety);
        Assert.Empty(accent.CTimbreAccentRows[1].CAccentText);
    }

    [Fact]
    public void TimbreAccentRead_RespellingHidden_PrintsThePhoneticText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(false);
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, "English"));
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, "ˈwɑːtɚ", 1));
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationRespellingCreate(draft, spoken, "WAH-ter"));

        CTimbreAccent accent = editor.CEditorTimbre.CTimbreAccentRead();

        Assert.False(accent.CTimbreAccentMark.CRespellingMarkShown);
        Assert.Equal("ˈwɑːtɚ", Assert.Single(accent.CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreAccentRead_EmptyDesk_AnswersTheMuteSheet()
    {
        CTimbre timbre = TTimbrePrepare([]);

        CTimbreAccent accent = timbre.CTimbreAccentRead();

        Assert.False(accent.CTimbreAccentMark.CRespellingMarkShown);
        Assert.Equal(CSounding.CSoundingVarietyRead(string.Empty, string.Empty), accent.CTimbreAccentPrimary);
        Assert.Empty(accent.CTimbreAccentRows);
        Assert.False(accent.CTimbreAccentFlagged);
    }

    [Fact]
    public void TimbreAccentSet_RespellingShown_WritesTheRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        CEditor editor = TTimbreAccentPrepare(engine, "ˈwɑːtɚ");
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        CAccentTyped typed = editor.CEditorTimbre.CTimbreAccentSet(spoken, "WAH-ter");

        Assert.Equal("WAH-ter", typed.CAccentTypedText);
        Assert.Equal("WAH-ter", Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
        engine.TEngineRespellingSave(false);
        Assert.Equal("ˈwɑːtɚ", Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreAccentSet_RespellingHidden_WritesThePhoneticText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(false);
        CEditor editor = TTimbreAccentPrepare(engine, "ˈwɑːtɚ");
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        CAccentTyped typed = editor.CEditorTimbre.CTimbreAccentSet(spoken, "ˈwɔːtə");

        Assert.Equal("ˈwɔːtə", typed.CAccentTypedText);
        Assert.Equal("ˈwɔːtə", Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreAccentSet_EmptyDesk_TakesNothingAndAnswersTheEmptyText()
    {
        CTimbre timbre = TTimbrePrepare([]);

        Assert.Equal(string.Empty, timbre.CTimbreAccentSet(1, "ˈwɔːtə").CAccentTypedText);
    }

    [Fact]
    public void TimbreAccentSet_WhileFilling_TakesNothingAndAnswersTheHeldText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreAccentPrepare(engine, "a");
        long spoken = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        string? answered = null;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
            answered = editor.CEditorTimbre.CTimbreAccentSet(spoken, "b").CAccentTypedText;

        editor.CEditorDesk.CDeskDraftResonate();

        Assert.Equal("a", answered);
        Assert.Equal("a", Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbrePronunciationAdd_AccentRow_PlacesTheBlankRowBelowIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreAccentPrepare(engine, "a", "b");
        long first = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        editor.CEditorTimbre.CTimbrePronunciationAdd(first);

        Assert.Equal(
            ["a", string.Empty, "b"],
            editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows.Select(static row => row.CAccentText));
    }

    [Fact]
    public void TimbrePronunciationAdd_PrimaryOrGoneRow_PlacesTheBlankRowFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreAccentPrepare(engine, "a");

        editor.CEditorTimbre.CTimbrePronunciationAdd(0);
        editor.CEditorTimbre.CTimbrePronunciationAdd(long.MaxValue);

        Assert.Equal(
            [string.Empty, string.Empty, "a"],
            editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows.Select(static row => row.CAccentText));
    }

    [Fact]
    public void TimbrePronunciationRemove_AccentRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreAccentPrepare(engine, "a", "b");
        long first = editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        editor.CEditorTimbre.CTimbrePronunciationRemove(first);

        Assert.Equal("b", Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbrePronunciationRemove_Primary_DropsThePrimary()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TTimbreAccentPrepare(engine, "a", "b");

        editor.CEditorTimbre.CTimbrePronunciationRemove(0);

        Assert.Equal("b", Assert.Single(editor.CEditorTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreFontRead_HeldDraft_AnswersThePackFontOfEachRole()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreAccentPrepare(engine).CEditorTimbre;

        Assert.Equal(
            new CFont("Segoe UI", 40, CFontSlant.CFontSlantTheme),
            timbre.CTimbreFontRead(CFontRole.CFontRoleHeadword));
        Assert.Equal(
            new CFont("Georgia, Segoe UI", 15, CFontSlant.CFontSlantTheme),
            timbre.CTimbreFontRead(CFontRole.CFontRoleExample));
        Assert.Equal(
            new CFont("Georgia, Segoe UI", 15, CFontSlant.CFontSlantItalic),
            timbre.CTimbreFontRead(CFontRole.CFontRoleGloss));
    }

    [Fact]
    public void TimbreFontRead_EmptyDesk_AnswersNothingSet()
    {
        CTimbre timbre = TTimbrePrepare([]);

        Assert.Equal(
            new CFont(null, null, CFontSlant.CFontSlantTheme),
            timbre.CTimbreFontRead(CFontRole.CFontRoleGlyph));
    }

    internal static CEditor TTimbreAccentPrepare(LEngine engine, params string[] accents)
    {
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, "English"));
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestIpaCreate(draft, "ˈwɔːtə"));
        for (int index = 0; index < accents.Length; index++)
        {
            editor.CEditorDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, accents[index], index + 1));
        }

        return editor;
    }

    internal static CEditor TTimbreFlaggedPrepare(LEngine engine, string language)
    {
        CEditor editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, language));
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestIpaCreate(draft, "a˥"));
        editor.CEditorDesk.TDeskVarietySet(true, 0, "British");
        return editor;
    }

    internal static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        return TTimbreEditorPrepare(answers).CEditorTimbre;
    }

    internal static CEditor TTimbreEditorPrepare(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers.TryAdd("add_LEngineFoldChanged", _ => null);
        answers.TryAdd("remove_LEngineFoldChanged", _ => null);
        return TInterfaceEditor.TEditorCreate(
            TEngineFake.TEngineStubCreate<LDraftPort>(),
            TInterfaceConduct.TEntryBundleCreate([]),
            TInterfaceConduct.TPhonologyBundleCreate(answers),
            TEngineFake.TEngineCreate<LSettingsPort>(answers),
            TEngineFake.TEngineStubCreate<LMediaPort>());
    }
}
