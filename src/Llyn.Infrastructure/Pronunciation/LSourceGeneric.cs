using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// The language-agnostic source runner. Given a language pack's <see cref="LSourceSpec"/>, it
/// executes the source purely from that data: it tries each attempt in order, fetches the attempt's
/// URLs resiliently (see <see cref="LSourceReader"/>), and extracts the value with the attempt's
/// declared strategy — a regex capture, a JSON path, or the tolerant nested-span IPA reader. It
/// holds no knowledge of any particular source, dictionary, or language, so a new ordinary source
/// needs only a <c>source.json</c> entry and no code.
/// </summary>
public sealed class LSourceGeneric : LSource
{
    private const string LSourceGenericRegex = "regex";
    private const string LSourceGenericJson = "json";
    private const string LSourceGenericSpan = "span";
    private const string LSourceGenericToken = "{word}";

    private readonly LSourceSpec _lSourceGenericSpec;
    private readonly HttpClient _lSourceGenericClient;

    public LSourceGeneric(LSourceSpec spec, HttpClient client)
    {
        _lSourceGenericSpec = spec ?? throw new ArgumentNullException(nameof(spec));
        _lSourceGenericClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    public string LSourceName => _lSourceGenericSpec.LSourceSpecName;

    public string LSourceKind => _lSourceGenericSpec.LSourceSpecKind;

    public async Task<string?> LSourceFind(string word, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return null;
        }

        string trimmed = word.Trim();
        foreach (LSourceAttempt attempt in _lSourceGenericSpec.LSourceSpecAttempts)
        {
            string? value = await LSourceAttemptRun(attempt, trimmed, cancellation).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
        }

        return null;
    }

    private async Task<string?> LSourceAttemptRun(
        LSourceAttempt attempt,
        string word,
        CancellationToken cancellation)
    {
        string escaped = Uri.EscapeDataString(word);
        string[] urls = new string[attempt.LSourceAttemptUrls.Count];
        for (int index = 0; index < urls.Length; index++)
        {
            urls[index] = attempt.LSourceAttemptUrls[index].Replace(LSourceGenericToken, escaped, StringComparison.Ordinal);
        }

        string? body = await LSourceReader
            .LSourceReaderRead(_lSourceGenericClient, urls, attempt.LSourceAttemptHeaders, cancellation)
            .ConfigureAwait(false);
        if (body is null || !LSourceConfirm(attempt, body, word))
        {
            return null;
        }

        string? value = attempt.LSourceAttemptStrategy switch
        {
            LSourceGenericSpan => LSourceSpanRead(attempt, body),
            LSourceGenericJson => LSourceJsonRead(attempt, body),
            LSourceGenericRegex => LSourceRegexRead(attempt, body),
            _ => null
        };

        return LSourceAddressResolve(attempt, value);
    }

    private static string? LSourceAddressResolve(LSourceAttempt attempt, string? value)
    {
        if (string.IsNullOrEmpty(value) ||
            string.IsNullOrEmpty(attempt.LSourceAttemptPrefix) ||
            value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        return attempt.LSourceAttemptPrefix + value;
    }

    private static bool LSourceConfirm(LSourceAttempt attempt, string body, string word)
    {
        if (string.IsNullOrEmpty(attempt.LSourceAttemptGuard))
        {
            return true;
        }

        string pattern = attempt.LSourceAttemptGuard.Replace(LSourceGenericToken, Regex.Escape(word), StringComparison.Ordinal);
        return Regex.IsMatch(body, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static string? LSourceSpanRead(LSourceAttempt attempt, string body)
    {
        if (string.IsNullOrEmpty(attempt.LSourceAttemptPattern))
        {
            return null;
        }

        Regex open = new(attempt.LSourceAttemptPattern, RegexOptions.CultureInvariant);
        return LPronunciationText.LPronunciationTextRead(body, open);
    }

    private static string? LSourceRegexRead(LSourceAttempt attempt, string body)
    {
        string? captured = LSourceGroupRead(attempt, body);
        return LSourceNormalize(attempt, captured);
    }

    private static string? LSourceJsonRead(LSourceAttempt attempt, string body)
    {
        if (string.IsNullOrEmpty(attempt.LSourceAttemptPath))
        {
            return null;
        }

        string? text;
        try
        {
            using JsonDocument document = JsonDocument.Parse(body);
            JsonElement current = document.RootElement;
            foreach (string segment in attempt.LSourceAttemptPath.Split('.'))
            {
                if (current.ValueKind != JsonValueKind.Object ||
                    !current.TryGetProperty(segment, out JsonElement next))
                {
                    return null;
                }

                current = next;
            }

            text = current.ValueKind == JsonValueKind.String ? current.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        // A json value may still need a regex to pick the phonetic out of surrounding wikitext.
        string? captured = string.IsNullOrEmpty(attempt.LSourceAttemptPattern)
            ? text
            : LSourceGroupRead(attempt, text);
        return LSourceNormalize(attempt, captured);
    }

    private static string? LSourceGroupRead(LSourceAttempt attempt, string text)
    {
        if (string.IsNullOrEmpty(attempt.LSourceAttemptPattern))
        {
            return null;
        }

        Match match = Regex.Match(text, attempt.LSourceAttemptPattern, RegexOptions.CultureInvariant);
        if (!match.Success)
        {
            return null;
        }

        int group = attempt.LSourceAttemptGroup;
        return group >= 0 && group < match.Groups.Count ? match.Groups[group].Value : match.Value;
    }

    private static string? LSourceNormalize(LSourceAttempt attempt, string? captured)
    {
        if (string.IsNullOrEmpty(captured))
        {
            return null;
        }

        return attempt.LSourceAttemptPhonetic
            ? LPronunciationText.LPronunciationTextNormalize(captured)
            : captured.Trim();
    }
}
