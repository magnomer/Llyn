using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineReflex
{
    [Fact]
    public void ReflexRuleRead_FixturePack_ReadsRulesInPackOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LReflexRule> rules = engine.TEngineReflexRead(pack.TLanguageFixtureName);

        Assert.Equal(["Korean", "Japanese", "Mandarin"], rules.Select(rule => rule.LReflexRuleLanguage));
        Assert.Equal("{sound}[[({initial})]] | {sense}", rules[0].LReflexRuleTemplate);
        Assert.Equal("{text}", rules[1].LReflexRuleTemplate);
        Assert.True(rules[1].LReflexRuleEvery);
        Assert.Equal("Too fast", rules[2].LReflexRuleBusy);
        Assert.Empty(engine.TEngineReflexRead("English"));
    }

    [Fact]
    public async Task ReflexFind_ThreeRules_ReadsRowsKindsNotesAndMark()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "弄", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "롱(농) | 희롱할", "", true),
                ("Japanese", "Go-on", "る", "", false),
                ("Japanese", "Kan-on", "ろう", "", true),
                ("Mandarin", "", "nʊŋ⁵¹", "nòng", false),
                ("Mandarin", "", "lʊŋ⁵¹", "nòng", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal("Beijing", found[3].LReflexDraftRegion);
        Assert.Equal(string.Empty, found[0].LReflexDraftRegion);
    }

    [Fact]
    public async Task ReflexFind_OptionalSegmentUncaptured_DropsTheSegment()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "璋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal("장 | 홀", found[0].LReflexDraftText);
    }

    [Fact]
    public async Task ReflexFind_NoMainCaptured_MarksFirstRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "璋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "장 | 홀", "", true),
                ("Japanese", "Kan-on", "しょう", "", true),
                ("Mandarin", "", "ʈ͡ʂɑŋ⁵⁵", "zhāng", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_KoreanPairOfSeveralWords_ReadsLastWordAsReading()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Korean", "url": "https://example.test/api?query={word}",
              "match": "(?<=\"items\":\\[\\{[^\\[\\]]*\"expKoreanPron\":\"[^\"]*)
            """.TrimEnd()
            + """(?<note>[^\" ,/][^\",/]*?) (?<text>[^\" ,/]+)(?=[\",/])", "every": true, "first": true } ] }""");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/api?query=%E6%83%A1"] =
                    """{"items":[{"expEntry":"惡","expKoreanPron":"악할 악, 미워할 오, 강 이름 호","x":1}]}""",
            }));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "惡", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "악", "악할", true),
                ("Korean", "", "오", "미워할", false),
                ("Korean", "", "호", "강 이름", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_OneTextAcrossKinds_MarksEveryRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "安", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "안 | 편안", "", true),
                ("Japanese", "Go-on", "あん", "", true),
                ("Japanese", "Kan-on", "あん", "", true),
                ("Mandarin", "", "än⁵⁵", "ān", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_BorrowerPack_FillsRespellingBesideText()
    {
        using TLanguageFixture borrower = TLanguageFixture.TLanguageFixtureCreate(
            """{ "phonemic": true, "respelling": [ { "name": "Plain", "rules": [["ʊ", "u"], ["⁵¹", "4"]] } ] }""");
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            TReflexFixture.TReflexPack.Replace("\"Mandarin\"", "\"" + borrower.TLanguageFixtureName + "\""));
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "弄", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal("nʊŋ⁵¹", found[3].LReflexDraftText);
        Assert.Equal("nuŋ4", found[3].LReflexDraftRespelling);
        Assert.Equal("luŋ4", found[4].LReflexDraftRespelling);
        Assert.Equal("nòng", found[3].LReflexDraftNote);
        Assert.Equal("ろう", found[2].LReflexDraftText);
        Assert.Equal(string.Empty, found[2].LReflexDraftRespelling);
    }

    [Fact]
    public async Task ReflexFind_TwoCharacters_MergesRowsOfOneKindAndNote()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "弄璋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "롱(농) | 희롱할 장 | 홀", "", true),
                ("Japanese", "Go-on", "る", "", false),
                ("Japanese", "Kan-on", "ろう しょう", "", true),
                ("Mandarin", "", "nʊŋ⁵¹", "nòng", false),
                ("Mandarin", "", "lʊŋ⁵¹", "nòng", false),
                ("Mandarin", "", "ʈ͡ʂɑŋ⁵⁵", "zhāng", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_RecastAndSuperscript_MovesToneAndRaisesDigits()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexWuPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexWuPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "屋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Wu", "", "oʔ⁵", "oq⁷", true),
                ("Wu", "", "ʊʔ⁵", "ok⁷", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal("literary", found[1].LReflexDraftRemark);
    }
}
