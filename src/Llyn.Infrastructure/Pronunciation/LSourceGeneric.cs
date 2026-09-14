using System;
using System.Collections.Generic;
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
    private const int LSourceGenericHops = 2;

    private static readonly TimeSpan LSourceGenericPatience = TimeSpan.FromSeconds(2);

    private readonly LSourceSpec _lSourceGenericSpec;
    private readonly HttpClient _lSourceGenericClient;
    private readonly HashSet<string> _lSourceGenericVarieties;

    public LSourceGeneric(LSourceSpec spec, HttpClient client)
    {
        _lSourceGenericSpec = spec ?? throw new ArgumentNullException(nameof(spec));
        _lSourceGenericClient = client ?? throw new ArgumentNullException(nameof(client));
        _lSourceGenericVarieties = LSourceVarietyScan(spec);
    }

    public string LSourceName => _lSourceGenericSpec.LSourceSpecName;

    public async Task<LAnswer> LSourceFind(string word, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return LAnswer.LAnswerBlank;
        }

        string current = word.Trim();
        bool reached = false;
        Dictionary<string, LReading> filled = new(StringComparer.Ordinal);
        List<LReading> readings = [];
        foreach (LSourceAttempt attempt in _lSourceGenericSpec.LSourceSpecAttempts)
        {
            (LAnswer answer, current) = await LSourceAttemptResolve(attempt, current, cancellation).ConfigureAwait(false);
            reached |= answer.LAnswerReached;
            foreach (LReading reading in answer.LAnswerReadings)
            {
                if (filled.TryAdd(reading.LReadingVariety, reading))
                {
                    readings.Add(reading);
                }
            }

            if (filled.Count >= _lSourceGenericVarieties.Count)
            {
                break;
            }
        }

        if (readings.Count > 0)
        {
            return LAnswer.LAnswerCreate(readings);
        }

        return reached ? LAnswer.LAnswerBlank : LAnswer.LAnswerLost;
    }

    private static HashSet<string> LSourceVarietyScan(LSourceSpec spec)
    {
        HashSet<string> varieties = new(StringComparer.Ordinal);
        foreach (LSourceAttempt attempt in spec.LSourceSpecAttempts)
        {
            foreach (LSourceReading reading in attempt.LSourceAttemptReadings)
            {
                varieties.Add(reading.LSourceReadingVariety);
            }
        }

        return varieties;
    }

    private async Task<(LAnswer, string)> LSourceAttemptResolve(
        LSourceAttempt attempt,
        string word,
        CancellationToken cancellation)
    {
        HashSet<string> visited = new(StringComparer.Ordinal) { word };
        string current = word;
        for (int hop = 0; ; hop++)
        {
            (LAnswer answer, string? next) = await LSourceAttemptRun(attempt, current, cancellation).ConfigureAwait(false);
            if (!answer.LAnswerEmpty || next is null || hop >= LSourceGenericHops || !visited.Add(next))
            {
                return (answer, current);
            }

            current = next;
        }
    }

    private async Task<(LAnswer, string?)> LSourceAttemptRun(
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
            return (fetched, null);
        }

        try
        {
            return LSourceBodyRead(attempt, body, word);
        }
        catch (RegexMatchTimeoutException)
        {
            return (LAnswer.LAnswerBlank, null);
        }
    }

    private static (LAnswer, string?) LSourceBodyRead(LSourceAttempt attempt, string body, string word)
    {
        if (!LSourceConfirm(attempt, body, word))
        {
            return (LAnswer.LAnswerBlank, null);
        }

        List<LReading> readings = [];
        foreach (LSourceReading reading in attempt.LSourceAttemptReadings)
        {
            string? address = LSourceAddressResolve(attempt, LSourceValueRead(reading, body));
            if (!string.IsNullOrEmpty(address))
            {
                readings.Add(new LReading(reading.LSourceReadingVariety, address));
            }
        }

        if (readings.Count > 0)
        {
            return (LAnswer.LAnswerCreate(readings), null);
        }

        string? next = attempt.LSourceAttemptFollow is null
            ? null
            : LSourceValueRead(attempt.LSourceAttemptFollow, body)?.Trim();
        return (LAnswer.LAnswerBlank, string.IsNullOrEmpty(next) ? null : next);
    }

    private static string? LSourceValueRead(LSourceReading reading, string body)
    {
        return reading.LSourceReadingStrategy switch
        {
            LSourceGenericSpan => LSourceSpanRead(reading, body),
            LSourceGenericJson => LSourceJsonRead(reading, body),
            LSourceGenericRegex => LSourceRegexRead(reading, body),
            _ => null
        };
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
        return Regex.IsMatch(
            body, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, LSourceGenericPatience);
    }

    private static string? LSourceSpanRead(LSourceReading reading, string body)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPattern))
        {
            return null;
        }

        Regex open = new(reading.LSourceReadingPattern, RegexOptions.CultureInvariant, LSourceGenericPatience);
        int skip = reading.LSourceReadingSkip;
        if (skip <= 0)
        {
            return LPronunciationText.LPronunciationTextRead(body, open);
        }

        MatchCollection matches = open.Matches(body);
        if (skip >= matches.Count)
        {
            return null;
        }

        return LPronunciationText.LPronunciationTextRead(body[matches[skip].Index..], open);
    }

    private static string? LSourceRegexRead(LSourceReading reading, string body)
    {
        string? captured = LSourceGroupRead(reading, body);
        return LSourceNormalize(reading, captured);
    }

    private static string? LSourceJsonRead(LSourceReading reading, string body)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPath))
        {
            return null;
        }

        string? text;
        try
        {
            using JsonDocument document = JsonDocument.Parse(body);
            JsonElement current = document.RootElement;
            foreach (string segment in reading.LSourceReadingPath.Split('.'))
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

        string? captured = string.IsNullOrEmpty(reading.LSourceReadingPattern)
            ? text
            : LSourceGroupRead(reading, text);
        return LSourceNormalize(reading, captured);
    }

    private static string? LSourceGroupRead(LSourceReading reading, string text)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPattern))
        {
            return null;
        }

        MatchCollection matches = Regex.Matches(
            text, reading.LSourceReadingPattern, RegexOptions.CultureInvariant, LSourceGenericPatience);
        int skip = Math.Max(0, reading.LSourceReadingSkip);
        if (skip >= matches.Count)
        {
            return null;
        }

        Match match = matches[skip];
        int group = reading.LSourceReadingGroup;
        return group >= 0 && group < match.Groups.Count ? match.Groups[group].Value : match.Value;
    }

    private static string? LSourceNormalize(LSourceReading reading, string? captured)
    {
        if (string.IsNullOrEmpty(captured))
        {
            return null;
        }

        return reading.LSourceReadingPhonetic
            ? LPronunciationText.LPronunciationTextNormalize(captured)
            : captured.Trim();
    }
}
