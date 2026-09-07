using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLookup : LSeeker
{
    private readonly IReadOnlyList<LSource> _lLookupSources;

    public LLookup(IReadOnlyList<LSource> sources)
    {
        _lLookupSources = sources ?? throw new ArgumentNullException(nameof(sources));
    }

    public async Task LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        List<Task> pending = new(_lLookupSources.Count);
        for (int order = 0; order < _lLookupSources.Count; order++)
        {
            pending.Add(LLookupSourceRun(_lLookupSources[order], order, word, receiver, cancellation));
        }

        try
        {
            await Task.WhenAll(pending).ConfigureAwait(false);
        }
        finally
        {
            if (!cancellation.IsCancellationRequested)
            {
                receiver.LReceiverLookupFinish();
            }
        }
    }

    private static async Task LLookupSourceRun(
        LSource source,
        int order,
        string word,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        receiver.LReceiverSourceStart(source.LSourceName);

        string? phonetic;
        try
        {
            phonetic = await source.LSourceFind(word, cancellation).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return;
        }

        if (!string.IsNullOrEmpty(phonetic) && !cancellation.IsCancellationRequested)
        {
            receiver.LReceiverCandidateAdd(new LCandidate(source.LSourceName, phonetic, order));
        }
    }
}
