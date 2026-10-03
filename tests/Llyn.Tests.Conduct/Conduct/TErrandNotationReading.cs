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

public sealed class TErrandNotationReading
{
    [Fact]
    public async Task ReadingSet_PrimarySearch_WritesTheIpaAtOnceWithItsVariety()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotation.TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        errand.CErrandTranscriptionStart(0, string.Empty);

        errand.CErrandReadingSet("hɪl", "British");

        LPronunciationDraft? spoken = editor.CEditorDesk.TDeskHeldRead()?.LDraftContent.LEntryDraftPronunciation;
        Assert.Equal("hɪl", spoken?.LPronunciationDraftIpa);
        Assert.Equal("British", spoken?.LPronunciationDraftVariety);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ReadingSet_AccentSearch_WritesTheRowAndSkipsItOnceRemoved()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotation.TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        editor.CEditorTimbre.CTimbrePronunciationAdd(0);
        long accent = editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId;
        errand.CErrandTranscriptionStart(accent, string.Empty);

        errand.CErrandReadingSet("hɪl", "British");

        IReadOnlyList<LPronunciationDraft> written =
            editor.CEditorDesk.TDeskHeldRead()!.LDraftContent.LEntryDraftPronunciations;
        Assert.Equal(
            [(string.Empty, string.Empty), ("hɪl", "British")],
            written.Select(static row => (row.LPronunciationDraftIpa, row.LPronunciationDraftVariety)));
        editor.CEditorTimbre.CTimbrePronunciationRemove(accent);
        errand.CErrandReadingSet("hɪːl", "British");
        Assert.Equal(
            [string.Empty],
            editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations.Select(
                static row => row.LPronunciationDraftIpa));
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ReadingSet_SchemedSearch_WritesTheTranscriptionAtOnceAndNoVariety()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotation.TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        long entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hill",
            pack.TLanguageFixtureName,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a rise", 1)],
            [],
            transcriptions: [TInterface.TTranscriptionDraftCreate("Yale", string.Empty)])).LEntryId;
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        long row = Assert.Single(editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftTranscriptions)
            .LTranscriptionDraftId;
        errand.CErrandTranscriptionStart(row, "Yale");

        errand.CErrandReadingSet("hil", "British");

        LEntryDraft? content = editor.CEditorDesk.TDeskHeldRead()?.LDraftContent;
        Assert.Equal("hil", Assert.Single(content?.LEntryDraftTranscriptions ?? []).LTranscriptionDraftText);
        Assert.Equal(string.Empty, content?.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ReadingSet_NoSearchOrFillingDesk_WritesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotation.TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        errand.CErrandReadingSet("hɪl", "British");
        errand.CErrandTranscriptionStart(0, string.Empty);
        bool filling = false;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskFilling;
            errand.CErrandReadingSet("hɪl", "British");
        };

        editor.CEditorDesk.CDeskDraftResonate();

        Assert.True(filling);
        Assert.Equal(
            string.Empty,
            editor.CEditorDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftIpa
            ?? string.Empty);
        gate.SetResult();
        editor.TEditorFinish(false);
    }
}
