using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PEditorRequestHeadword = "Headword";

    private const string PEditorRequestIpa = "Ipa";

    private const string PEditorRequestNote = "Note";

    private const string PEditorRequestSpeech = "Speech";

    private readonly Dictionary<string, LRequest> _pEditorRequestPending = [];

    private PObserver? _pEditorObserver;

    private static string PEditorRequestFormat(PCard card, string field)
    {
        return string.Concat("Card:", card.PCardId.ToString(CultureInfo.InvariantCulture), ":", field);
    }

    private static string PEditorRequestFormat(PCard card, long rowId, string field)
    {
        return string.Concat(
            PEditorRequestFormat(card, field), ":", rowId.ToString(CultureInfo.InvariantCulture));
    }

    private bool PEditorRequestCheck(string key)
    {
        return _pEditorRequestPending.ContainsKey(key);
    }

    private void PEditorRequestDefer(string key, LRequest request)
    {
        if (_pEditorFill || _pEditorHalted || _pEditorDraft == 0)
        {
            return;
        }

        _pEditorRequestPending[key] = request;
        PEditorChangeDefer();
    }

    private void PEditorRequestSend(LRequest request)
    {
        if (_pEditorFill || _pEditorHalted || _pEditorDraft == 0)
        {
            return;
        }

        PEditorRequestPersist();

        if (_pEditorHalted)
        {
            return;
        }

        try
        {
            _lEngine.LEngineRequestApply(request);
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
            return;
        }

        PEditorChangeUpdate();
    }

    private void PEditorRequestPersist()
    {
        if (_pEditorRequestPending.Count == 0 || _pEditorDraft == 0)
        {
            _pEditorRequestPending.Clear();
            return;
        }

        List<string> keys = [.. _pEditorRequestPending.Keys];
        try
        {
            foreach (string key in keys)
            {
                if (!_pEditorRequestPending.Remove(key, out LRequest? request))
                {
                    continue;
                }

                _lEngine.LEngineRequestApply(request);
            }
        }
        catch (Exception exception)
        {
            _pEditorRequestPending.Clear();
            PEditorHoldSuspend(exception);
        }
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

        if (bulletin.LBulletinSubject != LSubject.LSubjectDraft
            || _pEditorDraft == 0
            || bulletin.LBulletinId != _pEditorDraft)
        {
            return;
        }

        PEditorDraftRestore();
    }

    private void PEditorLanguageSend()
    {
        PEditorRequestSend(new LRequestLanguage(_pEditorDraft, _pSpeakerChoice));
    }

    private void PEditorSpeechSend()
    {
        PEditorRequestSend(new LRequestSpeech(_pEditorDraft, PMarkerRead()));
    }

    private void PEditorAudioSend()
    {
        PEditorRequestSend(new LRequestAudio(_pEditorDraft, _pRecording ?? string.Empty, _pRecordingSource));
    }
}
