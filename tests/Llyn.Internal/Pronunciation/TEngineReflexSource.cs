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
    public async Task ReflexStart_SouthernMinPage_PartsBothGlossShapes()
    {
        const string match =
            """<li><small>\\((?:(?!</small>).)*?>Xiamen<(?:(?!</small>).)*?\\)"""
            + """(?:(?!</small>).)*?</small>\\s*<ul>(?:(?!</ul>).)*?<a[^>]*>Pe.{1,3}h-.{1,3}e-j.{1,3}</a>"""
            + """(?:(?!</li>).)*?<span class=\"zhpron-monospace\">(?<romanization>(?:(?!</li>).)*?)</li>"""
            + """(?:(?!</ul>).)*?Sinological <a[^>]*>IPA</a> \\((?:(?!</small>).)*?>Xiamen"""
            + """(?:(?!</small>).)*?\\)</small>: <span class=\"IPA\">(?<text>(?:(?!</li>).)*?)</li>""";
        const string gloss =
            """<li>{romanization} - (?<note>(?:(?!</li>|\\s*[(\"“”]).)*?)\\s*"""
            + """(?:\\(?\\s*[“\"](?<meaning>(?:(?!</li>).)*?)[”\"]\\s*\\)?)?[;.]?</li>""";
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            $$"""
            { "reflex": [ { "language": "Southern Min", "region": "Xiamen",
              "url": "https://example.test/wiki/{word}",
              "match": "{{match}}",
              "split": "\\s*[,]\\s*",
              "gloss": "{{gloss}}",
              "until": "<li><a[^>]*>[^<]+</a>\\s*<ul>",
              "every": true, "first": true, "folded": true, "superscript": true } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/wiki/%E6%83%A1"] =
                    "<li><small>(<a>Xiamen</a>)</small><ul>"
                    + "<li><a>Pe̍h-ōe-jī</a>: <span class=\"zhpron-monospace\">ok4, oh4</span></li>"
                    + "<li><small>Sinological <a>IPA</a> (<a>Xiamen</a>)</small>: "
                    + "<span class=\"IPA\">/ɔk³²/, /ɔʔ³²/</span></li></ul>"
                    + "<ul><li>ok4 - literary;</li><li>oh4 - vernacular (“hostile”).</li></ul>",
            }));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "惡", pack.TLanguageFixtureName);

        Assert.Equal(
            [("Southern Min", "", "ɔk³²", "ok⁴", true),
             ("Southern Min", "", "ɔʔ³²", "oh⁴", false)],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal(["literary", "vernacular"], found.Select(row => row.LReflexDraftNote));
        Assert.Equal(["", "hostile"], found.Select(row => row.LReflexDraftMeaning));
    }

    [Fact]
    public async Task ReflexStart_DialectRule_SplitsPairsGlossesAndRegions()
    {
        const string gloss =
            """<li>{romanization} - (?<note>(?:(?!</li>|\\s*[(\"“”]).)*?)\\s*"""
            + """(?:\\(?\\s*[“\"](?<meaning>(?:(?!</li>).)*?)[”\"]\\s*\\)?)?[;.]?</li>""";
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            $$"""
            { "reflex": [ { "language": "Jin", "region": "Taiyuan", "url": "https://example.test/wiki/{word}",
              "match": "Wiktionary: <span>(?<romanization>[^<]+)</span>.*?IPA: <span>(?<text>[^<]+)</span>",
              "split": "\\s*[,/]\\s*", "gloss": "{{gloss}}",
              "until": "<h3>", "every": true, "first": true, "folded": true } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
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

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "惡", pack.TLanguageFixtureName);

        Assert.Equal(
            [("Jin", "", "ɣaʔ²", "ghah4", true), ("Jin", "", "ɣəʔ²", "gheh4", false)],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal(["Taiyuan", "Taiyuan"], found.Select(row => row.LReflexDraftRegion));
        Assert.Equal(["literary", "vernacular"], found.Select(row => row.LReflexDraftNote));
        Assert.Equal(["", "difficult"], found.Select(row => row.LReflexDraftMeaning));
        Assert.True(Assert.Single(engine.TEngineReflexRead(pack.TLanguageFixtureName)).LReflexRuleFolded);
    }

    [Fact]
    public async Task ReflexStart_SameReadingTwice_KeepsOneRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Wu", "url": "https://example.test/wiki/{word}",
              "match": "Wugniu: <span>(?<romanization>[^<]+)</span>.*?IPA: <span>(?<text>[^<]+)</span>",
              "every": true } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/wiki/%E6%83%A1"] =
                    "<p>Wugniu: <span>5u</span></p><p>IPA: <span>/u³⁴/</span></p>"
                    + "<p>Wugniu: <span>5u</span></p><p>IPA: <span>/u³⁴/</span></p>",
            }));

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "惡", pack.TLanguageFixtureName);

        Assert.Equal([("Wu", "", "u³⁴", "5u", false)], found.Select(TReflexFixture.TReflexRowRead));
    }

    [Fact]
    public async Task ReflexStart_NoteGroup_LabelsHistoricalAndFolds()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """{ "reflex": [ { "language": "Japanese", "url": "https://example.test/wiki/{word}", "every": true,"""
            + """ "match": "<i>(?<=<b>(?<kind>Go-on|Kan-on)</b>: <span class=\"on-yomi\">(?:(?!</li>).){0,2000}?"""
            + """(?<main><span class=\"jouyou\">)?<i>)<a>(?<text>[^<]+)</a>"""
            + """(?=(?:(?!<sup>|</sup>|</li>).)*?(?:<a>)?(?<note>historical|ancient)<|)" } ] }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
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

        IReadOnlyList<LReflexDraft> found =

            await TReflexFixture.TReflexFetchRead(engine, "林", pack.TLanguageFixtureName);

        Assert.Equal(
            [("Japanese", "Go-on", "きょう", "", true),
             ("Japanese", "Go-on", "きやう", "", false),
             ("Japanese", "Kan-on", "りん", "", true),
             ("Japanese", "Kan-on", "りむ", "", false)],
            found.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal(["", "historical", "", "ancient"], found.Select(row => row.LReflexDraftNote));
    }

    [Fact]
    public async Task ReflexStart_MainGroupInsideNote_MarksNewStyleOverFirst()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Xiang", "region": "Changsha", "url": "https://example.test/wiki/{word}",
              "match": "(?<=<p>Note: <span>(?<romanization>[^<]+)</span></p>(?:(?!</ul>).)*?)IPA
            """.TrimEnd()
            + """(?: \\(<i>(?<note>(?<main>new-style)|[^<]+)</i>\\))?: <span>(?<text>[^<]+)</span>","""
            + """
              "every": true, "first": true } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/wiki/%E6%95%B4"] =
                    "<ul><p>Note: <span>zhen3</span></p><li>IPA (<i>old-style</i>): <span>/ʈ͡ʂən⁴¹/</span></li>"
                    + "<li>IPA (<i>new-style</i>): <span>/t͡sən⁴¹/</span></li></ul>",
                ["https://example.test/wiki/%E6%9E%97"] =
                    "<ul><p>Note: <span>lin2</span></p><li>IPA: <span>/lin¹³/</span></li></ul>",
            }));

        IReadOnlyList<LReflexDraft> styled =

            await TReflexFixture.TReflexFetchRead(engine, "整", pack.TLanguageFixtureName);
        IReadOnlyList<LReflexDraft> plain =
            await TReflexFixture.TReflexFetchRead(engine, "林", pack.TLanguageFixtureName);

        Assert.Equal(
            [("Xiang", "", "ʈ͡ʂən⁴¹", "zhen3", false), ("Xiang", "", "t͡sən⁴¹", "zhen3", true)],
            styled.Select(TReflexFixture.TReflexRowRead));
        Assert.Equal(["old-style", "new-style"], styled.Select(row => row.LReflexDraftNote));
        Assert.Equal([("Xiang", "", "lin¹³", "lin2", true)], plain.Select(TReflexFixture.TReflexRowRead));
    }
}
