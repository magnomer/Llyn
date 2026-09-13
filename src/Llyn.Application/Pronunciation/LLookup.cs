using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLookup : LSeeker
{
    private static readonly TimeSpan LLookupDeadline = TimeSpan.FromMinutes(1);

    private readonly IReadOnlyList<LSource> _lLookupSources;

    private readonly IReadOnlyList<LVariety> _lLookupVarieties;

    private readonly IReadOnlyList<LRespelling> _lLookupCleanups;

    public LLookup(
        IReadOnlyList<LSource> sources,
        IReadOnlyList<LVariety> varieties,
        IReadOnlyList<LRespelling> cleanups)
    {
        _lLookupSources = sources ?? throw new ArgumentNullException(nameof(sources));
        _lLookupVarieties = varieties ?? throw new ArgumentNullException(nameof(varieties));
        _lLookupCleanups = cleanups ?? throw new ArgumentNullException(nameof(cleanups));
    }

    public async Task<IReadOnlyList<LCandidate>> LSeekerStart(
        string word,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        List<LCandidate> found = [];
        List<Task> pending = new(_lLookupSources.Count);
        for (int order = 0; order < _lLookupSources.Count; order++)
        {
            pending.Add(LLookupSourceRun(
                _lLookupSources[order], order, word, _lLookupVarieties, _lLookupCleanups, receiver, found, cancellation));
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

        return found.OrderBy(static candidate => candidate.LCandidateOrder).ToList();
    }

    private static async Task LLookupSourceRun(
        LSource source,
        int order,
        string word,
        IReadOnlyList<LVariety> varieties,
        IReadOnlyList<LRespelling> cleanups,
        LReceiver receiver,
        List<LCandidate> found,
        CancellationToken cancellation)
    {
        receiver.LReceiverSourceStart(source.LSourceName, order);

        LAnswer answer = await LLookupAnswerRead(source, word, cancellation).ConfigureAwait(false);
        cancellation.ThrowIfCancellationRequested();

        IReadOnlyList<LReading> readings = LReading.LReadingScan(answer.LAnswerReadings, varieties);
        List<LCandidate> candidates = new(Math.Max(1, readings.Count));
        if (readings.Count == 0)
        {
            candidates.Add(new LCandidate(source.LSourceName, null, order, answer.LAnswerReached, string.Empty));
        }
        else
        {
            foreach (LReading reading in readings)
            {
                string cleaned = LRespelling.LRespellingScan(
                    cleanups, LReading.LReadingNormalize(reading.LReadingPhonetic), reading.LReadingVariety);
                candidates.Add(new LCandidate(
                    source.LSourceName, cleaned, order, answer.LAnswerReached, reading.LReadingVariety));
            }
        }

        lock (found)
        {
            found.AddRange(candidates);
        }

        foreach (LCandidate candidate in candidates)
        {
            receiver.LReceiverCandidateAdd(candidate);
        }
    }

    private static async Task<LAnswer> LLookupAnswerRead(
        LSource source,
        string word,
        CancellationToken cancellation)
    {
        using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        deadline.CancelAfter(LLookupDeadline);

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
