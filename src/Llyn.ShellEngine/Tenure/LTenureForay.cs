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
        string word = LForay.LForayWordRead(LTenureRead());
        if (word.Length == 0)
        {
            return null;
        }

        LListenerRelay listener = new(sink);
        string language = LTenureLanguageRead();
        LForay foray = new(_lEngine, this, word, language, target, string.Empty);
        lock (_lTenureGate)
        {
            _lTenureRecordingForay?.LForayCancel();
            _lTenureRecordingForay = foray;
        }

        foray.LForayStart(
            token => _lEngine.LEnginePronunciation.LEngineRecordingFind(LTenureId, word, language, target, sink, token),
            listener.LListenerFinish);
        return foray;
    }

    public LForay? LTenureTranscriptionStart(long target, string scheme, Action<LForay, LLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(sink);

        LTenurePersist();
        string word = LForay.LForayWordRead(LTenureRead());
        if (word.Length == 0)
        {
            return null;
        }

        string language = LTenureLanguageRead();
        LForay foray = new(_lEngine, this, word, language, target, scheme);
        Action<LLookupStep> relay = step => sink(foray, step);
        LReceiverRelay receiver = new(relay);
        lock (_lTenureGate)
        {
            _lTenureTranscriptionForay?.LForayCancel();
            _lTenureTranscriptionForay = foray;
        }

        foray.LForayStart(
            token => scheme.Length == 0
                ? _lEngine.LEnginePronunciation.LEnginePronunciationFind(LTenureId, word, language, relay, token)
                : _lEngine.LEnginePronunciation.LEngineTranscriptionFind(
                    LTenureId, word, language, scheme, relay, token),
            receiver.LReceiverLookupFinish);
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
