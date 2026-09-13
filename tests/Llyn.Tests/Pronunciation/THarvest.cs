using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class THarvest
{
    private const string THarvestBritish = "https://example.test/tomato-gb.mp3";
    private const string THarvestAmerican = "https://example.test/tomato-us.mp3";

    [Fact]
    public async Task HarvestStart_UntaggedTwoVarieties_ReturnsOnePerVariety()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, THarvestBritish))],
            [TInterface.TVarietyCreate("British", "gb"), TInterface.TVarietyCreate("American", "us")]);

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", string.Empty, TPronunciationHelper.TListenerCreate(), CancellationToken.None);

        Assert.Equal(
            [(0, "British", THarvestBritish), (0, "American", THarvestBritish)],
            found.Select(recording =>
                (recording.LRecordingOrder, recording.LRecordingVariety, recording.LRecordingAddress)));
    }

    [Fact]
    public async Task HarvestStart_UntaggedNoVarieties_ReturnsOneUntagged()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, THarvestBritish))]);

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", string.Empty, TPronunciationHelper.TListenerCreate(), CancellationToken.None);

        LRecording recording = Assert.Single(found);
        Assert.Equal(string.Empty, recording.LRecordingVariety);
        Assert.Equal(THarvestBritish, recording.LRecordingAddress);
    }

    [Fact]
    public async Task HarvestStart_TaggedAndUntagged_FansOutMissingVariety()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [
                TPronunciationHelper.TSourceStubCreate(
                    TInterface.TReadingCreate("British", THarvestBritish),
                    TInterface.TReadingCreate(string.Empty, THarvestAmerican)),
            ],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", string.Empty, TPronunciationHelper.TListenerCreate(), CancellationToken.None);

        Assert.Equal(
            [("British", THarvestBritish), ("American", THarvestAmerican)],
            found.Select(recording => (recording.LRecordingVariety, recording.LRecordingAddress)));
    }

    [Fact]
    public async Task HarvestStart_BothTaggedAmericanFilter_StreamsAmerican()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [
                TPronunciationHelper.TSourceStubCreate(
                    TInterface.TReadingCreate("British", THarvestBritish),
                    TInterface.TReadingCreate("American", THarvestAmerican)),
            ],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", "American", listener, CancellationToken.None);

        Assert.Equal(2, found.Count);
        LRecording recording = Assert.Single(listener.TListenerStubRecordings);
        Assert.Equal("American", recording.LRecordingVariety);
        Assert.Equal(THarvestAmerican, recording.LRecordingAddress);
    }

    [Fact]
    public async Task HarvestStart_UntaggedAmericanFilter_StreamsAmerican()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, THarvestBritish))],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", "American", listener, CancellationToken.None);

        Assert.Equal(2, found.Count);
        LRecording recording = Assert.Single(listener.TListenerStubRecordings);
        Assert.Equal("American", recording.LRecordingVariety);
        Assert.Equal(THarvestBritish, recording.LRecordingAddress);
    }

    [Fact]
    public async Task HarvestStart_BritishAmericanFilter_StreamsNullAddress()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate("British", THarvestBritish))],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", "American", listener, CancellationToken.None);

        Assert.Equal(THarvestBritish, Assert.Single(found).LRecordingAddress);
        LRecording recording = Assert.Single(listener.TListenerStubRecordings);
        Assert.Null(recording.LRecordingAddress);
        Assert.True(recording.LRecordingReached);
        Assert.Equal(string.Empty, recording.LRecordingVariety);
    }

    [Fact]
    public void HarvestRecordingScan_TwoSourcesAmerican_KeepsOnePerSource()
    {
        LRecording[] found =
        [
            TInterface.TRecordingCreate("Oxford", THarvestBritish, 0, true, "British"),
            TInterface.TRecordingCreate("Oxford", THarvestAmerican, 0, true, "American"),
            TInterface.TRecordingCreate("Naver", THarvestBritish, 1, true, "British"),
            TInterface.TRecordingCreate("Lost", null, 2, false, string.Empty),
        ];

        IReadOnlyList<LRecording> kept = TInterface.THarvestRecordingScan(found, "American");

        Assert.Equal(
            [(0, "American", THarvestAmerican), (1, string.Empty, null), (2, string.Empty, null)],
            kept.Select(recording => (recording.LRecordingOrder, recording.LRecordingVariety, recording.LRecordingAddress)));
        Assert.True(kept[1].LRecordingReached);
        Assert.False(kept[2].LRecordingReached);
        Assert.Same(found, TInterface.THarvestRecordingScan(found, string.Empty));
    }

    [Fact]
    public async Task HarvestStart_LostAnswer_ReturnsOneUnreached()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [TPronunciationHelper.TSourceLostCreate()],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", string.Empty, listener, CancellationToken.None);

        LRecording recording = Assert.Single(found);
        Assert.Null(recording.LRecordingAddress);
        Assert.False(recording.LRecordingReached);
        Assert.Equal(1, listener.TListenerStubFinished);
    }

    [Fact]
    public async Task HarvestStart_TwoSources_KeepsSourceOrder()
    {
        LHarvest harvest = TInterface.THarvestCreate(
            [
                TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, THarvestBritish)),
                TPronunciationHelper.TSourceStubCreate(TInterface.TReadingCreate(string.Empty, THarvestAmerican)),
            ],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        IReadOnlyList<LRecording> found = await harvest.THarvestStart(
            "tomato", string.Empty, listener, CancellationToken.None);

        Assert.Equal([0, 0, 1, 1], found.Select(recording => recording.LRecordingOrder));
        Assert.Equal(2, listener.TListenerStubSources.Count);
        Assert.Equal(4, listener.TListenerStubRecordings.Count);
    }
}
