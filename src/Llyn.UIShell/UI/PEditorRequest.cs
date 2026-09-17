using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PEditor
{
    private PObserver? _pEditorObserver;

    private void PEditorRequestDefer(LRequest request)
    {
        if (_pEditorFill || _pEditorTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestDefer(request);
    }

    private void PEditorRequestSend(LRequest request)
    {
        if (_pEditorFill || _pEditorTenure is not LTenure held)
        {
            return;
        }

        held.LTenureRequestApply(request);
    }

    private void PEditorBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectFrequency)
        {
            if (bulletin.LBulletinId == PEditorEntryRead())
            {
                PEditorFrequencyShow();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectGrasp)
        {
            if (bulletin.LBulletinId == PEditorEntryRead())
            {
                PEditorGraspShow();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectInflection)
        {
            if (bulletin.LBulletinId == PEditorEntryRead())
            {
                PEditorParadigmShow();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectScript)
        {
            PEditorScriptShow();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFanqie)
        {
            PEditorFanqieShow();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectReference)
        {
            PSentenceLoad();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectReflex)
        {
            if (bulletin.LBulletinId == PEditorEntryRead())
            {
                PReflexPendingShow();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectSettings)
        {
            PEditorDraftRestore();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectTenure)
        {
            if (bulletin.LBulletinId == PEditorDraft)
            {
                PEditorChangeUpdate();
            }

            return;
        }

        if (bulletin.LBulletinSubject != LSubject.LSubjectDraft
            || PEditorDraft == 0
            || bulletin.LBulletinId != PEditorDraft)
        {
            return;
        }

        if (_pEditorPreparing)
        {
            _pEditorPrepareStale = true;
            return;
        }

        PEditorDraftRestore();
    }

    private void PEditorLanguageSend()
    {
        PEditorRequestSend(new LRequestLanguage(PEditorDraft, _pSpeakerChoice));
    }

    private void PEditorSpeechSend()
    {
        PEditorRequestSend(new LRequestSpeech(PEditorDraft, PMarkerRead()));
    }

    private void PEditorAudioSend()
    {
        PEditorRequestSend(new LRequestAudio(PEditorDraft, _pRecording ?? string.Empty, _pRecordingSource));
    }
}
