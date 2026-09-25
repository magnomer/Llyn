using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSourceReader
{
    private const int LSourceReaderRetry = 2;

    public static async Task<(LAnswer, string?)> LSourceReaderRead(
        HttpClient client,
        IReadOnlyList<string> urls,
        IReadOnlyDictionary<string, string>? headers,
        CancellationToken cancellation)
    {
        bool reached = false;
        foreach (string url in urls)
        {
            LAnswer answer = await LSourceReaderLoad(client, url, headers, cancellation).ConfigureAwait(false);
            if (!answer.LAnswerEmpty)
            {
                return (answer, url);
            }

            reached |= answer.LAnswerReached;
        }

        return (reached ? LAnswer.LAnswerBlank : LAnswer.LAnswerLost, null);
    }

    private static async Task<LAnswer> LSourceReaderLoad(
        HttpClient client,
        string url,
        IReadOnlyDictionary<string, string>? headers,
        CancellationToken cancellation)
    {
        for (int attempt = 0; ; attempt++)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, url);
                if (headers is not null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                using HttpResponseMessage response =
                    await client.SendAsync(request, cancellation).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
                    return string.IsNullOrEmpty(body) ? LAnswer.LAnswerBlank : LAnswer.LAnswerCreate(body);
                }

                if (!LSourceReaderConfirm(response.StatusCode))
                {
                    return LAnswer.LAnswerBlank;
                }
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
            }
            catch (HttpRequestException)
            {
            }

            if (attempt >= LSourceReaderRetry)
            {
                return LAnswer.LAnswerLost;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250 * (attempt + 1)), cancellation).ConfigureAwait(false);
        }
    }

    private static bool LSourceReaderConfirm(HttpStatusCode status)
    {
        return status == HttpStatusCode.RequestTimeout
            || status == HttpStatusCode.TooManyRequests
            || (int)status >= 500;
    }
}
