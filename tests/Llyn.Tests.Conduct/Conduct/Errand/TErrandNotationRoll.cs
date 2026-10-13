using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TErrandNotationRoll
{
    [Theory]
    [InlineData(true, "WAW-tuh")]
    [InlineData(false, "ˈwɔːtə")]
    public void LookupResonate_FoundReadings_ListsEachReadingReadyWithTheSearchMark(bool respelled, string text)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(respelled);
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, "English", "water");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        LForay foray = editor.TEditorFixtureDesk.TDeskForayStart(string.Empty)!;
        CNotationRoll? shown = null;
        errand.CErrandNotationChanged += roll => shown = roll;

        errand.TErrandLookupResonate(
            new CLookupStep("Wiktionary", 1, new("Wiktionary", "ˈwɔːtə", 1, true, "British", "WAW-tuh"), false),
            foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Wiktionary", 1, new("Wiktionary", "ˈwɑːtɚ", 1, true, string.Empty, null), false),
            foray);

        Assert.NotNull(shown);
        CNotationItem row = Assert.Single(shown.CNotationRollRows);
        Assert.True(row.CNotationItemReady);
        Assert.Equal(string.Empty, row.CNotationItemNotice);
        CRespellingMark mark = new(respelled, "[", "]");
        Assert.Equal(
            [
                new CNotationReading(
                    "ˈwɔːtə", text, CVariety.CVarietyRead("English", "British"), true, mark),
                new CNotationReading(
                    "ˈwɑːtɚ", "ˈwɑːtɚ", CVariety.CVarietyRead("English", string.Empty), false, mark),
            ],
            row.CNotationItemReading);
        foray.TForayCancel();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void LookupResonate_SchemedSearch_ListsThePhoneticBare()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, "English", "water");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        LForay foray = editor.TEditorFixtureDesk.TDeskForayStart("Yale")!;
        CNotationRoll? shown = null;
        errand.CErrandNotationChanged += roll => shown = roll;

        errand.TErrandLookupResonate(
            new CLookupStep("Yale", 0, new("Yale", "wɔ́ tə", 0, true, string.Empty, "WAW-tuh"), false), foray);

        Assert.NotNull(shown);
        CNotationReading reading = Assert.Single(Assert.Single(shown.CNotationRollRows).CNotationItemReading);
        Assert.Equal("wɔ́ tə", reading.CNotationReadingText);
        Assert.Equal(new CRespellingMark(false, string.Empty, string.Empty), reading.CNotationReadingMark);
        foray.TForayCancel();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void LookupResonate_NoReading_ChoosesMissingWhenReachedAndBrokenOtherwise()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotation.TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        LForay foray = editor.TEditorFixtureDesk.TDeskForayStart(string.Empty)!;
        CNotationRoll? shown = null;
        errand.CErrandNotationChanged += roll => shown = roll;

        errand.TErrandLookupResonate(
            new CLookupStep("Reached", 0, new("Reached", null, 0, true, string.Empty, null), false), foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Lost", 1, new("Lost", null, 1, false, string.Empty, null), false), foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Found", 2, new("Found", "hɪl", 2, true, string.Empty, null), false), foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Found", 2, new("Found", null, 2, true, string.Empty, null), false), foray);

        Assert.NotNull(shown);
        Assert.Equal(
            [("Phonetician.Missing", false, 0), ("Phonetician.Broken", false, 0), (string.Empty, true, 1)],
            shown.CNotationRollRows.Select(
                static row => (row.CNotationItemNotice, row.CNotationItemReady, row.CNotationItemReading.Count)));
        foray.TForayCancel();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void LookupResonate_SourcesThenEnd_KeepsThePackOrderAndEndsOnTheSchemeNotice()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotation.TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        editor.TEditorFixtureDesk.CDeskObserverAttach(static _ => { });
        List<CNotationRoll> shown = [];
        errand.CErrandNotationChanged += shown.Add;
        CNotationRoll started = errand.CErrandTranscriptionStart(0, "Yale");
        LForay foray = editor.TEditorFixtureDesk.TDeskForayStart("Yale")!;

        errand.TErrandLookupResonate(new CLookupStep("Late", 2, null, false), foray);
        errand.TErrandLookupResonate(new CLookupStep("Early", 1, null, false), foray);
        errand.TErrandLookupResonate(new CLookupStep("Early", 1, null, false), foray);
        errand.TErrandLookupResonate(new CLookupStep(string.Empty, 0, null, true), foray);

        Assert.True(started.CNotationRollSearching);
        Assert.Equal("Phonetician.Searching", started.CNotationRollNotice);
        Assert.Equal(4, shown.Count);
        Assert.True(shown[2].CNotationRollSearching);
        CNotationRoll ended = shown[3];
        Assert.Equal(["Early", "Late"], ended.CNotationRollRows.Select(static row => row.CNotationItemSource));
        Assert.All(
            ended.CNotationRollRows, static row => Assert.Equal("Phonetician.Searching", row.CNotationItemNotice));
        Assert.False(ended.CNotationRollSearching);
        Assert.False(ended.CNotationRollEmpty);
        Assert.Equal("Transcription.Empty", ended.CNotationRollNotice);
        foray.TForayCancel();
        gate.SetResult();
        editor.TEditorFinish(false);
    }
}
