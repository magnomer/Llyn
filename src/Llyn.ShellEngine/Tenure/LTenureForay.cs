using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private LForay? _lTenureRecordingForay;

    private LForay? _lTenureTranscriptionForay;

    public LForay? LTenureRecordingStart(long target, Action<LHarvestStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        LTenurePersist();
        string word;
        string language;
        try
        {
            word = LForay.LForayWordRead(LTenureRead());
            language = word.Length == 0 ? string.Empty : LTenureLanguageRead();
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceStaleCheck(exception))
        {
            return null;
        }

        if (word.Length == 0)
        {
            return null;
        }

        LForay foray = new(_lEngine, this, word, language, target, string.Empty);
        lock (_lTenureGate)
        {
            _lTenureRecordingForay?.LForayCancel();
            _lTenureRecordingForay = foray;
        }

        LListenerRelay listener = new(sink);
        foray.LForayStart(
            token => _lEngine.LEnginePronunciation.LEngineRecordingFind(
                LTenureId, word, language, target, listener, token),
            () =>
            {
                if (!listener.LListenerRelayEnded)
                {
                    listener.LListenerFinish();
                }
            });
        return foray;
    }

    public LForay? LTenureTranscriptionStart(long target, string scheme, Action<LForay, LLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(sink);

        LTenurePersist();
        string word;
        string language;
        try
        {
            word = LForay.LForayWordRead(LTenureRead());
            language = word.Length == 0 ? string.Empty : LTenureLanguageRead();
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceStaleCheck(exception))
        {
            return null;
        }

        if (word.Length == 0)
        {
            return null;
        }

        LForay foray = new(_lEngine, this, word, language, target, scheme);
        LReceiverRelay receiver = new(step => sink(foray, step));
        lock (_lTenureGate)
        {
            _lTenureTranscriptionForay?.LForayCancel();
            _lTenureTranscriptionForay = foray;
        }

        foray.LForayStart(
            token => scheme.Length == 0
                ? _lEngine.LEnginePronunciation.LEnginePronunciationFind(LTenureId, word, language, receiver, token)
                : _lEngine.LEnginePronunciation.LEngineTranscriptionFind(
                    LTenureId, word, language, scheme, receiver, token),
            () =>
            {
                if (!receiver.LReceiverRelayEnded)
                {
                    receiver.LReceiverLookupFinish();
                }
            });
        return foray;
    }

    private void LTenureForayStop()
    {
        _lTenureRecordingForay?.LForayCancel();
        _lTenureRecordingForay = null;
        _lTenureTranscriptionForay?.LForayCancel();
        _lTenureTranscriptionForay = null;
    }
}
