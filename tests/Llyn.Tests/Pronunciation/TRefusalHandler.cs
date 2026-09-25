using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Llyn.Tests;

internal sealed class TRefusalHandler : HttpMessageHandler
{
    private readonly int _tRefusalHandlerRefusals;
    private int _tRefusalHandlerCount;

    internal TRefusalHandler(int refusals)
    {
        _tRefusalHandlerRefusals = refusals;
    }

    internal int TRefusalHandlerCount => Volatile.Read(ref _tRefusalHandlerCount);

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int count = Interlocked.Increment(ref _tRefusalHandlerCount);
        if (count <= _tRefusalHandlerRefusals)
        {
            HttpResponseMessage refused = new(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Too many requests"),
            };
            refused.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
            return Task.FromResult(refused);
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("audio"),
        });
    }
}
