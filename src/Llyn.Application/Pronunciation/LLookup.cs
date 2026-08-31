using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

/// <summary>
/// Fans a pronunciation request out to every configured pronunciation source concurrently and
/// streams each result back to the receiver as it arrives. One slow or failing source never blocks
/// or fails the others; the lookup reports complete once all sources have finished. The sources are
/// supplied ready-built and language-agnostic (see <see cref="LSource"/>); this orchestrator knows
/// nothing about any particular source or language.
/// </summary>
public sealed class LLookup : LSeeker
{
    private const string LLookupKind = "pronunciation";

    private readonly IReadOnlyList<LSource> _lLookupSources;

    public LLookup(IReadOnlyList<LSource> sources)
    {
        _lLookupSources = sources ?? throw new ArgumentNullException(nameof(sources));
    }

    public async Task LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        List<Task> pending = new(_lLookupSources.Count);
        foreach (LSource source in _lLookupSources)
        {
            if (string.Equals(source.LSourceKind, LLookupKind, StringComparison.Ordinal))
            {
                pending.Add(LLookupSourceRun(source, word, receiver, cancellation));
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
                receiver.LReceiverLookupFinish();
            }
        }
    }

    private static async Task LLookupSourceRun(
        LSource source,
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
            // A failed source yields no candidate; the lookup still completes with the others.
            return;
        }

        if (!string.IsNullOrEmpty(phonetic) && !cancellation.IsCancellationRequested)
        {
            receiver.LReceiverCandidateAdd(new LCandidate(source.LSourceName, phonetic));
        }
    }
}
