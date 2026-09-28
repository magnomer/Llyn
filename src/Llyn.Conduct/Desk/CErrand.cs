using System;
using System.Threading.Tasks;
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

    public event Action<string, int>? CErrandHarvestStarted;

    public event Action<CRecording>? CErrandRecordingAdded;

    public event Action? CErrandHarvestFinished;

    public event Action<string, int>? CErrandLookupStarted;

    public event Action<CCandidate>? CErrandCandidateAdded;

    public event Action? CErrandLookupFinished;

    public bool CErrandRecordingHeld => _cErrandRecording is not null;

    public string CErrandRecordingLanguage => _cErrandRecording?.LForayLanguage ?? string.Empty;

    public bool CErrandRecordingFlagged => _cErrandRecording?.LForayFlagged ?? false;

    public bool CErrandRecordingPrimary => _cErrandRecording?.LForayPrimary ?? false;

    public long CErrandRecordingTarget => _cErrandRecording?.LForayTarget ?? 0;

    public bool CErrandTranscriptionHeld => _cErrandTranscription is not null;

    public string CErrandTranscriptionLanguage => _cErrandTranscription?.LForayLanguage ?? string.Empty;

    public bool CErrandTranscriptionFlagged => _cErrandTranscription?.LForayFlagged ?? false;

    public bool CErrandTranscriptionPrimary => _cErrandTranscription?.LForayPrimary ?? false;

    public long CErrandTranscriptionTarget => _cErrandTranscription?.LForayTarget ?? 0;

    public string CErrandTranscriptionScheme => _cErrandTranscription?.LForayScheme ?? string.Empty;

    public bool CErrandTranscriptionSchemed => _cErrandTranscription?.LForaySchemed ?? false;

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

    public void CErrandHarvestResonate(CHarvestStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (step.CHarvestStepRecording is CRecording recording)
        {
            CErrandRecordingAdded?.Invoke(recording);
            return;
        }

        if (step.CHarvestStepEnded)
        {
            CErrandHarvestFinished?.Invoke();
            return;
        }

        CErrandHarvestStarted?.Invoke(step.CHarvestStepSource, step.CHarvestStepOrder);
    }

    public void CErrandLookupResonate(CLookupStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (step.CLookupStepCandidate is CCandidate candidate)
        {
            CErrandCandidateAdded?.Invoke(candidate);
            return;
        }

        if (step.CLookupStepEnded)
        {
            CErrandLookupFinished?.Invoke();
            return;
        }

        CErrandLookupStarted?.Invoke(step.CLookupStepSource, step.CLookupStepOrder);
    }

    public Task<bool> CErrandRecordingSave(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return _cErrandRecording?.LForayRecordingSave(CErrandRecordingRead(recording)) ?? Task.FromResult(false);
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
