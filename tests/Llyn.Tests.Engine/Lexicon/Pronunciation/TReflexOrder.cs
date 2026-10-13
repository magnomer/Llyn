using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflexOrder
{
    private const string TReflexOrderPack =
        """
        { "order": { "language": ["Korean", "Japanese", "Mandarin"], "kind": { "Japanese": ["Kan-on", "Go-on"] } } }
        """;

    [Fact]
    public void ReflexOrderSort_UndeclaredLanguageAndKind_FollowDeclaredByName()
    {
        LReflexOrder order = TInterface.TReflexOrderCreate(
            ["Mandarin", "Japanese"],
            new Dictionary<string, IReadOnlyList<string>> { ["Japanese"] = ["Kan-on"] });

        IReadOnlyList<LReflexDraft> sorted = order.TReflexOrderSort(
            [
                TInterface.TReflexDraftCreate("Wu", "", "ŋu"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
                TInterface.TReflexDraftCreate("Cantonese", "", "lʊŋ⁶"),
                TInterface.TReflexDraftCreate("Japanese", "", "ろ"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ら"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ろう", true),
                TInterface.TReflexDraftCreate("Mandarin", "", "nʊŋ⁵¹"),
            ]);

        Assert.Equal(
            [
                ("Mandarin", "", "nʊŋ⁵¹"),
                ("Japanese", "Kan-on", "ろう"),
                ("Japanese", "Kan-on", "ら"),
                ("Japanese", "", "ろ"),
                ("Japanese", "Go-on", "る"),
                ("Cantonese", "", "lʊŋ⁶"),
                ("Wu", "", "ŋu"),
            ],
            sorted.Select(row => (row.LReflexDraftLanguage, row.LReflexDraftKind, row.LReflexDraftText)));
    }

    [Fact]
    public void PackLoad_ClassicalChinese_DeclaresOrderApartFromRules()
    {
        LLanguage pack = TInterface.TLanguageLoad("Classical Chinese");

        Assert.Equal(
            ["Korean", "Japanese", "Mandarin", "Cantonese", "Gan", "Hakka", "Jin", "Southern Min", "Wu", "Xiang"],
            pack.LLanguageReflexOrder.LReflexOrderLanguages);
        Assert.Equal(
            ["Go-on", "Kan-on", "Tō-on", "Sō-on", "Kan'yō-on"],
            pack.LLanguageReflexOrder.LReflexOrderKinds["Japanese"]);
        Assert.Equal(
            ["Korean", "Mandarin", "Cantonese", "Japanese"],
            pack.LLanguageReflexRules.Take(4).Select(rule => rule.LReflexRuleLanguage));
    }

    [Fact]
    public void ReflexRead_StoredOutOfPackOrder_ListsInDeclaredOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexOrderPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "弄",
            pack.TLanguageFixtureName,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes:
            [
                TInterface.TReflexDraftCreate("Wu", "", "ŋu"),
                TInterface.TReflexDraftCreate("Mandarin", "", "nʊŋ⁵¹"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
                TInterface.TReflexDraftCreate("Cantonese", "", "lʊŋ⁶"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ら"),
                TInterface.TReflexDraftCreate("Japanese", "Kan-on", "ろう", true),
                TInterface.TReflexDraftCreate("Korean", "", "롱"),
            ]));
        string[] declared = ["Korean", "Japanese", "Japanese", "Japanese", "Mandarin", "Cantonese", "Wu"];

        IReadOnlyList<LReflexDraft> shown = engine.TEntryReflexRead(entry.LEntryId);
        LDraft edited = engine.TEngineDraftStart("Input", entry.LEntryId);
        LLiveryPage livery = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LRig rig = workspace.TWorkspaceRigCreate();
        LPortraitPage portrait = TInterface.TPortraitClerkCreate(rig)
            .TPortraitClerkRead(entry.LEntryId, TInterface.TPortraitLabelRead());
        LMarkupEntry exported = Assert.IsType<LMarkupEntry>(
            TInterface.TMarkupExportCreate(rig).TMarkupClerkLoad(entry.LEntryId));

        Assert.Equal(declared, shown.Select(row => row.LReflexDraftLanguage));
        Assert.Equal(["롱", "ろう", "ら", "る", "nʊŋ⁵¹", "lʊŋ⁶", "ŋu"], shown.Select(row => row.LReflexDraftText));
        Assert.Equal(
            shown.Select(row => row.LReflexDraftId),
            edited.LDraftContent.LEntryDraftReflexes.Select(row => row.LReflexDraftId));
        Assert.Equal(declared, livery.LLiveryPageDraft.LEntryDraftReflexes.Select(row => row.LReflexDraftLanguage));
        Assert.Equal(declared.Length, portrait.LPortraitPageLine.Count);
        Assert.All(
            declared.Zip(portrait.LPortraitPageLine),
            pair => Assert.StartsWith(pair.First, pair.Second.LPortraitLineLabel));
        Assert.Equal(declared, exported.LMarkupEntryReflex.Select(row => row.LReflexDraftLanguage));
        Assert.False(engine.TEngineDraftCheck(edited.LDraftId));
        Assert.Equal(
            0, workspace.TWorkspaceCountRead("SELECT position FROM reflex WHERE language = 'Wu';"));
    }

    [Fact]
    public void EntryUpdate_SameRowsInAnotherOrder_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る"),
            ]));
        const string changes = "SELECT COUNT(*) FROM revision_change WHERE target_type = 'reflex';";
        long logged = workspace.TWorkspaceCountRead(changes);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;

        Assert.Equal(["Japanese", "Korean"], loaded.LEntryDraftReflexes.Select(row => row.LReflexDraftLanguage));

        engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        Assert.Equal(logged, workspace.TWorkspaceCountRead(changes));
        Assert.Equal(
            0, workspace.TWorkspaceCountRead("SELECT position FROM reflex WHERE language = 'Korean';"));
    }

    [Fact]
    public void DraftMatch_SameRowsInAnotherOrder_IsNoChange()
    {
        LEntryDraft content = TInterface.TReflexDraftCreate(
            [
                TInterface.TReflexDraftCreate("Korean", "", "롱", id: 4),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "る", id: 9),
            ]);

        Assert.True(TInterface.TDraftMatch(
            content, content with { LEntryDraftReflexes = [.. content.LEntryDraftReflexes.Reverse()] }));
        Assert.False(TInterface.TDraftMatch(
            content,
            content with
            {
                LEntryDraftReflexes = [content.LEntryDraftReflexes[1], content.LEntryDraftReflexes[1]],
            }));
    }
}
