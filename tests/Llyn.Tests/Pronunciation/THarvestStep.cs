using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class THarvestStep
{
    private const string THarvestStepBritish = "https://example.test/tomato-gb.mp3";
    private const string THarvestStepAmerican = "https://example.test/tomato-us.mp3";

    [Fact]
    public async Task ListenerRelay_TwoRecordingSource_EmitsStartRecordingsFinish()
    {
        List<LHarvestStep> steps = [];
        LHarvest harvest = TInterface.THarvestCreate(
            [
                TPronunciationHelper.TSourceStubCreate(
                    TInterface.TReadingCreate("British", THarvestStepBritish),
                    TInterface.TReadingCreate("American", THarvestStepAmerican)),
            ]);

        await harvest.THarvestStart(
            "tomato", string.Empty, TInterface.TListenerRelayCreate(steps.Add), CancellationToken.None);

        Assert.Equal(
            [
                LHarvestKind.LHarvestKindSource,
                LHarvestKind.LHarvestKindRecording,
                LHarvestKind.LHarvestKindRecording,
                LHarvestKind.LHarvestKindEnd,
            ],
            steps.Select(step => step.LHarvestStepKind));
        Assert.Equal(("Stub", 0), (steps[0].LHarvestStepSource, steps[0].LHarvestStepOrder));
        Assert.Null(steps[0].LHarvestStepRecording);
        Assert.Equal(
            [("British", THarvestStepBritish), ("American", THarvestStepAmerican)],
            steps.Skip(1).Take(2).Select(step =>
                (step.LHarvestStepRecording!.LRecordingVariety, step.LHarvestStepRecording.LRecordingAddress)));
        Assert.Equal((string.Empty, 0), (steps[3].LHarvestStepSource, steps[3].LHarvestStepOrder));
        Assert.Null(steps[3].LHarvestStepRecording);
    }

    [Fact]
    public async Task ReceiverRelay_TwoReadingSource_EmitsStartCandidatesFinish()
    {
        List<LLookupStep> steps = [];
        LSeeker lookup = TInterface.TLookupCreate(
            [
                TPronunciationHelper.TSourceStubCreate(
                    TInterface.TReadingCreate("British", "təˈmɑːtəʊ"),
                    TInterface.TReadingCreate("American", "təˈmeɪtoʊ")),
            ]);

        await lookup.TLookupStart("tomato", TInterface.TReceiverRelayCreate(steps.Add), CancellationToken.None);

        Assert.Equal(
            [
                LLookupKind.LLookupKindSource,
                LLookupKind.LLookupKindCandidate,
                LLookupKind.LLookupKindCandidate,
                LLookupKind.LLookupKindEnd,
            ],
            steps.Select(step => step.LLookupStepKind));
        Assert.Equal(("Stub", 0), (steps[0].LLookupStepSource, steps[0].LLookupStepOrder));
        Assert.Equal(
            [("British", "təˈmɑːtəʊ"), ("American", "təˈmeɪtoʊ")],
            steps.Skip(1).Take(2).Select(step =>
                (step.LLookupStepCandidate!.LCandidateVariety, step.LLookupStepCandidate.LCandidatePhonetic)));
        Assert.Null(steps[3].LLookupStepCandidate);
    }

    [Fact]
    public void ReceiverRespelling_OverRelay_RespellsCandidateStep()
    {
        List<LLookupStep> steps = [];
        LReceiver receiver = TInterface.TReceiverRespellingCreate(
            TInterface.TReceiverRelayCreate(steps.Add),
            [TInterface.TRespellingCreate("British", ["British"], [TInterface.TRespellingRuleCreate("æ", "a")])]);

        receiver.TReceiverCandidateAdd(TInterface.TCandidateCreate("Cambridge", "ˈhæpɪ", 2, true, "British"));

        LLookupStep step = Assert.Single(steps);
        Assert.Equal(LLookupKind.LLookupKindCandidate, step.LLookupStepKind);
        Assert.Equal(("Cambridge", 2), (step.LLookupStepSource, step.LLookupStepOrder));
        Assert.Equal("ˈhapɪ", step.LLookupStepCandidate?.LCandidateRespelling);
    }
}
