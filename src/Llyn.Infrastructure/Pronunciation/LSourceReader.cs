using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Infrastructure;

/// <summary>
/// Resilient page reader for HTML pronunciation sources. Tries each candidate URL in turn and, per
/// URL, retries transient failures (timeout, 408, 429, 5xx) with a short backoff. Non-transient
/// responses (403, 404) fall through to the next URL rather than being retried. Returns the first
/// successful body, or <c>null</c> when every URL is exhausted.
/// </summary>
internal static class LSourceReader
{
    private const int LSourceReaderRetry = 2;

    public static async Task<string?> LSourceReaderRead(
        HttpClient client,
        IReadOnlyList<string> urls,
        CancellationToken cancellation)
    {
        foreach (string url in urls)
        {
            string? body = await LSourceReaderLoad(client, url, cancellation).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(body))
            {
                return body;
            }
        }

        return null;
    }

    private static async Task<string?> LSourceReaderLoad(
        HttpClient client,
        string url,
        CancellationToken cancellation)
    {
        for (int attempt = 0; ; attempt++)
        {
            bool retryable;
            try
            {
                using HttpResponseMessage response =
                    await client.GetAsync(url, cancellation).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
                }

                retryable = LSourceReaderConfirm(response.StatusCode);
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                // A per-request timeout (HttpClient.Timeout), not a user cancellation: retry.
                retryable = true;
            }
            catch (HttpRequestException)
            {
                retryable = true;
            }

            if (!retryable || attempt >= LSourceReaderRetry)
            {
                return null;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250 * (attempt + 1)), cancellation).ConfigureAwait(false);
        }
    }

    private static bool LSourceReaderConfirm(HttpStatusCode status)
    {
        return status == HttpStatusCode.RequestTimeout   // 408
            || status == HttpStatusCode.TooManyRequests  // 429
            || (int)status >= 500;                        // 5xx
    }
}
