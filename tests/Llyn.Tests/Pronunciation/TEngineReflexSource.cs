using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineReflexSource
{
    [Fact]
    public async Task ReflexFind_DialectRule_SplitsPairsRemarksAndRegions()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Jin", "region": "Taiyuan", "url": "https://example.test/wiki/{word}",
              "match": "Wiktionary: <span>(?<note>[^<]+)</span>.*?IPA: <span>(?<text>[^<]+)</span>",
              "split": "\\s*[,/]\\s*", "remark": "<li>{note} - (?<remark>(?:(?!</li>).)*?)[;.]?</li>",
              "until": "<h3>", "every": true, "first": true, "folded": true } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/wiki/%E6%83%A1"] =
                    """
                    <p>Wiktionary: <span>ghah4 / gheh4</span></p><p>IPA: <span>/ɣaʔ²/, /ɣəʔ²/</span></p>
                    <ul><li>ghah4 - literary;</li><li>gheh4 - vernacular (“difficult”).</li></ul>
                    <h3>Other</h3><ul><li>gheh4 - old-style.</li></ul>
                    """,
            }));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "惡", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [("Jin", "", "ɣaʔ²", "ghah4", true), ("Jin", "", "ɣəʔ²", "gheh4", false)],
            found.Select(TReflexRowRead));
        Assert.Equal(["Taiyuan", "Taiyuan"], found.Select(row => row.LReflexDraftRegion));
        Assert.Equal(["literary", "vernacular (“difficult”)"], found.Select(row => row.LReflexDraftRemark));
        Assert.True(Assert.Single(engine.TEngineReflexRead(pack.TLanguageFixtureName)).LReflexRuleFolded);
    }

    [Fact]
    public async Task ReflexFind_SameReadingTwice_KeepsOneRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Wu", "url": "https://example.test/wiki/{word}",
              "match": "Wugniu: <span>(?<note>[^<]+)</span>.*?IPA: <span>(?<text>[^<]+)</span>", "every": true } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/wiki/%E6%83%A1"] =
                    "<p>Wugniu: <span>5u</span></p><p>IPA: <span>/u³⁴/</span></p>"
                    + "<p>Wugniu: <span>5u</span></p><p>IPA: <span>/u³⁴/</span></p>",
            }));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "惡", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal([("Wu", "", "u³⁴", "5u", false)], found.Select(TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_RemarkGroup_LabelsHistoricalAndFoldsSameSpelling()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """{ "reflex": [ { "language": "Japanese", "url": "https://example.test/wiki/{word}", "every": true,"""
            + """ "match": "<i>(?<=<b>(?<kind>Go-on|Kan-on)</b>: <span class=\"on-yomi\">(?:(?!</li>).){0,2000}?"""
            + """(?<main><span class=\"jouyou\">)?<i>)<a>(?<text>[^<]+)</a>"""
            + """(?=(?:(?!<sup>|</sup>|</li>).)*?(?:<a>)?(?<remark>historical|ancient)<|)" } ] }""");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/wiki/%E6%9E%97"] =
                    "<li><b>Go-on</b>: <span class=\"on-yomi\"><span class=\"jouyou\"><i><a>きょう</a></i> (<a>Jōyō</a>)"
                    + "</span><sup>←<i><a>きやう</a></i> (<a>historical</a>)</sup></span></li>"
                    + "<li><b>Kan-on</b>: <span class=\"on-yomi\"><span class=\"jouyou\"><i><a>りん</a></i> (<a>Jōyō</a>)"
                    + "</span><sup>←<i><a>りん</a></i> (<a>historical</a>)</sup>"
                    + "<sup>←<i><a>りむ</a></i> (<span>ancient</span>)</sup></span></li>",
            }));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "林", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [("Japanese", "Go-on", "きょう", "", true),
             ("Japanese", "Go-on", "きやう", "", false),
             ("Japanese", "Kan-on", "りん", "", true),
             ("Japanese", "Kan-on", "りむ", "", false)],
            found.Select(TReflexRowRead));
        Assert.Equal(["", "historical", "", "ancient"], found.Select(row => row.LReflexDraftRemark));
    }

    private static (string, string, string, string, bool) TReflexRowRead(LReflexDraft row) =>
        (row.LReflexDraftLanguage,
         row.LReflexDraftKind,
         row.LReflexDraftText,
         row.LReflexDraftNote,
         row.LReflexDraftMain);
}
