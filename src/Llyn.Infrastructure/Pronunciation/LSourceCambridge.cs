using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Cambridge Dictionary pronunciation source. Cambridge exposes no public API, so this parses the
/// IPA from the rendered dictionary page. It reads resiliently: the UK and US dictionary hosts are
/// tried with transient-retry (see <see cref="LSourceReader"/>), and the IPA is captured tolerantly
/// across all <c>ipa</c> spans. A browser-like User-Agent on the shared <see cref="HttpClient"/> is
/// required, otherwise Cambridge responds with 403.
/// </summary>
public sealed class LSourceCambridge : LSource
{
    private static readonly Regex LSourceCambridgeIpa = new(
        "<span[^>]*class=\"[^\"]*\\bipa\\b[^\"]*\"[^>]*>",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LSourceCambridgeHeadword = new(
        "<[^>]*class=\"[^\"]*\\bhw\\b[^\"]*\\bdhw\\b[^\"]*\"[^>]*>(.*?)</",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private readonly HttpClient _lSourceCambridgeClient;

    public LSourceCambridge(HttpClient client)
    {
        _lSourceCambridgeClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    public LOrigin LSourceKind => LOrigin.LOriginCambridge;

    public async Task<LCandidate?> LSourceFind(string word, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return null;
        }

        string normalized = word.Trim().ToLowerInvariant();
        string escaped = Uri.EscapeDataString(normalized);
        string[] urls =
        [
            "https://dictionary.cambridge.org/dictionary/english/" + escaped,
            "https://dictionary.cambridge.org/us/dictionary/english/" + escaped
        ];

        string? html = await LSourceReader.LSourceReaderRead(_lSourceCambridgeClient, urls, cancellation)
            .ConfigureAwait(false);
        if (html is null || !LSourceEntryConfirm(html, normalized))
        {
            return null;
        }

        string? phonetic = LPronunciationText.LPronunciationTextRead(html, LSourceCambridgeIpa);
        return string.IsNullOrEmpty(phonetic) ? null : new LCandidate(LSourceKind, phonetic);
    }

    /// <summary>
    /// Confirms the page is a genuine dictionary entry for <paramref name="word"/>. Cambridge serves
    /// a 200 page for unknown words too, and that page carries unrelated IPA (a "Word of the day"
    /// widget), so a headword match guards against returning a stranger's pronunciation.
    /// </summary>
    private static bool LSourceEntryConfirm(string html, string word)
    {
        foreach (Match headword in LSourceCambridgeHeadword.Matches(html))
        {
            string text = LPronunciationText.LPronunciationTextNormalize(headword.Groups[1].Value).ToLowerInvariant();
            if (text == word)
            {
                return true;
            }
        }

        return false;
    }
}
