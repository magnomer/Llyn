using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineLiveryPage
{
    private const string TEngineLiveryReflex =
        """
        { "reflex": [
            { "language": "Jin", "url": "https://example.test/{word}", "match": "(?<text>x)", "folded": true },
            { "language": "Wu", "url": "https://example.test/{word}", "match": "(?<text>x)" } ] }
        """;

    private const string TEngineLiveryFanqie =
        """
        { "fanqie": [
            { "name": "Broad", "url": "https://example.test/broad",
              "form": { "word": "{word}" }, "match": "<span>{word}</span>", "busy": "Too fast" } ] }
        """;

    private const string TEngineLiveryScript =
        """
        { "script": [
            { "name": "Seal", "url": "https://example.test/seal/search",
              "form": { "Character": "{word}" },
              "match": "<td class=\"Cell\"><img src=\"([^\"]+)\" />(.*?)<td>",
              "image": 1, "caption": 2, "prefix": "https://example.test" } ] }
        """;

    private static readonly TimeSpan TEngineLiveryPatience = TimeSpan.FromSeconds(5);

    private static readonly Dictionary<string, string> TEngineLiveryPages = new()
    {
        ["https://example.test/seal/search"] = "<td class=\"Cell\"><img src=\"/image?text=a\" /><br />Shuowen<td>",
        ["https://example.test/image?text=a"] = "PNG-A",
    };

    [Fact]
    public void LiveryRead_StoredEntry_KeepsHeaderAndStamps()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, "A plain note", [TInterface.TCardCreate("a liquid", 1)], []));
        engine.TEngineFavoriteSave(water.LEntryId);
        engine.TEngineGraspSave(water.LEntryId, 3);

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(water.LEntryId));

        Assert.Equal("water", page.LLiveryPageDraft.LEntryDraftHeadword);
        Assert.Equal("A plain note", page.LLiveryPageDraft.LEntryDraftNote);
        Assert.True(page.LLiveryPageFavorite);
        Assert.Equal(3, page.LLiveryPageGrasp);
        Assert.NotEmpty(page.LLiveryPageCreated);
        Assert.NotEmpty(page.LLiveryPageUpdated);
    }

    [Fact]
    public void LiveryRead_MissingEntry_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TLiveryRead(987654));
    }

    [Fact]
    public void LiveryRead_FoldedReflex_KeepsFoldedLanguage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineLiveryReflex);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "整",
            pack.TLanguageFixtureName,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("orderly", 1)],
            [],
            reflexes:
            [
                TInterface.TReflexDraftCreate("Jin", string.Empty, "tɕiŋ"),
                TInterface.TReflexDraftCreate("Wu", string.Empty, "tsən"),
            ]));

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        Assert.Equal(["Jin"], page.LLiveryPageFolded);
        Assert.Equal(["Jin", "Wu"], page.LLiveryPageDraft.LEntryDraftReflexes.Select(row => row.LReflexDraftLanguage));
        Assert.Equal([true, false], page.LLiveryPageGuise.Select(static guise => guise.LReflexGuiseFolded));
    }

    [Fact]
    public void LiveryRead_FoldedStoredCard_KeepsCardIdInFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("to come apart", 1), TInterface.TCardCreate("to stop", 2)],
            []));
        LEntryDraft held = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        long folded = held.LEntryDraftMeanings[0].LCardDraftId;
        engine.TEngineFoldSave(entry.LEntryId, folded);

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        Assert.Equal([folded], page.LLiveryPageFold);
    }

    [Fact]
    public void LiveryRead_StoredFanqie_KeepsGroups()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineLiveryFanqie);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "弄", language, string.Empty, string.Empty, [TInterface.TCardCreate("play", 1)], []));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, "弄", [TInterface.TFanqieRowCreate("弄", "Broad", "盧貢", "來", "東", "東", "一", "去")]);

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        LFanqieGroup group = Assert.Single(page.LLiveryPageFanqie);
        Assert.Equal("Broad", group.LFanqieGroupLabel);
        Assert.Equal("來", Assert.Single(group.LFanqieGroupRows).LFanqieRowInitial);
    }

    [Fact]
    public async Task LiveryRead_StoredScript_KeepsStyleGroups()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineLiveryScript);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineLiveryPages));
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "整", pack.TLanguageFixtureName, string.Empty, string.Empty, [TInterface.TCardCreate("orderly", 1)], []));

        Assert.Empty(Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId)).LLiveryPageScript);
        engine.TEngineScriptStart(entry.LEntryId);
        DateTime deadline = DateTime.UtcNow + TEngineLiveryPatience;
        while (engine.TEngineScriptCheck(entry.LEntryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the script fetch to settle.");
            await Task.Delay(20);
        }

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        LScriptGroup group = Assert.Single(page.LLiveryPageScript);
        Assert.Equal("Seal", group.LScriptGroupStyle);
        Assert.Single(group.LScriptGroupImages);
    }

    [Fact]
    public void LiveryRead_SpanishVerbWithMorphologyOff_CarriesStoredFormInTheExpandedTable()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        engine.TEngineAnalysisSave(false);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hablar",
            "Spanish",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            ["Verb"]));
        LParadigmSlot slot = engine.TEngineParadigmShow(entry.LEntryId)
            .Single(static row => string.Equals(row.LParadigmSlotKey, "9+12+17+20", StringComparison.Ordinal));
        engine.TEntryInflectionSave(entry.LEntryId, [
            TInterfaceInflection.TInflectionCreate(
                entry.LEntryId,
                0,
                "hablo",
                null,
                slot.LParadigmSlotSpeech.LSpeechValueId,
                [.. slot.LParadigmSlotMorphologies.Select(static morphology => morphology.LMorphologyId)]),
        ]);

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        LParadigmView view = Assert.IsType<LParadigmView>(page.LLiveryPageInflection);
        LParadigmLine present = view.LParadigmViewExpanded.LParadigmTableLines[0];
        Assert.Equal("Inflection.Present", present.LParadigmLineLabel);
        Assert.Equal("hablo", present.LParadigmLineForms[0].LParadigmFormText);
        Assert.Equal("Paradigm.Absent", present.LParadigmLineForms[1].LParadigmFormTip);
    }

    [Fact]
    public void LiveryRead_CardWithStoredTarget_KeepsTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry target = engine.TEngineTranslationCreate("부수다", "Korean");
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "to come apart", [], [], [target.LEntryId], [], [], 1)],
            []));

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        long card = Assert.Single(page.LLiveryPageDraft.LEntryDraftMeanings).LCardDraftId;
        LTranslationTarget found = Assert.Single(page.LLiveryPageTarget[card]);
        Assert.Equal((target.LEntryId, "부수다"), (found.LTranslationTargetId, found.LTranslationTargetHeadword));
    }

    [Fact]
    public void LiveryRead_LinkedEntry_KeepsIncomingUsage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry target = engine.TEngineTranslationCreate("부수다", "Korean");
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "to come apart", [], [], [target.LEntryId], [], [], 1)],
            []));

        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(target.LEntryId));

        Assert.Equal(["break"], page.LLiveryPageIncoming.Select(static usage => usage.LUsageHeadword));
    }

    [Fact]
    public void LiveryFormat_FoldedLanguage_PutsRowsInsideDetails()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineLiveryReflex);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "整",
            pack.TLanguageFixtureName,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("orderly", 1)],
            [],
            reflexes:
            [
                TInterface.TReflexDraftCreate("Jin", string.Empty, "tɕiŋ"),
                TInterface.TReflexDraftCreate("Wu", string.Empty, "tsən"),
            ]));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        int open = body.IndexOf("<details", StringComparison.Ordinal);
        int close = body.IndexOf("</details>", StringComparison.Ordinal);
        int folded = body.IndexOf("tɕiŋ", StringComparison.Ordinal);
        int shown = body.IndexOf("tsən", StringComparison.Ordinal);
        Assert.True(open >= 0 && open < folded && folded < close);
        Assert.True(shown >= 0 && shown < open);
        Assert.Contains("<summary>Reflex.More</summary>", body, StringComparison.Ordinal);
    }
}
