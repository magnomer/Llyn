using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private LForay? _lTenureRecordingForay;

    private LForay? _lTenureTranscriptionForay;

    public LForay LTenureRecordingStart(string word, long target, LListener listener)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(listener);

        string language = LTenureLanguageRead();
        LForay foray = new(_lEngine, this, word, language, target, string.Empty);
        lock (_lTenureGate)
        {
            _lTenureRecordingForay?.LForayCancel();
            _lTenureRecordingForay = foray;
        }

        foray.LForayStart(
            token => _lEngine.LEngineRecordingFind(LTenureId, word, language, target, listener, token),
            listener.LListenerFinish);
        return foray;
    }

    public LForay LTenureTranscriptionStart(string word, long target, string scheme, LReceiver receiver)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(receiver);

        string language = LTenureLanguageRead();
        LForay foray = new(_lEngine, this, word, language, target, scheme);
        lock (_lTenureGate)
        {
            _lTenureTranscriptionForay?.LForayCancel();
            _lTenureTranscriptionForay = foray;
        }

        foray.LForayStart(
            token => scheme.Length == 0
                ? _lEngine.LEnginePronunciationFind(LTenureId, word, language, receiver, token)
                : _lEngine.LEngineTranscriptionFind(LTenureId, word, language, scheme, receiver, token),
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
