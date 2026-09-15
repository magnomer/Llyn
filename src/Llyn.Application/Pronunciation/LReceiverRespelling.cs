using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReceiverRespelling : LReceiver
{
    private readonly LReceiver _lReceiverRespellingInner;

    private readonly IReadOnlyList<LRespelling> _lReceiverRespellingGroups;

    public LReceiverRespelling(LReceiver inner, IReadOnlyList<LRespelling> groups)
    {
        _lReceiverRespellingInner = inner ?? throw new ArgumentNullException(nameof(inner));
        _lReceiverRespellingGroups = groups ?? throw new ArgumentNullException(nameof(groups));
    }

    public void LReceiverSourceStart(string source, int order)
    {
        _lReceiverRespellingInner.LReceiverSourceStart(source, order);
    }

    public void LReceiverCandidateAdd(LCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        _lReceiverRespellingInner.LReceiverCandidateAdd(
            candidate.LCandidatePhonetic is string phonetic
                ? candidate with
                {
                    LCandidateRespelling = LRespelling.LRespellingScan(
                        _lReceiverRespellingGroups, phonetic, candidate.LCandidateVariety),
                }
                : candidate);
    }

    public void LReceiverLookupFinish()
    {
        _lReceiverRespellingInner.LReceiverLookupFinish();
    }
}
