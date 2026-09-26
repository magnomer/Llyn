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
    public async Task ReflexStart_ThreeRules_StoresRowsRomanizationsAndMark()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "弄", pack.TLanguageFixtureName);

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
    public async Task ReflexStart_OptionalSegmentUncaptured_DropsTheSegment()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "璋", pack.TLanguageFixtureName);

        Assert.Equal("장 | 홀", found[0].LReflexDraftText);
    }

    [Fact]
    public async Task ReflexStart_NoMainCaptured_MarksFirstRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "璋", pack.TLanguageFixtureName);

        Assert.Equal(
            [
                ("Korean", "", "장 | 홀", "", true),
                ("Japanese", "Kan-on", "しょう", "", true),
                ("Mandarin", "", "ʈ͡ʂɑŋ⁵⁵", "zhāng", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
    }

    [Fact]
    public async Task ReflexStart_KoreanMultiWordPair_StoresLastWordAsReading()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Korean", "url": "https://example.test/api?query={word}",
              "match": "(?<=\"items\":\\[\\{[^\\[\\]]*\"expKoreanPron\":\"[^\"]*)
            """.TrimEnd()
            + """(?<meaning>[^\" ,/][^\",/]*?) (?<text>[^\" ,/]+)(?=[\",/])", "every": true, "first": true } ] }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/api?query=%E6%83%A1"] =
                    """{"items":[{"expEntry":"惡","expKoreanPron":"악할 악, 미워할 오, 강 이름 호","x":1}]}""",
            }));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "惡", pack.TLanguageFixtureName);

        Assert.Equal(
            [
                ("Korean", "", "악", "", true),
                ("Korean", "", "오", "", false),
                ("Korean", "", "호", "", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal(
            ["악할", "미워할", "강 이름"],
            found.Select(row => row.LReflexDraftMeaning));
    }

    [Fact]
    public async Task ReflexStart_OneTextAcrossKinds_MarksEveryRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "安", pack.TLanguageFixtureName);

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
    public async Task ReflexStart_BorrowerPack_FillsRespellingBesideText()
    {
        using TLanguageFixture borrower = TLanguageFixture.TLanguageFixtureCreate(
            """{ "phonemic": true, "respelling": [ { "name": "Plain", "rules": [["ʊ", "u"], ["⁵¹", "4"]] } ] }""");
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            TReflexFixture.TReflexPack.Replace("\"Mandarin\"", "\"" + borrower.TLanguageFixtureName + "\""));
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "弄", pack.TLanguageFixtureName);

        Assert.Equal("nʊŋ⁵¹", found[3].LReflexDraftText);
        Assert.Equal("nuŋ4", found[3].LReflexDraftRespelling);
        Assert.Equal("luŋ4", found[4].LReflexDraftRespelling);
        Assert.Equal("nòng", found[3].LReflexDraftRomanization);
        Assert.Equal("ろう", found[2].LReflexDraftText);
        Assert.Equal(string.Empty, found[2].LReflexDraftRespelling);
    }

    [Fact]
    public async Task ReflexStart_TwoCharacters_MergesRowsOfOneKind()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "弄璋", pack.TLanguageFixtureName);

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
    public async Task ReflexStart_Recast_MovesToneAndRaisesDigits()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexWuPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexWuPages));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "屋", pack.TLanguageFixtureName);

        Assert.Equal(
            [
                ("Wu", "", "oʔ⁵", "oq⁷", true),
                ("Wu", "", "ʊʔ⁵", "ok⁷", false),
            ],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal("literary", found[1].LReflexDraftNote);
    }
}
