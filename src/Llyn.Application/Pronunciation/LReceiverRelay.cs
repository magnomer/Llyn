using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReceiverRelay : LReceiver
{
    private readonly Action<LLookupStep> _lReceiverRelaySink;

    public LReceiverRelay(Action<LLookupStep> sink)
    {
        _lReceiverRelaySink = sink ?? throw new ArgumentNullException(nameof(sink));
    }

    public void LReceiverSourceStart(string source, int order)
    {
        _lReceiverRelaySink(new LLookupStep(LLookupKind.LLookupKindSource, source, order, null));
    }

    public void LReceiverCandidateAdd(LCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        _lReceiverRelaySink(new LLookupStep(
            LLookupKind.LLookupKindCandidate, candidate.LCandidateSource, candidate.LCandidateOrder, candidate));
    }

    public void LReceiverLookupFinish()
    {
        _lReceiverRelaySink(new LLookupStep(LLookupKind.LLookupKindEnd, string.Empty, 0, null));
    }
}
