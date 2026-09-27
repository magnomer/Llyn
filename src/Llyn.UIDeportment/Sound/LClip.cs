using System;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LClip
{
    private readonly CDesk _lClipDesk;

    internal LClip(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _lClipDesk = desk;
    }

    public event Action<string, int>? LClipSourceStarted;

    public event Action<CRecording>? LClipRecordingAdded;

    public event Action? LClipFinished;

    public bool LClipHeld => _lClipDesk.CDeskErrand.CErrandRecording is not null;

    public string LClipLanguage => _lClipDesk.CDeskErrand.CErrandRecording?.LForayLanguage ?? string.Empty;

    public bool LClipFlagged => _lClipDesk.CDeskErrand.CErrandRecording?.LForayFlagged ?? false;

    public bool LClipPrimary => _lClipDesk.CDeskErrand.CErrandRecording?.LForayPrimary ?? false;

    public long LClipTarget => _lClipDesk.CDeskErrand.CErrandRecording?.LForayTarget ?? 0;

    public void LClipCancel()
    {
        _lClipDesk.CDeskErrand.CErrandCancel();
    }

    public void LClipStepHandle(CHarvestStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (step.CHarvestStepRecording is CRecording recording)
        {
            LClipRecordingAdded?.Invoke(recording);
            return;
        }

        if (step.CHarvestStepEnded)
        {
            LClipFinished?.Invoke();
            return;
        }

        LClipSourceStarted?.Invoke(step.CHarvestStepSource, step.CHarvestStepOrder);
    }

    public Task<bool> LClipRecordingSave(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return _lClipDesk.CDeskErrand.CErrandRecording?.LForayRecordingSave(CErrand.CErrandRecordingRead(recording))
            ?? Task.FromResult(false);
    }
}
