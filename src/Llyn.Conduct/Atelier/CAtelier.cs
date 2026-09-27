using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAtelier : IDisposable
{
    private readonly LPosture _cAtelierPosture;

    internal CAtelier(
        LPosture posture,
        LDraftPort drafts,
        LEntryPort entries,
        LSettingsPort settings,
        LPhonologyPort phonology,
        LMediaPort media,
        LPortraitPort portraits)
    {
        ArgumentNullException.ThrowIfNull(posture);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(portraits);

        _cAtelierPosture = posture;
        CAtelierDraftPort = drafts;
        CAtelierEntryPort = entries;
        CAtelierSettingsPort = settings;
        CAtelierPhonologyPort = phonology;
        CAtelierMediaPort = media;
        CAtelierPortraitPort = portraits;
    }

    internal LDraftPort CAtelierDraftPort { get; }

    internal LEntryPort CAtelierEntryPort { get; }

    internal LSettingsPort CAtelierSettingsPort { get; }

    internal LPhonologyPort CAtelierPhonologyPort { get; }

    internal LMediaPort CAtelierMediaPort { get; }

    internal LPortraitPort CAtelierPortraitPort { get; }

    internal LVista CAtelierVistaStart(string tab, CSubject? subject, CCatalogOrder fallback, bool blank = false)
    {
        return _cAtelierPosture.LPostureVistaStart(tab, (LSubject?)subject, (LCatalogOrder)fallback, blank);
    }

    public bool CAtelierModeMatch(string? mode)
    {
        return _cAtelierPosture.LPostureModeMatch(mode);
    }

    public void CAtelierModeSave(string mode)
    {
        _cAtelierPosture.LPostureModeSave(mode);
    }

    public bool CAtelierSplitRead()
    {
        return _cAtelierPosture.LPostureRead().LPostureStateSplit;
    }

    public double CAtelierVolumeRead()
    {
        return _cAtelierPosture.LPostureRead().LPostureStateVolume;
    }

    public void CAtelierVolumeSet(double volume, bool settled)
    {
        _cAtelierPosture.LPostureVolumeSet(volume);
        CAtelierMediaPort.LEngineVolumeSet(volume);
        if (settled)
        {
            _cAtelierPosture.LPostureVolumeSave();
        }
    }

    public void CAtelierLeftoverSweep()
    {
        CAtelierDraftPort.LEngineLeftoverSweep();
        CAtelierMediaPort.LEngineRecordingSweep();
    }

    public void CAtelierWorkspaceChange(string path)
    {
        CAtelierSettingsPort.LEngineWorkspaceChange(path);
    }

    public void Dispose()
    {
        CAtelierDraftPort.LEngineLeftoverSweep();
        _cAtelierPosture.Dispose();
    }

    public Action CAtelierObserverAttach(Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return CAtelierObserverAdd(bulletin => observer(CAtelierBulletinRead(bulletin)));
    }

    public Action CAtelierObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return CAtelierObserverAdd(bulletin =>
        {
            if ((CSubject)bulletin.LBulletinSubject == subject)
            {
                observer(CAtelierBulletinRead(bulletin));
            }
        });
    }

    private Action CAtelierObserverAdd(Action<LBulletin> sent)
    {
        CAtelierDraftPort.LEngineObserverAttach(sent);
        return () => CAtelierDraftPort.LEngineObserverDetach(sent);
    }

    private static CBulletin CAtelierBulletinRead(LBulletin bulletin)
    {
        return new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored);
    }
}
