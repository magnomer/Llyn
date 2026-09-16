﻿using System;
using System.Collections.Generic;
using System.Net;
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
    private const string LSourceGenericLink = "link";
    private const string LSourceGenericToken = "{word}";
    private const string LSourceGenericHeadword = "{headword}";
    private const int LSourceGenericHops = 2;

    private static readonly char[] LSourceGenericSeparators = [',', '/'];

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

        string headword = word.Trim();
        string current = LSourceSpellingResolve(headword);
        bool reached = false;
        HashSet<string> filled = new(StringComparer.Ordinal);
        List<LReading> readings = [];
        foreach (LSourceAttempt attempt in _lSourceGenericSpec.LSourceSpecAttempts)
        {
            (LAnswer answer, current) = await LSourceAttemptResolve(attempt, current, headword, cancellation)
                .ConfigureAwait(false);
            reached |= answer.LAnswerReached;
            HashSet<string> fresh = new(StringComparer.Ordinal);
            foreach (LReading reading in answer.LAnswerReadings)
            {
                if (!filled.Contains(reading.LReadingVariety))
                {
                    readings.Add(reading);
                    fresh.Add(reading.LReadingVariety);
                }
            }

            filled.UnionWith(fresh);
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

    private string LSourceSpellingResolve(string word)
    {
        string spelled = word;
        foreach (LRespellingRule rule in _lSourceGenericSpec.LSourceSpecSpelling)
        {
            spelled = rule.LRespellingRuleResolve(spelled);
        }

        return spelled.Length == 0 ? word : spelled;
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
        string headword,
        CancellationToken cancellation)
    {
        HashSet<string> visited = new(StringComparer.Ordinal) { word };
        HashSet<string> seen = new(StringComparer.Ordinal);
        List<LReading> gathered = [];
        string current = word;
        bool reached = false;
        for (int hop = 0; ; hop++)
        {
            (LAnswer answer, string? next) = await LSourceAttemptRun(attempt, current, headword, cancellation)
                .ConfigureAwait(false);
            reached |= answer.LAnswerReached;
            foreach (LReading reading in answer.LAnswerReadings)
            {
                if (seen.Add(reading.LReadingVariety + '\n' + reading.LReadingPhonetic))
                {
                    gathered.Add(reading);
                }
            }

            if (next is null || hop >= LSourceGenericHops || !visited.Add(next))
            {
                break;
            }

            current = next;
        }

        if (gathered.Count > 0)
        {
            return (LAnswer.LAnswerCreate(gathered), current);
        }

        return (reached ? LAnswer.LAnswerBlank : LAnswer.LAnswerLost, current);
    }

    private async Task<(LAnswer, string?)> LSourceAttemptRun(
        LSourceAttempt attempt,
        string word,
        string headword,
        CancellationToken cancellation)
    {
        string escaped = Uri.EscapeDataString(word);
        string typed = Uri.EscapeDataString(headword);
        string[] urls = new string[attempt.LSourceAttemptUrls.Count];
        for (int index = 0; index < urls.Length; index++)
        {
            urls[index] = attempt.LSourceAttemptUrls[index]
                .Replace(LSourceGenericToken, escaped, StringComparison.Ordinal)
                .Replace(LSourceGenericHeadword, typed, StringComparison.Ordinal);
        }

        (LAnswer fetched, string? landed) = await LSourceReader
            .LSourceReaderRead(_lSourceGenericClient, urls, attempt.LSourceAttemptHeaders, cancellation)
            .ConfigureAwait(false);
        if (fetched.LAnswerValue is not string body || landed is null)
        {
            return (fetched, null);
        }

        if (attempt.LSourceAttemptDecoded)
        {
            body = WebUtility.HtmlDecode(body);
        }

        try
        {
            return LSourceBodyRead(attempt, body, word, landed);
        }
        catch (RegexMatchTimeoutException)
        {
            return (LAnswer.LAnswerBlank, null);
        }
    }

    private static (LAnswer, string?) LSourceBodyRead(
        LSourceAttempt attempt, string body, string word, string landed)
    {
        if (!LSourceConfirm(attempt, body, word))
        {
            return (LAnswer.LAnswerBlank, null);
        }

        List<LReading> readings = [];
        foreach (LSourceReading reading in attempt.LSourceAttemptReadings)
        {
            IReadOnlyList<string> values = reading.LSourceReadingStrategy == LSourceGenericLink
                ? [landed]
                : LSourceValueScan(LSourcePatternResolve(reading, word), body);
            foreach (string value in values)
            {
                string? address = LSourceAddressResolve(attempt, value);
                if (!string.IsNullOrEmpty(address))
                {
                    readings.Add(new LReading(reading.LSourceReadingVariety, address));
                }
            }
        }

        LAnswer answer = readings.Count > 0 ? LAnswer.LAnswerCreate(readings) : LAnswer.LAnswerBlank;
        string? next = attempt.LSourceAttemptFollow is null
            ? null
            : LSourceValueRead(LSourcePatternResolve(attempt.LSourceAttemptFollow, word), body)?.Trim();
        return (answer, string.IsNullOrEmpty(next) ? null : next);
    }

    private static LSourceReading LSourcePatternResolve(LSourceReading reading, string word)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPattern) ||
            !reading.LSourceReadingPattern.Contains(LSourceGenericToken, StringComparison.Ordinal))
        {
            return reading;
        }

        return reading with
        {
            LSourceReadingPattern = reading.LSourceReadingPattern.Replace(
                LSourceGenericToken, Regex.Escape(word), StringComparison.Ordinal),
        };
    }

    private static string? LSourceValueRead(LSourceReading reading, string body)
    {
        IReadOnlyList<string> values = LSourceValueScan(reading, body);
        return values.Count > 0 ? values[0] : null;
    }

    private static IReadOnlyList<string> LSourceValueScan(LSourceReading reading, string body)
    {
        IReadOnlyList<string> captured = reading.LSourceReadingStrategy switch
        {
            LSourceGenericSpan => LSourceSpanScan(reading, body),
            LSourceGenericJson => LSourceJsonScan(reading, body),
            LSourceGenericRegex => LSourceGroupScan(reading, body),
            _ => []
        };

        if (!reading.LSourceReadingEvery && captured.Count > 1)
        {
            captured = [captured[0]];
        }

        List<string> values = [];
        foreach (string text in captured)
        {
            foreach (string piece in LSourcePieceScan(reading, text))
            {
                string? value = LSourceNormalize(reading, piece);
                if (value is not null)
                {
                    values.Add(value);
                }
            }
        }

        return values;
    }

    private static IReadOnlyList<string> LSourcePieceScan(LSourceReading reading, string text)
    {
        return reading.LSourceReadingPhonetic
            ? WebUtility.HtmlDecode(text).Split(LSourceGenericSeparators, StringSplitOptions.RemoveEmptyEntries)
            : [text];
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

        string pattern = attempt.LSourceAttemptGuard
            .Replace(LSourceGenericToken, Regex.Escape(word), StringComparison.Ordinal);
        return Regex.IsMatch(
            body, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, LSourceGenericPatience);
    }

    private static IReadOnlyList<string> LSourceSpanScan(LSourceReading reading, string body)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPattern))
        {
            return [];
        }

        Regex open = new(reading.LSourceReadingPattern, RegexOptions.CultureInvariant, LSourceGenericPatience);
        return LPronunciationText.LPronunciationTextScan(body, open, reading.LSourceReadingSkip);
    }

    private static IReadOnlyList<string> LSourceJsonScan(LSourceReading reading, string body)
    {
        string? text = LSourcePathRead(reading, body);
        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        return string.IsNullOrEmpty(reading.LSourceReadingPattern) ? [text] : LSourceGroupScan(reading, text);
    }

    private static string? LSourcePathRead(LSourceReading reading, string body)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPath))
        {
            return null;
        }

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

            return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static IReadOnlyList<string> LSourceGroupScan(LSourceReading reading, string text)
    {
        if (string.IsNullOrEmpty(reading.LSourceReadingPattern))
        {
            return [];
        }

        MatchCollection matches = Regex.Matches(
            text, reading.LSourceReadingPattern, RegexOptions.CultureInvariant, LSourceGenericPatience);
        List<string> captured = [];
        int group = reading.LSourceReadingGroup;
        for (int index = Math.Max(0, reading.LSourceReadingSkip); index < matches.Count; index++)
        {
            Match match = matches[index];
            captured.Add(group >= 0 && group < match.Groups.Count ? match.Groups[group].Value : match.Value);
        }

        return captured;
    }

    private static string? LSourceNormalize(LSourceReading reading, string captured)
    {
        string value = reading.LSourceReadingPhonetic
            ? LPronunciationText.LPronunciationTextNormalize(captured)
            : captured.Trim();
        return value.Length == 0 ? null : value;
    }
}
