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

    private readonly IReadOnlyList<LVariety> _lHarvestVarieties;

    public LHarvest(IReadOnlyList<LSource> sources, IReadOnlyList<LVariety> varieties)
    {
        _lHarvestSources = sources ?? throw new ArgumentNullException(nameof(sources));
        _lHarvestVarieties = varieties ?? throw new ArgumentNullException(nameof(varieties));
    }

    public async Task<IReadOnlyList<LRecording>> LHarvestStart(
        string word,
        string variety,
        LListener listener,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(variety);
        ArgumentNullException.ThrowIfNull(listener);

        List<LRecording> found = [];
        List<Task> pending = new(_lHarvestSources.Count);
        for (int order = 0; order < _lHarvestSources.Count; order++)
        {
            pending.Add(LHarvestSourceRun(
                _lHarvestSources[order], order, word, variety, _lHarvestVarieties, listener, found, cancellation));
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
        string variety,
        IReadOnlyList<LVariety> varieties,
        LListener listener,
        List<LRecording> found,
        CancellationToken cancellation)
    {
        listener.LListenerSourceStart(source.LSourceName, order);

        LAnswer answer = await LHarvestAnswerRead(source, word, cancellation).ConfigureAwait(false);
        cancellation.ThrowIfCancellationRequested();

        IReadOnlyList<LReading> readings = LReading.LReadingScan(answer.LAnswerReadings, varieties);
        List<LRecording> recordings = new(Math.Max(1, readings.Count));
        if (readings.Count == 0)
        {
            recordings.Add(new LRecording(source.LSourceName, null, order, answer.LAnswerReached, string.Empty));
        }
        else
        {
            foreach (LReading reading in readings)
            {
                recordings.Add(new LRecording(
                    source.LSourceName,
                    reading.LReadingPhonetic,
                    order,
                    answer.LAnswerReached,
                    reading.LReadingVariety));
            }
        }

        lock (found)
        {
            found.AddRange(recordings);
        }

        foreach (LRecording recording in LHarvestRecordingScan(recordings, variety))
        {
            listener.LListenerRecordingAdd(recording);
        }
    }

    public static IReadOnlyList<LRecording> LHarvestRecordingScan(IReadOnlyList<LRecording> recordings, string variety)
    {
        ArgumentNullException.ThrowIfNull(recordings);
        ArgumentNullException.ThrowIfNull(variety);

        if (variety.Length == 0)
        {
            return recordings;
        }

        List<LRecording> kept = [];
        int position = 0;
        while (position < recordings.Count)
        {
            int order = recordings[position].LRecordingOrder;
            int start = position;
            int held = kept.Count;
            while (position < recordings.Count && recordings[position].LRecordingOrder == order)
            {
                LRecording recording = recordings[position];
                if (string.Equals(recording.LRecordingVariety, variety, StringComparison.Ordinal))
                {
                    kept.Add(recording);
                }

                position++;
            }

            if (kept.Count == held)
            {
                LRecording first = recordings[start];
                kept.Add(new LRecording(first.LRecordingSource, null, order, first.LRecordingReached, string.Empty));
            }
        }

        return kept;
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
