using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LErrand
{
    private readonly LEngine _lEngine;

    private readonly LTenure _lErrandTenure;

    private readonly object _lErrandGate;

    private readonly LQuillPronunciation _lErrandPronunciation;

    private LForay? _lErrandRecording;

    private LForay? _lErrandTranscription;

    internal LErrand(LEngine engine, LTenure tenure, object gate)
    {
        _lEngine = engine;
        _lErrandTenure = tenure;
        _lErrandGate = gate;
        _lErrandPronunciation = new LQuillPronunciation(tenure);
    }

    public LForay? LErrandRecordingStart(long target, Action<LHarvestStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        _lErrandTenure.LTenurePersist();
        string word;
        string language;
        try
        {
            word = LForay.LForayWordRead(_lErrandTenure.LTenureRead());
            language = word.Length == 0 ? string.Empty : _lErrandTenure.LTenureLanguageRead();
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceStaleCheck(exception))
        {
            return null;
        }

        if (word.Length == 0)
        {
            return null;
        }

        LForay foray = new(_lEngine, _lErrandTenure, _lErrandPronunciation, word, language, target, string.Empty);
        lock (_lErrandGate)
        {
            _lErrandRecording?.LForayCancel();
            _lErrandRecording = foray;
        }

        LListenerRelay listener = new(sink);
        foray.LForayStart(
            token => _lEngine.LEnginePronunciation.LEngineRecordingFind(
                _lErrandTenure.LTenureId, word, language, target, listener, token),
            () =>
            {
                if (!listener.LListenerRelayEnded)
                {
                    listener.LListenerFinish();
                }
            });
        return foray;
    }

    public LForay? LErrandTranscriptionStart(long target, string scheme, Action<LForay, LLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(sink);

        _lErrandTenure.LTenurePersist();
        string word;
        string language;
        try
        {
            word = LForay.LForayWordRead(_lErrandTenure.LTenureRead());
            language = word.Length == 0 ? string.Empty : _lErrandTenure.LTenureLanguageRead();
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceStaleCheck(exception))
        {
            return null;
        }

        if (word.Length == 0)
        {
            return null;
        }

        LForay foray = new(_lEngine, _lErrandTenure, _lErrandPronunciation, word, language, target, scheme);
        LReceiverRelay receiver = new(step => sink(foray, step));
        lock (_lErrandGate)
        {
            _lErrandTranscription?.LForayCancel();
            _lErrandTranscription = foray;
        }

        foray.LForayStart(
            token => scheme.Length == 0
                ? _lEngine.LEnginePronunciation.LEnginePronunciationFind(
                    _lErrandTenure.LTenureId, word, language, receiver, token)
                : _lEngine.LEnginePronunciation.LEngineTranscriptionFind(
                    _lErrandTenure.LTenureId, word, language, scheme, receiver, token),
            () =>
            {
                if (!receiver.LReceiverRelayEnded)
                {
                    receiver.LReceiverLookupFinish();
                }
            });
        return foray;
    }

    internal void LErrandStop()
    {
        _lErrandRecording?.LForayCancel();
        _lErrandRecording = null;
        _lErrandTranscription?.LForayCancel();
        _lErrandTranscription = null;
    }

    public void LErrandReadingSet(LForay foray, string phonetic, string variety)
    {
        ArgumentNullException.ThrowIfNull(foray);
        ArgumentNullException.ThrowIfNull(phonetic);
        ArgumentNullException.ThrowIfNull(variety);

        LEntryDraft? content = _lErrandTenure.LTenureRead()?.LDraftContent;
        long target = foray.LForayTarget;
        if (foray.LForaySchemed)
        {
            if (LErrandRowCheck(content?.LEntryDraftTranscriptions, target, static row => row.LTranscriptionDraftId))
            {
                _lErrandTenure.LTenureRequestApply(
                    new LRequestTranscriptionText(_lErrandTenure.LTenureId, target, phonetic));
            }

            return;
        }

        if (foray.LForayPrimary)
        {
            _lErrandTenure.LTenureRequestApply(new LRequestIpa(_lErrandTenure.LTenureId, phonetic));
        }
        else if (LErrandRowCheck(content?.LEntryDraftPronunciations, target, static row => row.LPronunciationDraftId))
        {
            _lErrandTenure.LTenureRequestApply(
                new LRequestPronunciationIpa(_lErrandTenure.LTenureId, target, phonetic));
        }
        else
        {
            return;
        }

        _lErrandPronunciation.LQuillVarietySet(foray.LForayPrimary, target, variety);
    }

    private static bool LErrandRowCheck<LErrandRow>(IReadOnlyList<LErrandRow>? rows, long id, Func<LErrandRow, long> key)
    {
        return rows is not null && id != 0 && LDraftClerkList.LDraftListFind(rows, id, key) >= 0;
    }
}
