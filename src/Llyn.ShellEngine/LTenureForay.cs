using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private LForay? _lTenureRecordingForay;

    private LForay? _lTenureTranscriptionForay;

    public LForay LTenureRecordingStart(string word, long target, Action<LHarvestStep> sink)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(sink);

        LListenerRelay listener = new(sink);
        string language = LTenureLanguageRead();
        LForay foray = new(_lEngine, this, word, language, target, string.Empty);
        lock (_lTenureGate)
        {
            _lTenureRecordingForay?.LForayCancel();
            _lTenureRecordingForay = foray;
        }

        foray.LForayStart(
            token => _lEngine.LEngineRecordingFind(LTenureId, word, language, target, sink, token),
            listener.LListenerFinish);
        return foray;
    }

    public LForay LTenureTranscriptionStart(string word, long target, string scheme, Action<LLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(word);
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(sink);

        LReceiverRelay receiver = new(sink);
        string language = LTenureLanguageRead();
        LForay foray = new(_lEngine, this, word, language, target, scheme);
        lock (_lTenureGate)
        {
            _lTenureTranscriptionForay?.LForayCancel();
            _lTenureTranscriptionForay = foray;
        }

        foray.LForayStart(
            token => scheme.Length == 0
                ? _lEngine.LEnginePronunciationFind(LTenureId, word, language, sink, token)
                : _lEngine.LEngineTranscriptionFind(LTenureId, word, language, scheme, sink, token),
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
