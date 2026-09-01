using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LHarvest
{
    private const string LHarvestKind = "audio";

    private readonly IReadOnlyList<LSource> _lHarvestSources;

    public LHarvest(IReadOnlyList<LSource> sources)
    {
        _lHarvestSources = sources ?? throw new ArgumentNullException(nameof(sources));
    }

    public async Task LHarvestStart(string word, LListener listener, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        List<Task> pending = new(_lHarvestSources.Count);
        foreach (LSource source in _lHarvestSources)
        {
            if (string.Equals(source.LSourceKind, LHarvestKind, StringComparison.Ordinal))
            {
                pending.Add(LHarvestSourceRun(source, word, listener, cancellation));
            }
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
            listener.LListenerRecordingAdd(new LRecording(source.LSourceName, address));
        }
    }
}
