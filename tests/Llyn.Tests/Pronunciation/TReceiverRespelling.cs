using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TReceiverRespelling
{
    private static readonly IReadOnlyList<LRespelling> TReceiverRespellingGroups =
    [
        TInterface.TRespellingCreate("British", ["British"], [TInterface.TRespellingRuleCreate("æ", "a")]),
        TInterface.TRespellingCreate("Shared", [], [TInterface.TRespellingRuleCreate("ɪ$", "i")]),
    ];

    [Fact]
    public void ReceiverCandidateAdd_Phonetic_RewritesTextOnly()
    {
        TReceiverStub inner = TPronunciationHelper.TReceiverCreate();
        LReceiver receiver = TInterface.TReceiverRespellingCreate(inner, TReceiverRespellingGroups);

        receiver.TReceiverCandidateAdd(TInterface.TCandidateCreate("Cambridge", "ˈhæpɪ", 2, true, "British"));

        LCandidate forwarded = Assert.Single(inner.TReceiverStubCandidates);
        Assert.Equal("ˈhapi", forwarded.LCandidatePhonetic);
        Assert.Equal("Cambridge", forwarded.LCandidateSource);
        Assert.Equal(2, forwarded.LCandidateOrder);
        Assert.True(forwarded.LCandidateReached);
        Assert.Equal("British", forwarded.LCandidateVariety);
    }

    [Fact]
    public void ReceiverCandidateAdd_NullPhonetic_ForwardsUntouched()
    {
        TReceiverStub inner = TPronunciationHelper.TReceiverCreate();
        LReceiver receiver = TInterface.TReceiverRespellingCreate(inner, TReceiverRespellingGroups);
        LCandidate candidate = TInterface.TCandidateCreate("Longman", null, 1, false, string.Empty);

        receiver.TReceiverCandidateAdd(candidate);

        Assert.Same(candidate, Assert.Single(inner.TReceiverStubCandidates));
    }

    [Fact]
    public void ReceiverCandidateAdd_OtherVariety_SkipsScopedGroup()
    {
        TReceiverStub inner = TPronunciationHelper.TReceiverCreate();
        LReceiver receiver = TInterface.TReceiverRespellingCreate(inner, TReceiverRespellingGroups);

        receiver.TReceiverCandidateAdd(TInterface.TCandidateCreate("Cambridge", "ˈhæpɪ", 0, true, "American"));

        Assert.Equal("ˈhæpi", Assert.Single(inner.TReceiverStubCandidates).LCandidatePhonetic);
    }

    [Fact]
    public void ReceiverSourceStart_AndFinish_ReachInnerOnce()
    {
        TReceiverStub inner = TPronunciationHelper.TReceiverCreate();
        LReceiver receiver = TInterface.TReceiverRespellingCreate(inner, TReceiverRespellingGroups);

        receiver.TReceiverSourceStart("Cambridge", 0);
        receiver.TReceiverLookupFinish();

        Assert.Equal("Cambridge", Assert.Single(inner.TReceiverStubSources));
        Assert.Equal(1, inner.TReceiverStubFinished);
    }
}
