using Llyn.Core;
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
