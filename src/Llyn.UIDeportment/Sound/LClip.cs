using System;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LClip
{
    private readonly LDesk _lClipDesk;

    internal LClip(LDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _lClipDesk = desk;
    }

    public event Action<string, int>? LClipSourceStarted;

    public event Action<CRecording>? LClipRecordingAdded;

    public event Action? LClipFinished;

    public bool LClipHeld => _lClipDesk.LDeskErrand.QErrandRecording is not null;

    public string LClipLanguage => _lClipDesk.LDeskErrand.QErrandRecording?.LForayLanguage ?? string.Empty;

    public bool LClipFlagged => _lClipDesk.LDeskErrand.QErrandRecording?.LForayFlagged ?? false;

    public bool LClipPrimary => _lClipDesk.LDeskErrand.QErrandRecording?.LForayPrimary ?? false;

    public long LClipTarget => _lClipDesk.LDeskErrand.QErrandRecording?.LForayTarget ?? 0;

    public void LClipCancel()
    {
        _lClipDesk.LDeskErrand.QErrandCancel();
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

        return _lClipDesk.LDeskErrand.QErrandRecording?.LForayRecordingSave(QErrand.QErrandRecordingRead(recording))
            ?? Task.FromResult(false);
    }
}
