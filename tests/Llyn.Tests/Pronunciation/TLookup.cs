using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLookup
{
    [Fact]
    public async Task LookupStart_TwoReadings_ReturnsTwoCandidates()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [
                TPronunciationHelper.TSourceStubCreate(
                    TInterface.TReadingCreate("British", "təˈmɑːtəʊ"),
                    TInterface.TReadingCreate("American", "təˈmeɪtoʊ")),
            ]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "tomato", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        Assert.Equal(
            [(0, "British", "təˈmɑːtəʊ"), (0, "American", "təˈmeɪtoʊ")],
            found.Select(candidate =>
                (candidate.LCandidateOrder, candidate.LCandidateVariety, candidate.LCandidatePhonetic)));
    }

    [Fact]
    public async Task LookupStart_UntaggedReadingTwoVarieties_ReturnsOnePerVariety()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, "təˈmɑːtəʊ"))],
            [TInterface.TVarietyCreate("British", "gb"), TInterface.TVarietyCreate("American", "us")]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "tomato", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        Assert.Equal(
            [(0, "British", "təˈmɑːtəʊ"), (0, "American", "təˈmɑːtəʊ")],
            found.Select(candidate =>
                (candidate.LCandidateOrder, candidate.LCandidateVariety, candidate.LCandidatePhonetic)));
    }

    [Fact]
    public async Task LookupStart_UntaggedReadingNoVarieties_ReturnsOneUntagged()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, "toˈmate"))]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "tomate", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        LCandidate candidate = Assert.Single(found);
        Assert.Equal(string.Empty, candidate.LCandidateVariety);
        Assert.Equal("toˈmate", candidate.LCandidatePhonetic);
    }

    [Fact]
    public async Task LookupStart_TaggedAndUntagged_FansOutOnlyMissingVariety()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [
                TPronunciationHelper.TSourceStubCreate(
                    TInterface.TReadingCreate("British", "təˈmɑːtəʊ"),
                    TInterface.TReadingCreate(string.Empty, "təˈmeɪtoʊ")),
            ],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "tomato", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        Assert.Equal(
            [("British", "təˈmɑːtəʊ"), ("American", "təˈmeɪtoʊ")],
            found.Select(candidate => (candidate.LCandidateVariety, candidate.LCandidatePhonetic)));
    }

    [Fact]
    public async Task LookupStart_EmptyAnswerWithVarieties_ReturnsOneUntaggedNull()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate()],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "tomato", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        LCandidate candidate = Assert.Single(found);
        Assert.Null(candidate.LCandidatePhonetic);
        Assert.Equal(string.Empty, candidate.LCandidateVariety);
    }


    [Fact]
    public async Task LookupStart_DottedReading_ReturnsNormalizedCandidate()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, "ˈdɪf.ər.əns"))]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "difference", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        Assert.Equal("ˈdɪfərəns", Assert.Single(found).LCandidatePhonetic);
    }

    [Fact]
    public async Task LookupStart_Literal_KeepsSpacesAndSkipsCleanup()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, " nǐ hǎo "))],
            null,
            [TInterface.TRespellingCreate([], [TInterface.TRespellingRuleCreate("ǐ", "i")])],
            true);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "你好", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        Assert.Equal("nǐ hǎo", Assert.Single(found).LCandidatePhonetic);
    }

    [Fact]
    public async Task LookupStart_CleanupGroup_AppliesWithoutSwitch()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, "ˈhæpɪ"))],
            null,
            [TInterface.TRespellingCreate([], [TInterface.TRespellingRuleCreate("ɪ$", "i")])]);
        TReceiverStub receiver = TPronunciationHelper.TReceiverCreate();

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart("happy", receiver, CancellationToken.None);

        Assert.Equal("ˈhæpi", Assert.Single(found).LCandidatePhonetic);
        Assert.Equal("ˈhæpi", Assert.Single(receiver.TReceiverStubCandidates).LCandidatePhonetic);
    }

    [Fact]
    public async Task LookupStart_ScopedCleanupGroup_AppliesToFannedOutRow()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, "hæt"))],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")],
            [TInterface.TRespellingCreate(["British"], [TInterface.TRespellingRuleCreate("æ", "a")])]);

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "hat", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);

        Assert.Equal(
            [("British", "hat"), ("American", "hæt")],
            found.Select(candidate => (candidate.LCandidateVariety, candidate.LCandidatePhonetic)));
    }

    [Fact]
    public async Task LookupStart_SavedThenRespelled_TroveKeepsCleanedText()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate("British", "ˈhæp.i"))]);
        LTrove trove = TInterface.TTroveCreate();

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart(
            "happy", TPronunciationHelper.TReceiverCreate(), CancellationToken.None);
        trove.TTroveCandidateSave(7, "happy", "English", found);

        IReadOnlyList<LCandidate>? held = trove.TTroveCandidateRead(7, "happy", "English");
        Assert.NotNull(held);
        Assert.Equal("ˈhæpi", Assert.Single(held).LCandidatePhonetic);

        TReceiverStub receiver = TPronunciationHelper.TReceiverCreate();
        LReceiver respelling = TInterface.TReceiverRespellingCreate(
            receiver,
            [TInterface.TRespellingCreate(["British"], [TInterface.TRespellingRuleCreate("æ", "a")])]);
        foreach (LCandidate candidate in held)
        {
            respelling.TReceiverCandidateAdd(candidate);
        }

        LCandidate respelled = Assert.Single(receiver.TReceiverStubCandidates);
        Assert.Equal("ˈhæpi", respelled.LCandidatePhonetic);
        Assert.Equal("ˈhapi", respelled.LCandidateRespelling);
        LCandidate kept = Assert.Single(trove.TTroveCandidateRead(7, "happy", "English")!);
        Assert.Equal("ˈhæpi", kept.LCandidatePhonetic);
        Assert.Null(kept.LCandidateRespelling);
    }

    [Fact]
    public async Task LookupStart_EmptyAnswer_ReturnsOneNullPhonetic()
    {
        LSeeker lookup = TInterface.TLookupCreate([TPronunciationHelper.TSourceStubCreate()]);
        TReceiverStub receiver = TPronunciationHelper.TReceiverCreate();

        IReadOnlyList<LCandidate> found = await lookup.TLookupStart("tomato", receiver, CancellationToken.None);

        LCandidate candidate = Assert.Single(found);
        Assert.Null(candidate.LCandidatePhonetic);
        Assert.True(candidate.LCandidateReached);
        Assert.Equal(string.Empty, candidate.LCandidateVariety);
        Assert.Single(receiver.TReceiverStubCandidates);
    }

    [Fact]
    public async Task LookupStart_NotCancelled_FinishesOnce()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [
                TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate("British", "təˈmɑːtəʊ")),
                TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate("American", "təˈmeɪtoʊ")),
            ]);
        TReceiverStub receiver = TPronunciationHelper.TReceiverCreate();

        await lookup.TLookupStart("tomato", receiver, CancellationToken.None);

        Assert.Equal(1, receiver.TReceiverStubFinished);
        Assert.Equal(2, receiver.TReceiverStubSources.Count);
    }

    [Fact]
    public async Task LookupStart_Cancelled_NeverFinishes()
    {
        LSeeker lookup = TInterface.TLookupCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate("British", "təˈmɑːtəʊ"))]);
        TReceiverStub receiver = TPronunciationHelper.TReceiverCreate();
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => lookup.TLookupStart("tomato", receiver, cancellation.Token));

        Assert.Equal(0, receiver.TReceiverStubFinished);
    }
}
