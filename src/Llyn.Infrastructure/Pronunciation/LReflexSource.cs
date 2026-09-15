using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LReflexSource
{
    private const string LReflexSourceToken = "{word}";

    private const RegexOptions LReflexSourceLoose =
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    private static readonly Regex LReflexSourceTag = new("<[^>]+>", RegexOptions.CultureInvariant);

    private static readonly Regex LReflexSourceSlot =
        new("\\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\\}", RegexOptions.CultureInvariant);

    private static readonly Regex LReflexSourceOption =
        new("\\[\\[(?<body>.*?)\\]\\]", RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly TimeSpan LReflexSourcePatience = TimeSpan.FromSeconds(2);

    public static async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexSourceFind(
        HttpClient client,
        LReflexRule rule,
        string character,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        string? body = await LReflexBodyRead(client, rule, character, cancellation).ConfigureAwait(false);
        if (body is null)
        {
            return ([], false);
        }

        try
        {
            if (!string.IsNullOrEmpty(rule.LReflexRuleBusy)
                && Regex.IsMatch(body, rule.LReflexRuleBusy, RegexOptions.CultureInvariant, LReflexSourcePatience))
            {
                return ([], false);
            }

            return (LReflexSourceScan(rule, character, body), true);
        }
        catch (RegexMatchTimeoutException)
        {
            return ([], true);
        }
    }

    public static IReadOnlyList<LReflexDraft> LReflexSourceScan(LReflexRule rule, string character, string body)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(character);
        ArgumentNullException.ThrowIfNull(body);

        string pattern = rule.LReflexRulePattern.Replace(
            LReflexSourceToken, Regex.Escape(character), StringComparison.Ordinal);
        Regex line = new(pattern, LReflexSourceLoose, LReflexSourcePatience);

        List<LReflexDraft> rows = [];
        foreach (Match match in line.Matches(body))
        {
            string text = LReflexTextResolve(rule, LReflexTextFormat(rule, line, match));
            if (text.Length == 0)
            {
                continue;
            }

            rows.Add(new LReflexDraft(
                rule.LReflexRuleLanguage,
                LReflexGroupRead(match, LReflexRule.LReflexRuleKind),
                text,
                match.Groups[LReflexRule.LReflexRuleMain] is { Success: true, Length: > 0 },
                LReflexDraftNote: LReflexGroupRead(match, LReflexRule.LReflexRuleNote)));

            if (!rule.LReflexRuleEvery)
            {
                break;
            }
        }

        if (rows.Count > 1 && rows.TrueForAll(row => row.LReflexDraftText == rows[0].LReflexDraftText))
        {
            for (int index = 0; index < rows.Count; index++)
            {
                rows[index] = rows[index] with { LReflexDraftMain = true };
            }
        }
        else if (rule.LReflexRuleFirst && rows.Count > 0 && !rows.Exists(row => row.LReflexDraftMain))
        {
            rows[0] = rows[0] with { LReflexDraftMain = true };
        }

        return rows;
    }

    private static string LReflexTextResolve(LReflexRule rule, string text)
    {
        foreach (LRespellingRule rewrite in rule.LReflexRuleRewrite)
        {
            text = rewrite.LRespellingRuleResolve(text);
        }

        return text.Trim();
    }

    private static string LReflexTextFormat(LReflexRule rule, Regex line, Match match)
    {
        string text = LReflexSourceOption.Replace(rule.LReflexRuleTemplate, option =>
        {
            bool missing = false;
            string body = LReflexSlotFormat(option.Groups["body"].Value, line, match, ref missing);
            return missing ? string.Empty : body;
        });

        bool blank = false;
        text = LReflexSlotFormat(text, line, match, ref blank);
        return string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string LReflexSlotFormat(string template, Regex line, Match match, ref bool missing)
    {
        bool lost = false;
        string filled = LReflexSourceSlot.Replace(template, slot =>
        {
            string name = slot.Groups["name"].Value;
            if (line.GroupNumberFromName(name) < 0)
            {
                return slot.Value;
            }

            string value = LReflexGroupRead(match, name);
            lost |= value.Length == 0;
            return value;
        });

        missing |= lost;
        return filled;
    }

    private static string LReflexGroupRead(Match match, string name)
    {
        Group group = match.Groups[name];
        return group.Success ? LReflexTextNormalize(group.Value) : string.Empty;
    }

    private static string LReflexTextNormalize(string raw)
    {
        string plain = WebUtility.HtmlDecode(LReflexSourceTag.Replace(raw, string.Empty));
        return string.Join(' ', plain.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .Normalize(NormalizationForm.FormC);
    }

    private static async Task<string?> LReflexBodyRead(
        HttpClient client, LReflexRule rule, string character, CancellationToken cancellation)
    {
        List<KeyValuePair<string, string>> fields = [];
        foreach (KeyValuePair<string, string> field in rule.LReflexRuleForm)
        {
            fields.Add(new KeyValuePair<string, string>(
                field.Key, field.Value.Replace(LReflexSourceToken, character, StringComparison.Ordinal)));
        }

        try
        {
            using HttpRequestMessage request = fields.Count == 0
                ? new HttpRequestMessage(
                    HttpMethod.Get,
                    rule.LReflexRuleUrl.Replace(
                        LReflexSourceToken, Uri.EscapeDataString(character), StringComparison.Ordinal))
                : new HttpRequestMessage(HttpMethod.Post, rule.LReflexRuleUrl)
                {
                    Content = new FormUrlEncodedContent(fields),
                };
            foreach (KeyValuePair<string, string> header in rule.LReflexRuleHeaders)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            using HttpResponseMessage response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return string.Empty;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException)
        {
            return null;
        }
    }
}
