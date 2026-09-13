using System.Net;
using System.Net.Http;

namespace Llyn.Tests;

internal sealed class TSourceHandler : HttpMessageHandler
{
    private readonly string _tSourceHandlerBody;
    private readonly HttpStatusCode _tSourceHandlerStatus;
    private readonly IReadOnlyDictionary<string, string>? _tSourceHandlerPages;
    private readonly Task? _tSourceHandlerGate;
    private int _tSourceHandlerCount;

    internal TSourceHandler(string body, HttpStatusCode status, Task? gate = null)
    {
        _tSourceHandlerBody = body;
        _tSourceHandlerStatus = status;
        _tSourceHandlerGate = gate;
    }

    internal TSourceHandler(IReadOnlyDictionary<string, string> pages)
    {
        _tSourceHandlerBody = string.Empty;
        _tSourceHandlerStatus = HttpStatusCode.NotFound;
        _tSourceHandlerPages = pages;
    }

    internal int TSourceHandlerCount => Volatile.Read(ref _tSourceHandlerCount);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _tSourceHandlerCount);
        if (_tSourceHandlerGate is not null)
        {
            await _tSourceHandlerGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        string url = request.RequestUri?.AbsoluteUri ?? string.Empty;
        if (_tSourceHandlerPages is not null && _tSourceHandlerPages.TryGetValue(url, out string? page))
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(page),
            };
        }

        return new HttpResponseMessage(_tSourceHandlerStatus)
        {
            Content = new StringContent(_tSourceHandlerBody),
        };
    }
}
