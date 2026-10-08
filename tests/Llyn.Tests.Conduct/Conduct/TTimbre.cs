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
        TEditorFixture editor = TTimbreEditorPrepare(new()
        {
            ["LEngineContourRead"] = _ => new List<LContour>
            {
                TInterface.TContourCreate("a", [0, 6, -1, int.MaxValue, int.MinValue]),
                TInterface.TContourCreate("b", [5, 9, 5, 0, 1, 3]),
                TInterface.TContourCreate(string.Empty, []),
                TInterface.TContourCreate("c", [1]),
            },
        });

        IReadOnlyList<CContour> syllables = editor.TEditorFixtureTimbre.CTimbreContourRead("a b c");

        Assert.Equal(["a", "b", string.Empty, "c"], syllables.Select(static syllable => syllable.CContourText));
        Assert.Empty(syllables[0].CContourLevels);
        Assert.Equal([5, 5, 1, 3], syllables[1].CContourLevels);
        Assert.Empty(syllables[2].CContourLevels);
        Assert.Equal([1], syllables[3].CContourLevels);
        Assert.Equal([false, true, false, true], syllables.Select(static syllable => syllable.CContourToned));
        Assert.All(
            syllables.SelectMany(static syllable => syllable.CContourLevels),
            level => Assert.Contains(level, editor.TEditorFixtureDisplay.CDisplayAccent.CDisplayAccentScale));
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
        CTimbre timbre = TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName).TEditorFixtureTimbre;

        IReadOnlyList<CContour> syllables = timbre.CTimbreContourRead("ma˧˥ ma");

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
        CTimbre timbre = TTimbreFlaggedPrepare(engine, "English").TEditorFixtureTimbre;

        Assert.Empty(timbre.CTimbreContourRead("ma˧˥ ma"));
    }

    [Fact]
    public void TimbreContourRead_TonalPackWithoutToneMarks_AnswersNothing()
    {
        using TLanguageFixture pack = TDisplayAccent.TDisplayPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreFlaggedPrepare(engine, pack.TLanguageFixtureName).TEditorFixtureTimbre;

        Assert.Empty(timbre.CTimbreContourRead("/həˈləʊ/"));
    }

    [Fact]
    public void ContourScale_ChaoLevels_RunsFromFiveDownToOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplayAccent area =
            new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay.CDisplayAccent;

        Assert.Equal(new[] { 5, 4, 3, 2, 1 }, area.CDisplayAccentScale);
    }

    [Fact]
    public void TimbreAccentRead_HeldDraft_AnswersEveryRowReadyToShow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.TEditorFixtureDesk.CDeskId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, "English"));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TRequestIpaCreate(draft, "ˈwɔːtə"));
        editor.TEditorFixtureDesk.TDeskVarietySet(true, 0, "British");
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, "ˈwɑːtɚ", 1));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, string.Empty, 2));
        long spoken = editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationVarietyCreate(draft, spoken, "American"));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationRespellingCreate(draft, spoken, "WAH-ter"));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationAudioCreate(draft, spoken, "row.mp3", "Forvo"));

        CTimbreAccent accent = editor.TEditorFixtureTimbre.CTimbreAccentRead();

        Assert.Equal(new CRespellingMark(true, "[", "]"), accent.CTimbreAccentMark);
        Assert.Equal(CVariety.CVarietyRead("English", "British"), accent.CTimbreAccentPrimary);
        Assert.True(accent.CTimbreAccentFlagged);
        Assert.Equal(2, accent.CTimbreAccentRows.Count);
        CAccent row = accent.CTimbreAccentRows[0];
        Assert.Equal(spoken, row.CAccentId);
        Assert.Equal("WAH-ter", row.CAccentText);
        Assert.Equal("row.mp3", row.CAccentAudio);
        Assert.Equal(CVariety.CVarietyRead("English", "American"), row.CAccentVariety);
        Assert.Empty(accent.CTimbreAccentRows[1].CAccentText);
    }

    [Fact]
    public void TimbreAccentRead_RespellingHidden_PrintsThePhoneticText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(false);
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.TEditorFixtureDesk.CDeskId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, "English"));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, "ˈwɑːtɚ", 1));
        long spoken = editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TPronunciationRespellingCreate(draft, spoken, "WAH-ter"));

        CTimbreAccent accent = editor.TEditorFixtureTimbre.CTimbreAccentRead();

        Assert.False(accent.CTimbreAccentMark.CRespellingMarkShown);
        Assert.Equal("ˈwɑːtɚ", Assert.Single(accent.CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreAccentRead_EmptyDesk_AnswersTheMuteSheet()
    {
        CTimbre timbre = TTimbrePrepare([]);

        CTimbreAccent accent = timbre.CTimbreAccentRead();

        Assert.False(accent.CTimbreAccentMark.CRespellingMarkShown);
        Assert.Equal(CVariety.CVarietyRead(string.Empty, string.Empty), accent.CTimbreAccentPrimary);
        Assert.Empty(accent.CTimbreAccentRows);
        Assert.False(accent.CTimbreAccentFlagged);
    }

    [Fact]
    public void TimbreAccentSet_RespellingShown_WritesTheRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        CTimbre timbre = TTimbreAccentPrepare(engine, "ˈwɑːtɚ").TEditorFixtureTimbre;
        long spoken = timbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        CAccentTyped typed = timbre.CTimbreAccentSet(spoken, "WAH-ter");

        Assert.Equal("WAH-ter", typed.CAccentTypedText);
        Assert.Equal("WAH-ter", Assert.Single(timbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
        engine.TEngineRespellingSave(false);
        Assert.Equal("ˈwɑːtɚ", Assert.Single(timbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreAccentSet_RespellingHidden_WritesThePhoneticText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(false);
        CTimbre timbre = TTimbreAccentPrepare(engine, "ˈwɑːtɚ").TEditorFixtureTimbre;
        long spoken = timbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        CAccentTyped typed = timbre.CTimbreAccentSet(spoken, "ˈwɔːtə");

        Assert.Equal("ˈwɔːtə", typed.CAccentTypedText);
        Assert.Equal("ˈwɔːtə", Assert.Single(timbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
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
        TEditorFixture editor = TTimbreAccentPrepare(engine, "a");
        long spoken = editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;
        string? answered = null;
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftChanged += _ =>
            answered = editor.TEditorFixtureTimbre.CTimbreAccentSet(spoken, "b").CAccentTypedText;

        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftResonate();

        Assert.Equal("a", answered);
        Assert.Equal("a", Assert.Single(editor.TEditorFixtureTimbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbrePronunciationAdd_AccentRow_PlacesTheBlankRowBelowIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreAccentPrepare(engine, "a", "b").TEditorFixtureTimbre;
        long first = timbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        timbre.CTimbrePronunciationAdd(first);

        Assert.Equal(
            ["a", string.Empty, "b"],
            timbre.CTimbreAccentRead().CTimbreAccentRows.Select(static row => row.CAccentText));
    }

    [Fact]
    public void TimbrePronunciationAdd_PrimaryOrGoneRow_PlacesTheBlankRowFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreAccentPrepare(engine, "a").TEditorFixtureTimbre;

        timbre.CTimbrePronunciationAdd(0);
        timbre.CTimbrePronunciationAdd(long.MaxValue);

        Assert.Equal(
            [string.Empty, string.Empty, "a"],
            timbre.CTimbreAccentRead().CTimbreAccentRows.Select(static row => row.CAccentText));
    }

    [Fact]
    public void TimbrePronunciationRemove_AccentRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreAccentPrepare(engine, "a", "b").TEditorFixtureTimbre;
        long first = timbre.CTimbreAccentRead().CTimbreAccentRows[0].CAccentId;

        timbre.CTimbrePronunciationRemove(first);

        Assert.Equal("b", Assert.Single(timbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbrePronunciationRemove_Primary_DropsThePrimary()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreAccentPrepare(engine, "a", "b").TEditorFixtureTimbre;

        timbre.CTimbrePronunciationRemove(0);

        Assert.Equal("b", Assert.Single(timbre.CTimbreAccentRead().CTimbreAccentRows).CAccentText);
    }

    [Fact]
    public void TimbreFontRead_HeldDraft_AnswersThePackFontOfEachRole()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CTimbre timbre = TTimbreAccentPrepare(engine).TEditorFixtureTimbre;

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

    internal static TEditorFixture TTimbreAccentPrepare(LEngine engine, params string[] accents)
    {
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        CDesk desk = editor.TEditorFixtureDesk;
        long draft = desk.CDeskId;
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, "English"));
        desk.TDeskDefer(TInterface.TRequestIpaCreate(draft, "ˈwɔːtə"));
        for (int index = 0; index < accents.Length; index++)
        {
            desk.TDeskDefer(TInterface.TPronunciationAdditionCreate(draft, accents[index], index + 1));
        }

        return editor;
    }

    internal static TEditorFixture TTimbreFlaggedPrepare(LEngine engine, string language)
    {
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long draft = editor.TEditorFixtureDesk.CDeskId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TRequestLanguageCreate(draft, language));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TRequestIpaCreate(draft, "a˥"));
        editor.TEditorFixtureDesk.TDeskVarietySet(true, 0, "British");
        return editor;
    }

    internal static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        return TTimbreEditorPrepare(answers).TEditorFixtureTimbre;
    }

    internal static TEditorFixture TTimbreEditorPrepare(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers.TryAdd("add_LEngineFoldChanged", _ => null);
        answers.TryAdd("remove_LEngineFoldChanged", _ => null);
        return new(TInterfaceEditor.TEditorCreate(
            TEngineFake.TEngineStubCreate<LDraftPort>(),
            TInterfaceConduct.TEntryBundleCreate([]),
            TInterfaceConduct.TPhonologyBundleCreate(answers),
            TEngineFake.TEngineCreate<LSettingsPort>(answers),
            TEngineFake.TEngineStubCreate<LMediaPort>()));
    }
}
