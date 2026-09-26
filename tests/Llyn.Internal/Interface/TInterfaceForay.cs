using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LForay TTenureRecordingStart(
        this LTenure tenure, string word, long target, Action<LHarvestStep> sink) =>
        tenure.LTenureRecordingStart(word, target, sink);

    internal static LForay TTenureTranscriptionStart(
        this LTenure tenure, string word, long target, string scheme, Action<LLookupStep> sink) =>
        tenure.LTenureTranscriptionStart(word, target, scheme, sink);

    internal static void TForayCancel(this LForay foray)
    {
        foray.LForayCancel();
    }

    internal static Task<bool> TForayRecordingSave(this LForay foray, LRecording recording) =>
        foray.LForayRecordingSave(recording);
}
