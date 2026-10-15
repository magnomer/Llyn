using System;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplayPlayback
{
    private readonly CVoice _cDisplayPlaybackVoice;

    private readonly CLedgerNoticed _cDisplayPlaybackNoticed;

    private readonly LMediaPort _cDisplayPlaybackMedia;

    private readonly CEnvoy _cDisplayPlaybackEnvoy;

    private readonly LSettingsPort _cDisplayPlaybackSettings;

    internal CDisplayPlayback(CRule display, LMediaPort media, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayPlaybackVoice = display.CVoice;
        _cDisplayPlaybackNoticed = display.CRuleNoticed;
        _cDisplayPlaybackMedia = media;
        _cDisplayPlaybackEnvoy = envoy;
        _cDisplayPlaybackSettings = settings;
    }

    private LEntryDraft? LDisplayPlaybackShown => _cDisplayPlaybackVoice.CVoiceShown;

    public CLecternPlayback CDisplayPlaybackRead()
    {
        if (LDisplayPlaybackShown is not LEntryDraft shown)
        {
            return new CLecternPlayback(false, false);
        }

        try
        {
            (bool recorded, bool audible) = _cDisplayPlaybackMedia.LEnginePlaybackRead(shown);
            return new CLecternPlayback(recorded, audible);
        }
        catch (Exception exception)
        {
            _cDisplayPlaybackNoticed.LLedgerRepaintShow(
                _cDisplayPlaybackEnvoy, _cDisplayPlaybackSettings, "Sound.LoadFailed", exception);
            return new CLecternPlayback(false, false);
        }
    }

    public void CDisplayPlaybackStart(double volume)
    {
        if (LDisplayPlaybackShown is not LEntryDraft shown)
        {
            return;
        }

        LDisplayPlaybackStart(() => _cDisplayPlaybackMedia.LEngineRecordingPlay(shown, volume));
    }

    public void CDisplayPlaybackStart(long accent, double volume)
    {
        if (LDisplayPlaybackShown?.LEntryDraftAccents.FirstOrDefault(row => row.LPronunciationDraftId == accent)
            is not LPronunciationDraft spoken)
        {
            return;
        }

        string audio = spoken.LPronunciationDraftAudio;
        LDisplayPlaybackStart(() => _cDisplayPlaybackMedia.LEngineRecordingPlay(audio, volume));
    }

    private void LDisplayPlaybackStart(Func<int> play)
    {
        try
        {
            _cDisplayPlaybackVoice.CVoiceTicket = play();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cDisplayPlaybackEnvoy, _cDisplayPlaybackSettings, "Sound.PlayFailed", exception);
        }
    }

    public void CDisplayPlaybackCancel()
    {
        _cDisplayPlaybackVoice.CVoicePlaybackStop();
    }
}
