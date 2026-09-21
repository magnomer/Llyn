using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LShengfuSourceHttp : LShengfuSource
{
    private readonly HttpClient _lShengfuSourceClient;

    private const string LShengfuSourceToken = "{word}";

    private const RegexOptions LShengfuSourceLoose =
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    private static readonly Regex LShengfuSourceTag = new("<[^>]+>", RegexOptions.CultureInvariant);

    private static readonly TimeSpan LShengfuSourcePatience = TimeSpan.FromSeconds(2);

    public LShengfuSourceHttp(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _lShengfuSourceClient = client;
    }

    public async Task<(LShengfu? LShengfuFound, bool LShengfuReached)> LShengfuSourceFind(
        LShengfuRule rule, string character, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        string? body = await LShengfuBodyRead(_lShengfuSourceClient, rule, character, cancellation)
            .ConfigureAwait(false);
        if (body is null)
        {
            return (null, false);
        }

        try
        {
            if (!string.IsNullOrEmpty(rule.LShengfuRuleBusy)
                && Regex.IsMatch(body, rule.LShengfuRuleBusy, RegexOptions.CultureInvariant, LShengfuSourcePatience))
            {
                return (null, false);
            }

            return (LShengfuSourceScan(rule, character, body), true);
        }
        catch (RegexMatchTimeoutException)
        {
            return (null, true);
        }
    }

    public static LShengfu? LShengfuSourceScan(LShengfuRule rule, string character, string body)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(character);
        ArgumentNullException.ThrowIfNull(body);

        string pattern = rule.LShengfuRulePattern.Replace(
            LShengfuSourceToken, Regex.Escape(character), StringComparison.Ordinal);
        Regex line = new(pattern, LShengfuSourceLoose, LShengfuSourcePatience);

        List<string> found = [];
        foreach (Match match in line.Matches(body))
        {
            Group group = match.Groups[LShengfuRule.LShengfuRuleGroup];
            string text = LShengfuTextNormalize(group.Success ? group.Value : match.Value);
            if (text.Length > 0 && !found.Contains(text))
            {
                found.Add(text);
            }
        }

        return found.Count == 0
            ? null
            : new LShengfu(
                character, string.Join(rule.LShengfuRuleSeparator, found), rule.LShengfuRuleSource);
    }

    private static string LShengfuTextNormalize(string raw)
    {
        string plain = WebUtility.HtmlDecode(LShengfuSourceTag.Replace(raw, string.Empty));
        return string.Join(' ', plain.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static async Task<string?> LShengfuBodyRead(
        HttpClient client, LShengfuRule rule, string character, CancellationToken cancellation)
    {
        List<KeyValuePair<string, string>> fields = [];
        foreach (KeyValuePair<string, string> field in rule.LShengfuRuleForm)
        {
            fields.Add(new KeyValuePair<string, string>(
                field.Key, field.Value.Replace(LShengfuSourceToken, character, StringComparison.Ordinal)));
        }

        try
        {
            using HttpRequestMessage request = fields.Count == 0
                ? new HttpRequestMessage(
                    HttpMethod.Get,
                    rule.LShengfuRuleUrl.Replace(
                        LShengfuSourceToken, Uri.EscapeDataString(character), StringComparison.Ordinal))
                : new HttpRequestMessage(HttpMethod.Post, rule.LShengfuRuleUrl)
                {
                    Content = new FormUrlEncodedContent(fields),
                };
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
