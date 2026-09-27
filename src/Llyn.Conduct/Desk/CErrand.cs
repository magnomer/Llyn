using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CErrand
{
    private readonly CDesk _cErrandDesk;

    private LForay? _cErrandRecording;

    private LForay? _cErrandTranscription;

    internal CErrand(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cErrandDesk = desk;
    }

    internal LForay? CErrandRecording => _cErrandRecording;

    internal LForay? CErrandTranscription => _cErrandTranscription;

    public bool CErrandRecordingStart(string word, long target, Action<CHarvestStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        CErrandForayStop(_cErrandRecording);
        _cErrandRecording = CErrandRecordingRun(_cErrandDesk.CDeskTenure, word, target, CErrandHarvestSend);
        return _cErrandRecording is not null;

        void CErrandHarvestSend(LHarvestStep step)
        {
            sink(new CHarvestStep(
                step.LHarvestStepSource, step.LHarvestStepOrder,
                CErrandRecordingRead(step.LHarvestStepRecording), step.LHarvestStepEnded));
        }
    }

    public bool CErrandTranscriptionStart(string word, long target, string scheme, Action<CLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        CErrandForayStop(_cErrandTranscription);
        _cErrandTranscription = CErrandTranscriptionRun(
            _cErrandDesk.CDeskTenure, word, target, scheme, CErrandLookupSend);
        return _cErrandTranscription is not null;

        void CErrandLookupSend(LLookupStep step)
        {
            sink(new CLookupStep(
                step.LLookupStepSource, step.LLookupStepOrder,
                CErrandCandidateRead(step.LLookupStepCandidate), step.LLookupStepEnded));
        }
    }

    public void CErrandCancel()
    {
        CErrandForayStop(_cErrandRecording);
        _cErrandRecording = null;
        CErrandForayStop(_cErrandTranscription);
        _cErrandTranscription = null;
    }

    private static LForay? CErrandRecordingRun(LTenure? tenure, string word, long target, Action<LHarvestStep> sink)
    {
        return tenure?.LTenureRecordingStart(word, target, sink);
    }

    private static LForay? CErrandTranscriptionRun(
        LTenure? tenure, string word, long target, string scheme, Action<LLookupStep> sink)
    {
        return tenure?.LTenureTranscriptionStart(word, target, scheme, sink);
    }

    private static void CErrandForayStop(LForay? foray)
    {
        foray?.LForayCancel();
    }

    internal static CRecording? CErrandRecordingRead(LRecording? recording)
    {
        return recording is null
            ? null
            : new CRecording(
                recording.LRecordingSource, recording.LRecordingAddress, recording.LRecordingOrder,
                recording.LRecordingReached, recording.LRecordingVariety);
    }

    internal static LRecording CErrandRecordingRead(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return new LRecording(
            recording.CRecordingSource, recording.CRecordingAddress, recording.CRecordingOrder,
            recording.CRecordingReached, recording.CRecordingVariety);
    }

    internal static CCandidate? CErrandCandidateRead(LCandidate? candidate)
    {
        return candidate is null
            ? null
            : new CCandidate(
                candidate.LCandidateSource, candidate.LCandidatePhonetic, candidate.LCandidateOrder,
                candidate.LCandidateReached, candidate.LCandidateVariety, candidate.LCandidateRespelling);
    }
}
