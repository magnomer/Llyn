using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LScriptSourceHttp : LScriptSource
{
    private readonly HttpClient _lScriptSourceClient;

    private const string LScriptSourceToken = "{word}";

    private static readonly Regex LScriptSourceTag = new("<[^>]+>", RegexOptions.CultureInvariant);

    private static readonly Regex LScriptSourceBreak = new(
        "<br\\s*/?>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly TimeSpan LScriptSourcePatience = TimeSpan.FromSeconds(2);

    public LScriptSourceHttp(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _lScriptSourceClient = client;
    }

    public async Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LScriptSourceFind(
        LScriptStyle style,
        string character,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(style);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        string? body = await LScriptBodyRead(_lScriptSourceClient, style, character, cancellation)
            .ConfigureAwait(false);
        if (body is null)
        {
            return ([], false);
        }

        IReadOnlyList<(string LScriptAddress, string LScriptCaption, string LScriptEpoch)> hits;
        string gloss;
        try
        {
            hits = LScriptHitScan(style, body);
            gloss = LScriptGlossRead(style, body);
        }
        catch (RegexMatchTimeoutException)
        {
            return ([], true);
        }

        List<LScriptImage> images = [];
        foreach ((string address, string caption, string epoch) in hits)
        {
            byte[]? data = await LScriptDataRead(_lScriptSourceClient, address, cancellation).ConfigureAwait(false);
            if (data is not null)
            {
                images.Add(new LScriptImage(
                    character, style.LScriptStyleName, images.Count, caption, gloss, data, epoch));
            }
        }

        return (images, true);
    }

    private static async Task<string?> LScriptBodyRead(
        HttpClient client, LScriptStyle style, string character, CancellationToken cancellation)
    {
        List<KeyValuePair<string, string>> fields = [];
        foreach (KeyValuePair<string, string> field in style.LScriptStyleForm)
        {
            fields.Add(new KeyValuePair<string, string>(
                field.Key, field.Value.Replace(LScriptSourceToken, character, StringComparison.Ordinal)));
        }

        try
        {
            using HttpRequestMessage request = new(HttpMethod.Post, style.LScriptStyleUrl)
            {
                Content = new FormUrlEncodedContent(fields),
            };
            using HttpResponseMessage response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
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

    private static IReadOnlyList<(string LScriptAddress, string LScriptCaption, string LScriptEpoch)> LScriptHitScan(
        LScriptStyle style, string body)
    {
        List<(string, string, string)> hits = [];
        foreach (Match match in Regex.Matches(
            body, style.LScriptStylePattern, RegexOptions.CultureInvariant, LScriptSourcePatience))
        {
            string address = LScriptGroupRead(match, style.LScriptStyleImage);
            if (address.Length == 0)
            {
                continue;
            }

            (string epoch, string caption) = LEpoch.LEpochResolve(
                style.LScriptStyleEpoch, LScriptTextNormalize(LScriptGroupRead(match, style.LScriptStyleCaption)));
            hits.Add((LScriptAddressResolve(style, address), caption, epoch));
        }

        return hits;
    }

    private static string LScriptGlossRead(LScriptStyle style, string body)
    {
        if (string.IsNullOrEmpty(style.LScriptStyleGloss))
        {
            return string.Empty;
        }

        Match match = Regex.Match(body, style.LScriptStyleGloss, RegexOptions.CultureInvariant, LScriptSourcePatience);
        return match.Success ? LScriptTextNormalize(LScriptGroupRead(match, 1)) : string.Empty;
    }

    private static string LScriptGroupRead(Match match, int group)
    {
        return group > 0 && group < match.Groups.Count ? match.Groups[group].Value : string.Empty;
    }

    private static string LScriptAddressResolve(LScriptStyle style, string address)
    {
        string resolved = WebUtility.HtmlDecode(address);
        foreach (LRespellingRule rule in style.LScriptStyleRewrite)
        {
            resolved = rule.LRespellingRuleResolve(resolved);
        }

        if (string.IsNullOrEmpty(style.LScriptStylePrefix)
            || resolved.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return resolved;
        }

        return style.LScriptStylePrefix + resolved;
    }

    private static string LScriptTextNormalize(string raw)
    {
        string spaced = LScriptSourceBreak.Replace(raw, " ");
        string plain = WebUtility.HtmlDecode(LScriptSourceTag.Replace(spaced, string.Empty));
        return string.Join(' ', plain.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static async Task<byte[]?> LScriptDataRead(
        HttpClient client, string address, CancellationToken cancellation)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(address, cancellation).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            byte[] data = await response.Content.ReadAsByteArrayAsync(cancellation).ConfigureAwait(false);
            return data.Length == 0 ? null : data;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
            when (exception is HttpRequestException or OperationCanceledException or UriFormatException)
        {
            return null;
        }
    }
}
