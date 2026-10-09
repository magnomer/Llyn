using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineInflectionCell
{
    private const string TInflectionCellVocabulary = """
        {
          "parts": [ { "id": 1, "name": "Verb" } ],
          "features": [
            { "id": 1, "part": 1, "name": "tense" },
            { "id": 2, "part": 1, "name": "person" }
          ],
          "values": [
            { "id": 3, "feature": 1, "name": "past" },
            { "id": 4, "feature": 1, "name": "present" },
            { "id": 5, "feature": 2, "name": "first" },
            { "id": 6, "feature": 2, "name": "second" }
          ],
          "paradigms": [ { "part": 1, "cells": [[3, 5], [6, 4]] } ]
        }
        """;

    private const string TInflectionCellSource = """
        {
          "morphology": [
            {
              "name": "Wiktionary",
              "attempts": [
                {
                  "urls": [ "https://en.wiktionary.org/api/rest_v1/page/html/{word}" ],
                  "confirm": "lang-fx",
                  "readings": [
                    {
                      "variety": "3+5",
                      "strategy": "regex",
                      "match": "class=\"lang-fx c35\">([^<]+)<",
                      "group": 1,
                      "normalize": false
                    },
                    {
                      "variety": "4+6",
                      "strategy": "regex",
                      "match": "class=\"lang-fx c46\">([^<]+)<",
                      "group": 1,
                      "normalize": false
                    }
                  ]
                }
              ]
            }
          ]
        }
        """;

    private const string TInflectionCellPast = "<i class=\"lang-fx c35\">ran</i>";

    private const string TInflectionCellBody = TInflectionCellPast + "<i class=\"lang-fx c46\">runnest</i>";

    private static readonly TimeSpan TInflectionCellPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task InflectionStart_CellFound_StoresEveryValue()
    {
        using TLanguageFixture pack = TInflectionPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TInflectionCellBody, HttpStatusCode.OK);

        LEntry entry = await TInflectionCellRun(workspace, handler, pack.TLanguageFixtureName);

        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmShow(entry.LEntryId);
        Assert.Equal(["3+5", "4+6"], slots.Select(slot => slot.LParadigmSlotKey));
        Assert.Equal(["ran", "runnest"], slots.Select(slot => slot.LParadigmSlotInflection?.LInflectionText));
        Assert.All(slots, slot => Assert.Equal(LState.LStateSpecified, slot.LParadigmSlotState));
        Assert.All(
            slots,
            slot => Assert.Equal(
                slot.LParadigmSlotMorphologies.Select(morphology => morphology.LMorphologyId).Order(),
                slot.LParadigmSlotInflection!.LInflectionMorphology.Order()));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna;"));
    }

    [Fact]
    public async Task InflectionStart_CellMissed_MarksOnlyThatCellUnknown()
    {
        using TLanguageFixture pack = TInflectionPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TInflectionCellPast, HttpStatusCode.OK);

        LEntry entry = await TInflectionCellRun(workspace, handler, pack.TLanguageFixtureName);

        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmShow(entry.LEntryId);
        Assert.Equal(LState.LStateSpecified, slots[0].LParadigmSlotState);
        Assert.Equal("ran", slots[0].LParadigmSlotInflection?.LInflectionText);
        Assert.Equal(LState.LStateUnknown, slots[1].LParadigmSlotState);
        Assert.Null(slots[1].LParadigmSlotInflection);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna WHERE cell = '4+6';"));
    }

    [Fact]
    public async Task InflectionStart_AfterReopen_KeepsUnknownCell()
    {
        using TLanguageFixture pack = TInflectionPackCreate();
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TInflectionCellPast, HttpStatusCode.OK);
        LEntry entry = await TInflectionCellRun(workspace, handler, pack.TLanguageFixtureName);

        using LEngine reopened = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        int asked = handler.TSourceHandlerCount;
        reopened.TEngineInflectionStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(asked, handler.TSourceHandlerCount);
        Assert.False(reopened.TEngineInflectionCheck(entry.LEntryId));
        IReadOnlyList<LParadigmSlot> slots = reopened.TEngineParadigmShow(entry.LEntryId);
        Assert.Equal(LState.LStateSpecified, slots[0].LParadigmSlotState);
        Assert.Equal(LState.LStateUnknown, slots[1].LParadigmSlotState);
    }

    private static TLanguageFixture TInflectionPackCreate()
    {
        TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TInflectionCellSource);
        pack.TLanguageFixtureSave("vocabulary.json", TInflectionCellVocabulary);
        return pack;
    }

    private static async Task<LEntry> TInflectionCellRun(TWorkspace workspace, TSourceHandler handler, string language)
    {
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "run",
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "to move fast", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            ["Verb"]));
        Assert.Equal(2, engine.TEngineParadigmShow(entry.LEntryId).Count);

        engine.TEngineInflectionStart(entry.LEntryId);
        DateTime deadline = DateTime.UtcNow + TInflectionCellPatience;
        while (handler.TSourceHandlerCount == 0 || engine.TEngineInflectionCheck(entry.LEntryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the fetch to settle.");
            await Task.Delay(20);
        }

        return entry;
    }
}
