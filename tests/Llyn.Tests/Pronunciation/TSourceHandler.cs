using System.Net;
using System.Net.Http;

namespace Llyn.Tests;

internal sealed class TSourceHandler : HttpMessageHandler
{
    private readonly string _tSourceHandlerBody;
    private readonly HttpStatusCode _tSourceHandlerStatus;

    internal TSourceHandler(string body, HttpStatusCode status)
    {
        _tSourceHandlerBody = body;
        _tSourceHandlerStatus = status;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage(_tSourceHandlerStatus)
        {
            Content = new StringContent(_tSourceHandlerBody),
        });
    }
}
