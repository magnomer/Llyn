using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LHarvest
{
    private static readonly TimeSpan LHarvestDeadline = TimeSpan.FromMinutes(1);

    private readonly IReadOnlyList<LSource> _lHarvestSources;

    public LHarvest(IReadOnlyList<LSource> sources)
    {
        _lHarvestSources = sources ?? throw new ArgumentNullException(nameof(sources));
    }

    public async Task<IReadOnlyList<LRecording>> LHarvestStart(
        string word,
        LListener listener,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        List<LRecording> found = [];
        List<Task> pending = new(_lHarvestSources.Count);
        for (int order = 0; order < _lHarvestSources.Count; order++)
        {
            pending.Add(LHarvestSourceRun(_lHarvestSources[order], order, word, listener, found, cancellation));
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

        found.Sort(static (one, other) => one.LRecordingOrder.CompareTo(other.LRecordingOrder));
        return found;
    }

    private static async Task LHarvestSourceRun(
        LSource source,
        int order,
        string word,
        LListener listener,
        List<LRecording> found,
        CancellationToken cancellation)
    {
        listener.LListenerSourceStart(source.LSourceName, order);

        LAnswer answer = await LHarvestAnswerRead(source, word, cancellation).ConfigureAwait(false);
        cancellation.ThrowIfCancellationRequested();

        LRecording recording = new(source.LSourceName, answer.LAnswerValue, order, answer.LAnswerReached);
        lock (found)
        {
            found.Add(recording);
        }

        listener.LListenerRecordingAdd(recording);
    }

    private static async Task<LAnswer> LHarvestAnswerRead(
        LSource source,
        string word,
        CancellationToken cancellation)
    {
        using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        deadline.CancelAfter(LHarvestDeadline);

        try
        {
            return await source.LSourceFind(word, deadline.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return LAnswer.LAnswerLost;
        }
    }
}
