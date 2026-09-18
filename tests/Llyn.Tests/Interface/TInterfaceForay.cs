using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LForay TTenureRecordingStart(
        this LTenure tenure, string word, long target, LListener listener) =>
        tenure.LTenureRecordingStart(word, target, listener);

    internal static LForay TTenureTranscriptionStart(
        this LTenure tenure, string word, long target, string scheme, LReceiver receiver) =>
        tenure.LTenureTranscriptionStart(word, target, scheme, receiver);

    internal static void TForayCancel(this LForay foray)
    {
        foray.LForayCancel();
    }

    internal static Task<bool> TForayRecordingSave(this LForay foray, LRecording recording) =>
        foray.LForayRecordingSave(recording);
}
