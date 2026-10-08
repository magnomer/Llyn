using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LForay? TTenureRecordingStart(this LTenure tenure, long target, Action<LHarvestStep> sink) =>
        tenure.LTenureErrand.LErrandRecordingStart(target, sink);

    internal static LForay? TTenureTranscriptionStart(
        this LTenure tenure, long target, string scheme, Action<LLookupStep> sink) =>
        tenure.LTenureErrand.LErrandTranscriptionStart(target, scheme, (_, step) => sink(step));

    internal static void TForayCancel(this LForay foray)
    {
        foray.LForayCancel();
    }

    internal static Task<bool?> TForayRecordingSave(this LForay foray, LRecording recording) =>
        foray.LForayRecordingSave(recording);

    internal static Task<string?> TForayRecordingPrepare(this LForay foray, LRecording recording) =>
        foray.LForayRecordingPrepare(recording);

    internal static Task TForayEnsignLoad(
        this LForay foray, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store) =>
        foray.LForayEnsignLoad(store);
}
