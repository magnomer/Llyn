using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

internal static class TReflexFixture
{
    internal const string TReflexPack =
        """
        { "reflex": [
            { "language": "Korean", "url": "https://example.test/wiki/{word}",
              "match": "eumhun: (?<sense>[^ ]+) (?<sound>[^ <]+)(?: initial (?<initial>[^ <]+))?",
              "format": "{sound}[[({initial})]] | {sense}", "epithet": "{text}", "clip": "\\(.*\\)",
              "first": true },
            { "language": "Japanese", "url": "https://example.test/wiki/{word}",
              "match": "<b>(?<kind>Go-on|Kan-on)</b>: (?<main><i class=\"jouyou\">)?<span>(?<text>[^<]+)</span>",
              "every": true, "first": true },
            { "language": "Mandarin", "url": "https://example.test/ipa/{word}",
              "match": "IPA: (?<text>/[^ <]+/(?:, /[^ <]+/)*)(?: (?<romanization>[^<]+))?", "busy": "Too fast",
              "split": "\\s*,\\s*", "region": "Beijing" } ] }
        """;

    internal const string TReflexNong =
        """
        <p>eumhun: 희롱할 롱 initial 농</p>
        <ul><li><b>Go-on</b>: <span>る</span></li>
        <li><b>Kan-on</b>: <i class="jouyou"><span>ろう</span></i></li></ul>
        """;

    internal const string TReflexZhang =
        """
        <p>eumhun: 홀 장</p>
        <ul><li><b>Kan-on</b>: <span>しょう</span></li></ul>
        """;

    internal const string TReflexAn =
        """
        <p>eumhun: 편안 안</p>
        <ul><li><b>Go-on</b>: <span>あん</span></li>
        <li><b>Kan-on</b>: <i class="jouyou"><span>あん</span></i></li></ul>
        """;

    internal static readonly TimeSpan TReflexPatience = TimeSpan.FromSeconds(5);

    internal static readonly Dictionary<string, string> TReflexPages = new()
    {
        ["https://example.test/wiki/%E5%BC%84"] = TReflexNong,
        ["https://example.test/wiki/%E7%92%8B"] = TReflexZhang,
        ["https://example.test/wiki/%E5%AE%89"] = TReflexAn,
        ["https://example.test/ipa/%E5%AE%89"] = "<p>IPA: /än⁵⁵/ ān</p>",
        ["https://example.test/ipa/%E5%BC%84"] = "<p>IPA: /nʊŋ⁵¹/, /lʊŋ⁵¹/ nòng</p>",
        ["https://example.test/ipa/%E7%92%8B"] = "<p>IPA: /ʈ͡ʂɑŋ⁵⁵/ zhāng</p>",
    };

    internal const string TReflexWuPack =
        """
        { "reflex": [
            { "language": "Wu", "url": "https://example.test/wu/{word}",
              "match": "Wugniu: (?<romanization>[^<]+)</span>.*?IPA: (?<text>[^<]+)</span>",
              "split": "\\s*[,/]\\s*", "recast": [["^(\\d)(.+)$", "$2$1"]], "superscript": true,
              "gloss": "<li>{romanization} - (?<note>[^<]*?)[;.]?</li>", "every": true, "first": true } ] }
        """;

    internal static readonly Dictionary<string, string> TReflexWuPages = new()
    {
        ["https://example.test/wu/%E5%B1%8B"] =
            "<p>Wugniu: 7oq / 7ok</span> IPA: /oʔ⁵/, /ʊʔ⁵/</span></p><ul><li>7ok - literary;</li></ul>",
    };

    internal static async Task TReflexSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TReflexPatience;
        while (engine.TEngineReflexCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the reflex fetch to settle.");
            await Task.Delay(20);
        }
    }

    internal static (string, string, string, string, bool) TReflexRowRead(LReflexDraft row) =>
        (row.LReflexDraftLanguage,
         row.LReflexDraftKind,
         row.LReflexDraftText,
         row.LReflexDraftRomanization,
         row.LReflexDraftMain);

    internal static (string, string, string, string, bool) TReflexRowRead(LReflex row) =>
        (row.LReflexLanguage,
         row.LReflexKind,
         row.LReflexText,
         row.LReflexRomanization,
         row.LReflexMain);

    internal static LEntryDraft TReflexDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []);
    }
}
