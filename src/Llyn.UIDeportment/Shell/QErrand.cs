using System;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class QErrand
{
    private readonly LDesk _qErrandDesk;

    private LForay? _qErrandRecording;

    private LForay? _qErrandTranscription;

    internal QErrand(LDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _qErrandDesk = desk;
    }

    internal LForay? QErrandRecording => _qErrandRecording;

    internal LForay? QErrandTranscription => _qErrandTranscription;

    public bool QErrandRecordingStart(string word, long target, Action<CHarvestStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        QErrandForayStop(_qErrandRecording);
        _qErrandRecording = QErrandRecordingRun(_qErrandDesk.LDeskTenure, word, target, QErrandHarvestSend);
        return _qErrandRecording is not null;

        void QErrandHarvestSend(LHarvestStep step)
        {
            sink(new CHarvestStep(
                step.LHarvestStepSource, step.LHarvestStepOrder,
                QErrandRecordingRead(step.LHarvestStepRecording), step.LHarvestStepEnded));
        }
    }

    public bool QErrandTranscriptionStart(string word, long target, string scheme, Action<CLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        QErrandForayStop(_qErrandTranscription);
        _qErrandTranscription = QErrandTranscriptionRun(
            _qErrandDesk.LDeskTenure, word, target, scheme, QErrandLookupSend);
        return _qErrandTranscription is not null;

        void QErrandLookupSend(LLookupStep step)
        {
            sink(new CLookupStep(
                step.LLookupStepSource, step.LLookupStepOrder,
                QErrandCandidateRead(step.LLookupStepCandidate), step.LLookupStepEnded));
        }
    }

    public void QErrandCancel()
    {
        QErrandForayStop(_qErrandRecording);
        _qErrandRecording = null;
        QErrandForayStop(_qErrandTranscription);
        _qErrandTranscription = null;
    }

    private static LForay? QErrandRecordingRun(LTenure? tenure, string word, long target, Action<LHarvestStep> sink)
    {
        return tenure?.LTenureRecordingStart(word, target, sink);
    }

    private static LForay? QErrandTranscriptionRun(
        LTenure? tenure, string word, long target, string scheme, Action<LLookupStep> sink)
    {
        return tenure?.LTenureTranscriptionStart(word, target, scheme, sink);
    }

    private static void QErrandForayStop(LForay? foray)
    {
        foray?.LForayCancel();
    }

    internal static CRecording? QErrandRecordingRead(LRecording? recording)
    {
        return recording is null
            ? null
            : new CRecording(
                recording.LRecordingSource, recording.LRecordingAddress, recording.LRecordingOrder,
                recording.LRecordingReached, recording.LRecordingVariety);
    }

    internal static LRecording QErrandRecordingRead(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return new LRecording(
            recording.CRecordingSource, recording.CRecordingAddress, recording.CRecordingOrder,
            recording.CRecordingReached, recording.CRecordingVariety);
    }

    internal static CCandidate? QErrandCandidateRead(LCandidate? candidate)
    {
        return candidate is null
            ? null
            : new CCandidate(
                candidate.LCandidateSource, candidate.LCandidatePhonetic, candidate.LCandidateOrder,
                candidate.LCandidateReached, candidate.LCandidateVariety, candidate.LCandidateRespelling);
    }
}
