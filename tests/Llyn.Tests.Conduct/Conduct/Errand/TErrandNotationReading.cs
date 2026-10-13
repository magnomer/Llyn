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
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        errand.CErrandTranscriptionStart(0, string.Empty);

        errand.CErrandReadingSet("hɪl", "British");

        LPronunciationDraft? spoken = editor.TEditorFixtureDesk.TDeskHeldRead()?.LDraftContent.LEntryDraftPronunciation;
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
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CDesk desk = editor.TEditorFixtureDesk;
        CErrand errand = desk.CDeskErrand;
        editor.TEditorFixtureTimbre.CTimbrePronunciationAdd(0);
        long accent = desk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId;
        errand.CErrandTranscriptionStart(accent, string.Empty);

        errand.CErrandReadingSet("hɪl", "British");

        IReadOnlyList<LPronunciationDraft> written =
            desk.TDeskHeldRead()!.LDraftContent.LEntryDraftPronunciations;
        Assert.Equal(
            [(string.Empty, string.Empty), ("hɪl", "British")],
            written.Select(static row => (row.LPronunciationDraftIpa, row.LPronunciationDraftVariety)));
        editor.TEditorFixtureTimbre.CTimbrePronunciationRemove(accent);
        errand.CErrandReadingSet("hɪːl", "British");
        Assert.Equal(
            [string.Empty],
            desk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations.Select(
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
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(entry);
        CDesk desk = editor.TEditorFixtureDesk;
        CErrand errand = desk.CDeskErrand;
        long row = Assert.Single(desk.TDeskRead()!.LDraftContent.LEntryDraftTranscriptions)
            .LTranscriptionDraftId;
        errand.CErrandTranscriptionStart(row, "Yale");

        errand.CErrandReadingSet("hil", "British");

        LEntryDraft? content = desk.TDeskHeldRead()?.LDraftContent;
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
        TEditorFixture editor = TErrandNotation.TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CDesk desk = editor.TEditorFixtureDesk;
        CErrand errand = desk.CDeskErrand;
        errand.CErrandReadingSet("hɪl", "British");
        errand.CErrandTranscriptionStart(0, string.Empty);
        bool filling = false;
        desk.CDeskDraft.CDeskDraftChanged += _ =>
        {
            filling = desk.CDeskDraft.CDeskDraftFilling;
            errand.CErrandReadingSet("hɪl", "British");
        };

        desk.CDeskDraft.CDeskDraftResonate();

        Assert.True(filling);
        Assert.Equal(
            string.Empty,
            desk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftIpa
            ?? string.Empty);
        gate.SetResult();
        editor.TEditorFinish(false);
    }
}
