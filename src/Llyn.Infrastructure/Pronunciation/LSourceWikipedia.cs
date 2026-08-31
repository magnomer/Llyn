using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Wikipedia/Wiktionary pronunciation source. Tries the Wiktionary action API first (structured,
/// but the server refuses requests fairly often) and falls back to a resilient parse of the
/// rendered Wiktionary page: several page endpoints are tried with transient-retry, and the IPA is
/// captured tolerantly so wrapping markup does not defeat it.
/// </summary>
public sealed class LSourceWikipedia : LSource
{
    private static readonly Regex LSourceWikipediaTemplate = new(
        @"\{\{IPA\|[^}]*?(/[^/|}]+/|\[[^\]|}]+\])",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LSourceWikipediaIpa = new(
        "<span[^>]*class=\"[^\"]*\\bIPA\\b[^\"]*\"[^>]*>",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly HttpClient _lSourceWikipediaClient;

    public LSourceWikipedia(HttpClient client)
    {
        _lSourceWikipediaClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    public LOrigin LSourceKind => LOrigin.LOriginWikipedia;

    public async Task<LCandidate?> LSourceFind(string word, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return null;
        }

        string? phonetic = await LSourceApiRead(word, cancellation).ConfigureAwait(false)
            ?? await LSourcePageRead(word, cancellation).ConfigureAwait(false);

        return string.IsNullOrEmpty(phonetic) ? null : new LCandidate(LSourceKind, phonetic);
    }

    private async Task<string?> LSourceApiRead(string word, CancellationToken cancellation)
    {
        string url =
            "https://en.wiktionary.org/w/api.php?action=parse&prop=wikitext&format=json&redirects=1&page="
            + Uri.EscapeDataString(word);

        try
        {
            using HttpResponseMessage response =
                await _lSourceWikipediaClient.GetAsync(url, cancellation).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
            using JsonDocument document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("parse", out JsonElement parse) ||
                !parse.TryGetProperty("wikitext", out JsonElement wikitext) ||
                !wikitext.TryGetProperty("*", out JsonElement content) ||
                content.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            Match match = LSourceWikipediaTemplate.Match(content.GetString()!);
            return match.Success ? LPronunciationText.LPronunciationTextNormalize(match.Groups[1].Value) : null;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private async Task<string?> LSourcePageRead(string word, CancellationToken cancellation)
    {
        string escaped = Uri.EscapeDataString(word);
        string[] urls =
        [
            // Clean rendered fragment from the stable REST endpoint, then the full pages (desktop
            // and mobile) as further backups. Any one succeeding ends the search.
            "https://en.wiktionary.org/api/rest_v1/page/html/" + escaped,
            "https://en.wiktionary.org/wiki/" + escaped,
            "https://en.m.wiktionary.org/wiki/" + escaped
        ];

        string? html = await LSourceReader.LSourceReaderRead(_lSourceWikipediaClient, urls, cancellation)
            .ConfigureAwait(false);

        return html is null ? null : LPronunciationText.LPronunciationTextRead(html, LSourceWikipediaIpa);
    }
}
