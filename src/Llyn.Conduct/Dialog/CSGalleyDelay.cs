using System;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Conduct;

internal sealed class CSGalleyDelay(int time) : IDisposable
{
    private CancellationTokenSource? _csGalleyDelaySource;

    internal int CSGalleyDelayTime { get; set; } = time;

    public void Dispose()
    {
        _csGalleyDelaySource?.Cancel();
        _csGalleyDelaySource?.Dispose();
        _csGalleyDelaySource = null;
    }

    internal async Task<CancellationToken?> LSGalleyDelayStart()
    {
        Dispose();
        CancellationTokenSource source = new();
        _csGalleyDelaySource = source;
        try
        {
            await Task.Delay(CSGalleyDelayTime, source.Token);
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        return source.Token;
    }
}
