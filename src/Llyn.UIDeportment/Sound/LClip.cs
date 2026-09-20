using System;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LClip
{
    private LForay? _lClipForay;

    public event Action<string, int>? LClipSourceStarted;

    public event Action<LRecording>? LClipRecordingAdded;

    public event Action? LClipFinished;

    public bool LClipHeld => _lClipForay is not null;

    public string LClipLanguage => _lClipForay?.LForayLanguage ?? string.Empty;

    public bool LClipFlagged => _lClipForay?.LForayFlagged ?? false;

    public bool LClipPrimary => _lClipForay?.LForayPrimary ?? false;

    public long LClipTarget => _lClipForay?.LForayTarget ?? 0;

    public void LClipForaySet(LForay? foray)
    {
        LClipCancel();
        _lClipForay = foray;
    }

    public void LClipCancel()
    {
        _lClipForay?.LForayCancel();
        _lClipForay = null;
    }

    public void LClipStepHandle(LHarvestStep step)
    {
        ArgumentNullException.ThrowIfNull(step);

        if (step.LHarvestStepRecording is LRecording recording)
        {
            LClipRecordingAdded?.Invoke(recording);
            return;
        }

        if (step.LHarvestStepEnded)
        {
            LClipFinished?.Invoke();
            return;
        }

        LClipSourceStarted?.Invoke(step.LHarvestStepSource, step.LHarvestStepOrder);
    }

    public Task<bool> LClipRecordingSave(LRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return _lClipForay?.LForayRecordingSave(recording) ?? Task.FromResult(false);
    }
}
