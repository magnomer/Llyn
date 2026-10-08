using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CErrand
{
    private readonly CDesk _cErrandDesk;

    private LForay? _cErrandRecording;

    private LForay? _cErrandTranscription;

    private readonly CClip _cErrandClip = new();

    private readonly CNotation _cErrandNotation = new();

    private Action<Action> _cErrandMarshal = static run => run();

    internal CErrand(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cErrandDesk = desk;
    }

    public event Action<CClipRoll>? CErrandClipChanged;

    public event Action<CNotationRoll>? CErrandNotationChanged;

    public CClipRoll CErrandRecordingStart(long? target)
    {
        CErrandCancel();
        _cErrandClip.LClipStart();
        try
        {
            _cErrandRecording = LErrandRecordingRun(
                _cErrandDesk.CDeskDraft.CDeskDraftTenure, target ?? 0, LErrandHarvestSend);
            if (_cErrandRecording is null)
            {
                _cErrandClip.LClipFinish();
            }
        }
        catch (Exception exception)
        {
            _cErrandClip.LClipFinish();
            _cErrandDesk.LDeskFailureShow(".RecordingFailed", exception);
        }

        return _cErrandClip.LClipRead();

        void LErrandHarvestSend(LHarvestStep step)
        {
            CHarvestStep sent = new(
                step.LHarvestStepSource, step.LHarvestStepOrder,
                CErrandRecordingRead(step.LHarvestStepRecording), step.LHarvestStepEnded);
            _cErrandMarshal(() => LErrandHarvestResonate(sent));
        }
    }

    public async Task<CClipRoll> CErrandEnsignLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        try
        {
            await LErrandEnsignRun(_cErrandRecording, store);
        }
        catch (Exception exception)
        {
            _cErrandDesk.LDeskFailureShow(".RecordingFailed", exception);
        }

        return _cErrandClip.LClipRead();
    }

    public CNotationRoll CErrandTranscriptionStart(long? target, string scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        CErrandCancel();
        _cErrandNotation.LNotationStart(scheme.Length > 0);
        try
        {
            _cErrandTranscription = LErrandTranscriptionRun(
                _cErrandDesk.CDeskDraft.CDeskDraftTenure, target ?? 0, scheme, LErrandLookupSend);
            if (_cErrandTranscription is null)
            {
                _cErrandNotation.LNotationFinish();
            }
        }
        catch (Exception exception)
        {
            _cErrandNotation.LNotationFinish();
            _cErrandDesk.LDeskFailureShow(".TranscriptionFailed", exception);
        }

        return _cErrandNotation.LNotationRead();

        void LErrandLookupSend(LForay foray, LLookupStep step)
        {
            CLookupStep sent = new(
                step.LLookupStepSource, step.LLookupStepOrder,
                CErrandCandidateRead(step.LLookupStepCandidate), step.LLookupStepEnded);
            _cErrandMarshal(() => LErrandLookupResonate(sent, foray));
        }
    }

    public async Task<CNotationRoll> CErrandFlagLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        try
        {
            await LErrandEnsignRun(_cErrandTranscription, store);
        }
        catch (Exception exception)
        {
            _cErrandDesk.LDeskFailureShow(".TranscriptionFailed", exception);
        }

        return _cErrandNotation.LNotationRead();
    }

    public void CErrandReadingSet(string phonetic, string variety)
    {
        if (_cErrandTranscription is LForay foray)
        {
            _cErrandDesk.CDeskDraft.CDeskDraftErrand?.LErrandReadingSet(foray, phonetic, variety);
        }
    }

    internal void LErrandObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        _cErrandMarshal = marshal;
    }

    internal void LErrandHarvestResonate(CHarvestStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (step.CHarvestStepRecording is CRecording recording)
        {
            _cErrandClip.LClipRecordingAdd(recording, _cErrandRecording, _cErrandDesk.CDeskDraft.CDeskDraftTenure);
            CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
            return;
        }

        if (step.CHarvestStepEnded)
        {
            _cErrandClip.LClipFinish();
            CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
            return;
        }

        _cErrandClip.LClipPlace(step.CHarvestStepSource, step.CHarvestStepOrder);
        CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
    }

    internal void LErrandLookupResonate(CLookupStep step, LForay foray)
    {
        ArgumentNullException.ThrowIfNull(step);
        ArgumentNullException.ThrowIfNull(foray);

        if (foray.LForayCancelled)
        {
            return;
        }

        if (step.CLookupStepCandidate is CCandidate candidate)
        {
            _cErrandNotation.LNotationCandidateAdd(candidate, foray);
        }
        else if (step.CLookupStepEnded)
        {
            _cErrandNotation.LNotationFinish();
        }
        else
        {
            _cErrandNotation.LNotationPlace(step.CLookupStepSource, step.CLookupStepOrder);
        }

        CErrandNotationChanged?.Invoke(_cErrandNotation.LNotationRead());
    }

    public async Task<Uri?> CErrandPreviewStart(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        if (_cErrandRecording is not LForay foray)
        {
            return null;
        }

        _cErrandClip.LClipPreviewStart(recording);
        CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
        string? path;
        try
        {
            path = await foray.LForayRecordingPrepare(CErrandRecordingRead(recording));
        }
        catch (Exception exception)
        {
            _cErrandDesk.LDeskFailureShow(".RecordingFailed", exception);
            path = null;
        }

        if (path is null || !Uri.TryCreate(path, UriKind.Absolute, out Uri? address))
        {
            _cErrandClip.LClipRefusedSet(recording);
            CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
            return null;
        }

        if (!_cErrandClip.LClipPreviewPlay(recording))
        {
            return null;
        }

        CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
        return address;
    }

    public void CErrandPreviewFinish()
    {
        if (_cErrandClip.LClipPreviewFinish())
        {
            CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
        }
    }

    public async Task<bool> CErrandRecordingSave(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        if (_cErrandRecording is not LForay foray || !_cErrandClip.LClipSaveStart(recording))
        {
            return false;
        }

        CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
        bool? attached;
        try
        {
            attached = await foray.LForayRecordingSave(CErrandRecordingRead(recording));
        }
        catch (Exception exception)
        {
            _cErrandDesk.LDeskFailureShow(".RecordingFailed", exception);
            attached = null;
        }

        _cErrandClip.LClipSaveFinish(recording, attached is not null);
        CErrandClipChanged?.Invoke(_cErrandClip.LClipRead());
        return attached == true;
    }

    public void CErrandCancel()
    {
        _cErrandClip.LClipPreviewFinish();
        CErrandForayStop(_cErrandRecording);
        _cErrandRecording = null;
        CErrandForayStop(_cErrandTranscription);
        _cErrandTranscription = null;
    }

    private static LForay? LErrandRecordingRun(LTenure? tenure, long target, Action<LHarvestStep> sink)
    {
        return tenure?.LTenureErrand.LErrandRecordingStart(target, sink);
    }

    private static Task LErrandEnsignRun(
        LForay? foray, Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        return foray?.LForayEnsignLoad((rows, delete) => store(CCatalog.CCatalogEnsignRead(rows), delete))
            ?? Task.CompletedTask;
    }

    private static LForay? LErrandTranscriptionRun(
        LTenure? tenure, long target, string scheme, Action<LForay, LLookupStep> sink)
    {
        return tenure?.LTenureErrand.LErrandTranscriptionStart(target, scheme, sink);
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
