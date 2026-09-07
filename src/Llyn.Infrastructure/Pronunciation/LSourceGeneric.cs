using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

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

    public async Task<LAnswer> LSourceFind(string word, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return LAnswer.LAnswerBlank;
        }

        string trimmed = word.Trim();
        bool reached = false;
        foreach (LSourceAttempt attempt in _lSourceGenericSpec.LSourceSpecAttempts)
        {
            LAnswer answer = await LSourceAttemptRun(attempt, trimmed, cancellation).ConfigureAwait(false);
            if (!answer.LAnswerEmpty)
            {
                return answer;
            }

            reached |= answer.LAnswerReached;
        }

        return reached ? LAnswer.LAnswerBlank : LAnswer.LAnswerLost;
    }

    private async Task<LAnswer> LSourceAttemptRun(
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

        LAnswer fetched = await LSourceReader
            .LSourceReaderRead(_lSourceGenericClient, urls, attempt.LSourceAttemptHeaders, cancellation)
            .ConfigureAwait(false);
        if (fetched.LAnswerValue is not string body)
        {
            return fetched;
        }

        if (!LSourceConfirm(attempt, body, word))
        {
            return LAnswer.LAnswerBlank;
        }

        string? value = attempt.LSourceAttemptStrategy switch
        {
            LSourceGenericSpan => LSourceSpanRead(attempt, body),
            LSourceGenericJson => LSourceJsonRead(attempt, body),
            LSourceGenericRegex => LSourceRegexRead(attempt, body),
            _ => null
        };

        string? address = LSourceAddressResolve(attempt, value);
        return string.IsNullOrEmpty(address) ? LAnswer.LAnswerBlank : LAnswer.LAnswerCreate(address);
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
