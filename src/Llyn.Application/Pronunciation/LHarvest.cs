using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LHarvest
{
    private readonly IReadOnlyList<LSource> _lHarvestSources;

    public LHarvest(IReadOnlyList<LSource> sources)
    {
        _lHarvestSources = sources ?? throw new ArgumentNullException(nameof(sources));
    }

    public async Task LHarvestStart(string word, LListener listener, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        List<Task> pending = new(_lHarvestSources.Count);
        for (int order = 0; order < _lHarvestSources.Count; order++)
        {
            pending.Add(LHarvestSourceRun(_lHarvestSources[order], order, word, listener, cancellation));
        }

        try
        {
            await Task.WhenAll(pending).ConfigureAwait(false);
        }
        finally
        {
            if (!cancellation.IsCancellationRequested)
            {
                listener.LListenerFinish();
            }
        }
    }

    private static async Task LHarvestSourceRun(
        LSource source,
        int order,
        string word,
        LListener listener,
        CancellationToken cancellation)
    {
        listener.LListenerSourceStart(source.LSourceName);

        string? address;
        try
        {
            address = await source.LSourceFind(word, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return;
        }

        if (!string.IsNullOrEmpty(address) && !cancellation.IsCancellationRequested)
        {
            listener.LListenerRecordingAdd(new LRecording(source.LSourceName, address, order));
        }
    }
}
